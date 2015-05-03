using System;
using System.Threading;
using System.Collections.Generic;

using NUnit.Framework;

using LunarPlugin;
using LunarPlugin.Test;
using LunarEditor;
using LunarPluginInternal;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;
    using CommandAction = CommandAction<string[]>;

    [TestFixture()]
    public class CCommandTestCRegistryTest : TestFixtureBase
    {
        [Test()]
        public void TestListCommands()
        {
            CRegistery.Clear();

            CCommand a11 = new cmd_a11();
            CCommand a12 = new cmd_a12();
            CCommand b11 = new cmd_b11();
            CCommand b12 = new cmd_b12();

            CRegistery.Register(a11);
            CRegistery.Register(a12);
            CRegistery.Register(b11);
            CRegistery.Register(b12);

            IList<CCommand> commands = CRegistery.ListCommands();
            AssertTypes(commands, typeof(cmd_a11), typeof(cmd_a12), typeof(cmd_b11), typeof(cmd_b12));

            commands = CRegistery.ListCommands("a");
            AssertTypes(commands, typeof(cmd_a11), typeof(cmd_a12));

            commands = CRegistery.ListCommands("a1");
            AssertTypes(commands, typeof(cmd_a11), typeof(cmd_a12));

            commands = CRegistery.ListCommands("a11");
            AssertTypes(commands, typeof(cmd_a11));

            commands = CRegistery.ListCommands("a13");
            AssertTypes(commands);
        }

        [Test()]
        public void TestListVars()
        {
            CRegistery.Clear();

            CVar a11 = new CVar("a11", "value");
            CVar a12 = new CVar("a12", "value");
            CVar b11 = new CVar("b11", "value");
            CVar b12 = new CVar("b12", "value");

            IList<CVar> vars = CRegistery.ListVars();
            AssertList(vars, a11, a12, b11, b12);

            vars = CRegistery.ListVars("a");
            AssertList(vars, a11, a12);

            vars = CRegistery.ListVars("a1");
            AssertList(vars, a11, a12);

            vars = CRegistery.ListVars("a11");
            AssertList(vars, a11);

            vars = CRegistery.ListVars("a13");
            AssertList(vars);
        }

        [Test()]
        public void TestRegisterMethodsCommands()
        {
            CRegistery.Clear();

            Lunar.RegisterCommand("del1", Del1);
            Lunar.RegisterCommand("del2", Del2);
            Lunar.RegisterCommand("del3", Del3);

            IList<CCommand> cmds = CRegistery.ListCommands("del");
            Assert.AreEqual(3, cmds.Count);
            Assert.AreEqual("del1", cmds[0].Name);
            Assert.AreEqual("del2", cmds[1].Name);
            Assert.AreEqual("del3", cmds[2].Name);
        }

        [Test()]
        public void TestRegisterMultipleMethodsCommands()
        {
            CRegistery.Clear();
            
            Lunar.RegisterCommand("del1", Del1);
            Lunar.RegisterCommand("del2", Del2);
            Lunar.RegisterCommand("del2", Del2);
            Lunar.RegisterCommand("del3", Del3);
            
            IList<CCommand> cmds = CRegistery.ListCommands("del");
            Assert.AreEqual(3, cmds.Count);
            Assert.AreEqual("del1", cmds[0].Name);
            Assert.AreEqual("del2", cmds[1].Name);
            Assert.AreEqual("del3", cmds[2].Name);
        }

        /*
        [Test()]
        public void TestRegisterMethodsCommandsGC()
        {
            CRegistery.Clear();

            WeakReference reference = RegisterDummyDelegate("del");
            GC.Collect(int.MaxValue, GCCollectionMode.Forced);

            while (reference.Target != null)
            {
                Thread.Sleep(100);
            }
            
            List<CCommand> cmds = CRegistery.ListCommands("del");
            Assert.AreEqual(1, cmds.Count);
            Assert.IsNull(((CDelegateCommand) cmds[0]).Delegate);

            Dummy dummy = new Dummy();
            CRegistery.Register("del", dummy.Execute);
            cmds = CRegistery.ListCommands("del");
            Assert.AreEqual(1, cmds.Count);
            Assert.AreEqual(((CDelegateCommand) cmds[0]).Delegate, (CCommandExecutionDelegate)dummy.Execute);
        }
        */

        [Test()]
        public void TestRegisterMethodsCommandsFromTheSameObject()
        {
            CRegistery.Clear();

            Dummy dummy = new Dummy();

            CommandAction<string[]> del1 = dummy.Execute;
            CommandAction<string[]> del2 = dummy.Execute2;

            Lunar.RegisterCommand("del1", del1);
            Lunar.RegisterCommand("del2", del2);

            IList<CCommand> cmds = CRegistery.ListCommands("del");
            Assert.AreEqual(2, cmds.Count);
            Assert.AreEqual(del1, (cmds[0] as CDelegateCommand).ActionDelegate);
            Assert.AreEqual(del2, (cmds[1] as CDelegateCommand).ActionDelegate);

            CRegistery.UnregisterAll(dummy);
            cmds = CRegistery.ListCommands("del");
            Assert.AreEqual(0, cmds.Count);
        }

        /* we should allocate object on a separate stack */
        private WeakReference RegisterDummyDelegate(string name)
        {
            Dummy dummy = new Dummy();
            WeakReference reference = new WeakReference(new CommandAction<string[]>(dummy.Execute));
            
            Lunar.RegisterCommand(name, dummy.Execute);

            return reference;
        }

        #region CCommandExecutionDelegates

        public void Del1(string[] args)
        {
        }

        public void Del2(string[] args)
        {
        }

        public void Del3(string[] args)
        {
        }

        #endregion
    }

    class AbstractCommand : CCommand
    {
        public AbstractCommand(string name)
        {
            Name = name;
        }

        void Execute()
        {
        }
    }

    class cmd_a11 : AbstractCommand
    {
        public cmd_a11()
            : base("a11")
        {
        }
    }

    class cmd_a12 : AbstractCommand
    {
        public cmd_a12()
            : base("a12")
        {
        }
    }

    class cmd_b11 : AbstractCommand
    {
        public cmd_b11()
            : base("b11")
        {
        }
    }

    class cmd_b12 : AbstractCommand
    {
        public cmd_b12()
            : base("b12")
        {
        }
    }

    class Dummy
    {
        public void Execute(string[] args)
        {
        }

        public void Execute2(string[] args)
        {
        }
    }
}