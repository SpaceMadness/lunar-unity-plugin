using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using LunarPlugin;
using LunarPluginInternal;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;
    using Option = CCommand.Option;

    [TestFixture]
    public class CCommandTestPrintUsage : CCommandTest
    {
        [Test]
        public void TestPrintUsageArrayArg()
        {
            Execute("man test1");
            AssertResult("  usage: test1 ...");
        }

        [Test]
        public void TestPrintUsageMultipleExecuteMethods()
        {
            Execute("man test2");
            AssertResult(
                "  usage: test2\n" + 
                "         test2 <name>\n" + 
                "         test2 <name> <command>"
            );
        }

        [Test]
        public void TestPrintUsageExecuteMethodOptionalArgs()
        {
            Execute("man test3");
            AssertResult("  usage: test3 <name> [<command>]");
        }

        [Test]
        public void TestPrintOptionsUsage()
        {
            Execute("man test4");
            AssertResult("  usage:" +
                " test4" + 
                " [--str <string>]" +
                " [--int <int>]" +
                " [--float <float>]" +
                " [--bool]" +
                " [--ints <int,int>]" +
                " [--floats <float,float,float>]" +
                " [--bools <bool,bool,bool,bool>]" +
                " [--strings <string,string,string,string,string>]" +
                " <arg>"
            );
        }

        #region Setup

        [SetUp]
        public void SetUp()
        {
            RunSetUp();

            this.IsTrackTerminalLog = true;

            CRegistery.Register(new man());
            CRegistery.Register(new cmd_test1());
            CRegistery.Register(new cmd_test2());
            CRegistery.Register(new cmd_test3());
            CRegistery.Register(new cmd_test4());
        }

        [TearDown]
        public void TearDown()
        {
            RunTearDown();
        }

        #endregion

        #region Inner classes

        class cmd_test1 : CCommand
        {
            public cmd_test1()
                : base("test1")
            {
                ResolveOptions(this);
            }

            void Execute(string[] args)
            {
            }
        }

        class cmd_test2 : CCommand
        {
            public cmd_test2()
                : base("test2")
            {
                ResolveOptions(this);
            }

            void Execute()
            {
            }

            void Execute(string name)
            {
            }

            void Execute(string name, string command)
            {
            }
        }

        class cmd_test3 : CCommand
        {
            public cmd_test3()
                : base("test3")
            {
                ResolveOptions(this);
            }

            void Execute(string name, string command = null)
            {
            }
        }

        class cmd_test4 : CCommand
        {
            [CCommandOption(Name = "str")]
            public string m_str;

            [CCommandOption(Name = "int")]
            public int m_int;

            [CCommandOption(Name = "float")]
            public float m_float;

            [CCommandOption(Name = "bool")]
            public bool m_bool;

            [CCommandOption()]
            public int[] ints = new int[2];

            [CCommandOption()]
            public float[] floats = new float[3];

            [CCommandOption()]
            public bool[] bools = new bool[4];

            [CCommandOption()]
            public string[] strings = new string[5];

            public cmd_test4()
                : base("test4")
            {
                ResolveOptions(this);
            }

            void Execute(string arg)
            {
            }
        }

        #endregion
    }
}

