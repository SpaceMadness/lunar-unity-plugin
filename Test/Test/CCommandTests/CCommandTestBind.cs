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

        #region Input

        [Test]
        public void TestKeyDown()
        {
            Execute("bind t 'test arg'");

            TapKeys(KeyCode.T);
            AssertResult("test arg");

            RunUpdate();
            AssertResult();
        }

        [Test]
        public void TestModifierShiftKeyDown()
        {
            Execute("bind shift+t 'test arg'");

            TapKeys(KeyCode.LeftShift, KeyCode.T);
            AssertResult("test arg");

            RunUpdate();
            AssertResult();

            TapKeys(KeyCode.RightShift, KeyCode.T);
            AssertResult("test arg");
        }

        [Test]
        public void TestModifierCtrlKeyDown()
        {
            Execute("bind ctrl+t 'test arg'");

            TapKeys(KeyCode.LeftControl, KeyCode.T);
            AssertResult("test arg");

            RunUpdate();
            AssertResult();

            TapKeys(KeyCode.RightControl, KeyCode.T);
            AssertResult("test arg");
        }

        [Test]
        public void TestModifierAltKeyDown()
        {
            Execute("bind alt+t 'test arg'");

            TapKeys(KeyCode.LeftAlt, KeyCode.T);
            AssertResult("test arg");

            RunUpdate();
            AssertResult();

            TapKeys(KeyCode.RightAlt, KeyCode.T);
            AssertResult("test arg");
        }

        [Test]
        public void TestMultipleModifiersKeyDown()
        {
            Execute("bind ctrl+shift+alt+t 'test arg'");

            TapKeys(KeyCode.LeftControl, KeyCode.LeftShift, KeyCode.LeftAlt, KeyCode.T);
            AssertResult("test arg");

            RunUpdate();
            AssertResult();

            TapKeys(KeyCode.LeftControl, KeyCode.LeftShift, KeyCode.T);
            AssertResult();

            TapKeys(KeyCode.LeftControl, KeyCode.T);
            AssertResult();

            TapKeys(KeyCode.T);
            AssertResult();
        }

        #endregion

        #region +/- bindings

        [Test]
        public void TestListOperationCommands()
        {
            Execute("bind mouse0 +bool");

            Assert.AreEqual(0, CRegistery.ListCommands("+bool").Count);
            Assert.AreEqual(0, CRegistery.ListCommands("-bool").Count);
        }

        [Test]
        public void TestListSystemCommands()
        {
            Execute("bind mouse0 +bool");

            Assert.AreEqual(1, CRegistery.ListCommands("+bool", CCommandListOptions.System).Count);
            Assert.AreEqual(1, CRegistery.ListCommands("-bool", CCommandListOptions.System).Count);
        }

        [Test]
        public void TestHoldPositiveKeyBoolVar()
        {
            CVar cvar = new CVar("bool", false);

            Execute("bind mouse0 +bool");

            PressKeys(KeyCode.Mouse0);
            Assert.IsTrue(cvar.BoolValue);

            RunUpdate();
            Assert.IsTrue(cvar.BoolValue);

            RunUpdate();
            Assert.IsTrue(cvar.BoolValue);

            ReleaseKeys(KeyCode.Mouse0);
            Assert.IsFalse(cvar.BoolValue);
        }

        [Test]
        public void TestHoldNegativeKeyBoolVar()
        {
            CVar cvar = new CVar("bool", true);

            Execute("bind mouse0 -bool");

            PressKeys(KeyCode.Mouse0);
            Assert.IsFalse(cvar.BoolValue);

            RunUpdate();
            Assert.IsFalse(cvar.BoolValue);

            RunUpdate();
            Assert.IsFalse(cvar.BoolValue);

            ReleaseKeys(KeyCode.Mouse0);
            Assert.IsTrue(cvar.BoolValue);
        }

        [Test]
        public void TestHoldKeyUnsupportedVarType()
        {
            new CVar("int", 0);

            this.IsTrackTerminalLog = true;

            Execute("bind mouse0 +int");

            PressKeys(KeyCode.Mouse0);
            AssertResult("  Boolean variable expected: 'int'");
        }

        [Test]
        public void TestHoldKeyMissingVar()
        {
            this.IsTrackTerminalLog = true;

            Execute("bind mouse0 +foo");

            PressKeys(KeyCode.Mouse0);

            AssertResult("  Can't find boolean variable: 'foo'");
        }

        #endregion

        #region Variable Delegates

        [Test]
        public void TestDelegates()
        {
            bool delegateCalled = false;

            CVar cvar = new CVar("var", "Default value");
            cvar.AddDelegate(delegate(CVar v)
            {
                delegateCalled = true;
            });

            Execute("bind v 'var value'");

            TapKeys(KeyCode.V);
            Assert.IsTrue(delegateCalled);
        }

        [Test]
        public void TestDelegatesOperationCommand()
        {
            string delegateValue = null;

            CVar cvar = new CVar("var", false);
            cvar.AddDelegate(delegate(CVar v)
            {
                delegateValue = v.BoolValue.ToString();
            });

            Execute("bind v +var");

            PressKeys(KeyCode.V);
            Assert.AreEqual("True", delegateValue);

            ReleaseKeys(KeyCode.V);
            Assert.AreEqual("False", delegateValue);
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

        protected new void AssertResult(params string[] expected)
        {
            base.AssertResult(expected);
            ClearResult();
        }

        #endregion
    }
}

