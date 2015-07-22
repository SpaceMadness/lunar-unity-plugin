using System;
using System.Collections.Generic;

using NUnit.Framework;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using LunarPlugin.Test;
using CCommandTests;

namespace TerminalTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class AutoCompleteCmdlistTest : AutoCompleteTestFixture
    {
        [Test]
        public void TestEmpty()
        {
            string suggestion = DoAutoComplete("cmdlist ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestEmptyDoubleTab()
        {
            string suggestion = DoAutoComplete("cmdlist ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("foo", "test1", "test12", "test2", "test3");
        }

        [Test]
        public void TestEmptyReleaseDoubleTab()
        {
            OverrideDebugMode(false);

            string suggestion = DoAutoComplete("cmdlist ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("foo", "test1", "test12", "test2");
        }

        [Test]
        public void TestFiltered()
        {
            string suggestion = DoAutoComplete("cmdlist t");

            Assert.AreEqual("cmdlist test", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredDoubleTab()
        {
            string suggestion = DoAutoComplete("cmdlist t", true);

            Assert.AreEqual("cmdlist test", suggestion);
            AssertDoubleTabSuggestions("test1", "test12", "test2", "test3");
        }

        [Test]
        public void TestFilteredReleaseDoubleTab()
        {
            OverrideDebugMode(false);

            string suggestion = DoAutoComplete("cmdlist t", true);

            Assert.AreEqual("cmdlist test", suggestion);
            AssertDoubleTabSuggestions("test1", "test12", "test2");
        }

        [Test]
        public void TestFilteredSingle()
        {
            string suggestion = DoAutoComplete("cmdlist test2");

            Assert.AreEqual("cmdlist test2 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredSingleDoubleTab()
        {
            string suggestion = DoAutoComplete("cmdlist test2", true);

            Assert.AreEqual("cmdlist test2 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredCompleteSingle()
        {
            string suggestion = DoAutoComplete("cmdlist test2 ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredCompleteDoubleTab()
        {
            string suggestion = DoAutoComplete("cmdlist test2 ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        [SetUp]
        protected override void RunSetUp()
        {
            base.RunSetUp();

            RegisterCommand(typeof(Cmd_cmdlist));
            RegisterCommand(typeof(Cmd_alias));
            RegisterCommand(typeof(Cmd_test1), false);
            RegisterCommand(typeof(Cmd_foo), false);
            RegisterCommand(typeof(Cmd_test3), false);
            RegisterCommand(typeof(Cmd_test4), true); // hidden
            RegisterCommand(typeof(Cmd_test5), false);

            new CVar("var", 0); // var should be ignored
            Lunar.RegisterCommand("test12", delegate() {}); // delegate
            Execute("alias test2 test1");
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Classes

        class Cmd_test1 : CCommand
        {
            void Execute()
            {
            }
        }

        class Cmd_foo : CCommand
        {
            void Execute()
            {
            }
        }

        class Cmd_test3 : CCommand
        {
            public Cmd_test3()
            {
                this.IsDebug = true;
            }

            void Execute()
            {
            }
        }

        class Cmd_test4 : CCommand
        {
            public Cmd_test4()
            {
                this.IsHidden = true;
            }

            void Execute()
            {
            }
        }

        class Cmd_test5 : CCommand
        {
            public Cmd_test5()
            {
                this.IsSystem = true;
            }

            void Execute()
            {
            }
        }

        #endregion
    }
}

