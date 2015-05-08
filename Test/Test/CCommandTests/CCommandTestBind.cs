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
    public class CCommandTestBind : CCommandTest
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
            Execute("bind JoystickButton1 test-1");
            Execute("bind Joystick1Button1 test-2");
            Execute("bind Joystick1Button11 test-3");

            IList<CBinding> list = CBindings.List("Joystick");
            AssertList(list,
                "bind joystickbutton1 test-1",
                "bind joystick1button1 test-2",
                "bind joystick1button11 test-3"
            );

            list = CBindings.List("Joystick1");
            AssertList(list,
                "bind joystick1button1 test-2",
                "bind joystick1button11 test-3"
            );

            list = CBindings.List("Joystick1Button11");
            AssertList(list,
                "bind joystick1button11 test-3"
            );

            list = CBindings.List("Joystick1Button110");
            AssertList(list);

            list = CBindings.List("foo");
            AssertList(list);
        }

        #endregion

        #region Setup

        [SetUp]
        public void SetUp()
        {
            RunSetUp();

            CRegistery.Register(new bind());
            CRegistery.Register(new unbind());
            CRegistery.Register(new bindlist());

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

