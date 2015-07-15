using System;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using LunarPlugin;
using LunarPluginInternal;
using LunarEditor;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class CCommandTestBind : CCommandTestFixture
    {
        #region Binds

        [Test]
        public void TestSingleBind()
        {
            Execute("bind t test");

            IList<CBinding> list = CBindings.List();
            AssertList(list, "bind t test");
        }

        [Test]
        public void TestSingleBindWithArg()
        {
            Execute("bind t \"test arg\"");

            IList<CBinding> list = CBindings.List();
            AssertList(list, "bind t \"test arg\"");
        }

        [Test]
        public void TestSingleBindWithArgSingleQuotes()
        {
            Execute("bind t 'test arg'");

            IList<CBinding> list = CBindings.List();
            AssertList(list, "bind t \"test arg\"");
        }

        [Test]
        public void TestSingleBindWithInnerArgs()
        {
            Execute("bind t \"test arg1 \\\"arg2 arg3\\\" arg4\"");

            IList<CBinding> list = CBindings.List();
            AssertList(list, "bind t \"test arg1 \\\"arg2 arg3\\\" arg4\"");
        }

        [Test]
        public void TestSingleBindWithInnerArgsAndSingleQuotes()
        {
            Execute("bind t 'test arg1 \"arg2 arg3\" arg4'");

            IList<CBinding> list = CBindings.List();
            AssertList(list, "bind t \"test arg1 \\\"arg2 arg3\\\" arg4\"");
        }

        #endregion

        #region Replacement

        [Test]
        public void TestBindReplaceItem()
        {
            Execute("bind a test-1");
            Execute("bind b test-2");
            Execute("bind c test-3");
            Execute("bind d test-4");

            Execute("bind d test-5");
            Execute("bind a test-6");
            Execute("bind c test-7");
            Execute("bind b test-8");

            IList<CBinding> list = CBindings.List();
            AssertList(list, 
                "bind a test-6",
                "bind b test-8",
                "bind c test-7",
                "bind d test-5"
            );
        }

        #endregion

        #region Unbind

        [Test]
        public void TestUnbind()
        {
            Execute("bind a test-1");
            Execute("bind b test-2");
            Execute("bind c test-3");

            IList<CBinding> list = CBindings.List();
            AssertList(list,
                "bind a test-1",
                "bind b test-2",
                "bind c test-3"
            );

            Execute("unbind a");
            list = CBindings.List();
            AssertList(list,
                "bind b test-2",
                "bind c test-3"
            );

            Execute("unbind c");
            list = CBindings.List();
            AssertList(list,
                "bind b test-2"
            );

            Execute("unbind b");
            list = CBindings.List();
            AssertList(list);

            Execute("unbind b");
            list = CBindings.List();
            AssertList(list);
        }

        #endregion

        #region List binding

        [Test]
        public void TestListBindings()
        {
            Execute("bind mouse0 test-1");
            Execute("bind mouse1 test-2");
            Execute("bind m test-3");

            IList<CBinding> list = CBindings.List("m");
            AssertList(list,
                "bind mouse0 test-1",
                "bind mouse1 test-2",
                "bind m test-3"
            );

            list = CBindings.List("mo");
            AssertList(list,
                "bind mouse0 test-1",
                "bind mouse1 test-2"
            );

            list = CBindings.List("mouse0");
            AssertList(list,
                "bind mouse0 test-1"
            );

            list = CBindings.List("mouse2");
            AssertList(list);

            list = CBindings.List("foo");
            AssertList(list);
        }

        #endregion

        #region Auto Completion

        [Test]
        public void TestAutocompletionSingleTabSingleChoice()
        {
//            CommandDelegate del = new CommandDelegate();
//
//            bind cmd = new bind();
//            cmd.Delegate = del;
//
//            string commandLine = "bind del";
//            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
//
//            Assert.AreEqual("bind delete ", cmd.AutoComplete(commandLine, tokens, false));

            throw new NotImplementedException("Implement me");
        }

        [Test]
        public void TestAutocompletionSingleTabMultipleChoice()
        {
//            CommandDelegate del = new CommandDelegate();
//
//            bind cmd = new bind();
//            cmd.Delegate = del;
//
//            string commandLine = "bind mou";
//            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
//
//            Assert.AreEqual("bind mouse", cmd.AutoComplete(commandLine, tokens, false));

            throw new NotImplementedException("Implement me");
        }

        [Test]
        public void TestAutocompletionDoubleTabMultipleChoice()
        {
//            CommandDelegate del = new CommandDelegate();
//
//            bind cmd = new bind();
//            cmd.Delegate = del;
//
//            string commandLine = "bind mou";
//            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
//
//            Assert.AreEqual("bind mouse", cmd.AutoComplete(commandLine, tokens, true));
//            AssertArray(del.table, "mouse0", "mouse1", "mouse2", "mouse3", "mouse4", "mouse5", "mouse6");

            throw new NotImplementedException("Implement me");
        }

        #endregion

        #region Setup

        [SetUp]
        public void SetUp()
        {
            RunSetUp();

            RegisterCommand(typeof(Cmd_bind));
            RegisterCommand(typeof(Cmd_unbind));
            RegisterCommand(typeof(Cmd_bindlist));

            Lunar.RegisterCommand("test", delegate(CCommand cmd, string[] args)
            {
                AddResult(cmd.CommandString);
            });
        }

        [TearDown]
        public void TearDown()
        {
            RunTearDown();
        }

        #endregion

        #region Helpers

        internal void AssertList(IList<CBinding> actual, params string[] expected)
        {
            Assert.AreEqual(actual.Count, expected.Length, StringUtils.TryFormat("Expected: [{0}]\nActual: [{1}]"), Join(", ", expected), Join(", ", actual));
            for (int i = 0; i < expected.Length; ++i)
            {
                Assert.AreEqual(expected[i], Cmd_bind.ToString(actual[i]));
            }
        }

        #endregion
    }
}

