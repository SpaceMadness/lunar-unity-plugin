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
    public class AutoCompleteManTest : AutoCompleteTestFixture
    {
        [Test]
        public void TestEmpty()
        {
            string suggestion = DoAutoComplete("man ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestEmptyDoubleTab()
        {
            string suggestion = DoAutoComplete("man ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("debug", "foo", "test1", "test12", "test2");
        }

        [Test]
        public void TestEmptyReleaseDoubleTab()
        {
            OverrideDebugMode(false);

            string suggestion = DoAutoComplete("man ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("foo", "test1", "test12", "test2");
        }

        [Test]
        public void TestFiltered()
        {
            string suggestion = DoAutoComplete("man t");

            Assert.AreEqual("man test", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredDoubleTab()
        {
            string suggestion = DoAutoComplete("man t", true);

            Assert.AreEqual("man test", suggestion);
            AssertDoubleTabSuggestions("test1", "test12", "test2");
        }

        [Test]
        public void TestFilteredSingle()
        {
            string suggestion = DoAutoComplete("man test2");

            Assert.AreEqual("man test2 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredSingleDoubleTab()
        {
            string suggestion = DoAutoComplete("man test2", true);

            Assert.AreEqual("man test2 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredCompleteSingle()
        {
            string suggestion = DoAutoComplete("man test2 ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredCompleteDoubleTab()
        {
            string suggestion = DoAutoComplete("man test2 ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        [SetUp]
        protected override void RunSetUp()
        {
            base.RunSetUp();

            RegisterCommand(typeof(Cmd_man));
            RegisterCommand(typeof(Cmd_alias));
            RegisterCommand(typeof(Cmd_test1), false);
            RegisterCommand(typeof(Cmd_test12), false);
            RegisterCommand(typeof(Cmd_test2), false);
            RegisterCommand(typeof(Cmd_foo), false);
            RegisterCommand(typeof(Cmd_hidden));
            RegisterCommand(typeof(Cmd_debug), false);
            RegisterCommand(typeof(Cmd_system), false);

            new CVar("var", 0); // cvar should be ignored
            Execute("alias test test1"); // alias should be ignored
            Lunar.RegisterCommand("delegate", delegate() {}); // delegate should be ignored
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

        class Cmd_test12 : CCommand
        {
            void Execute()
            {
            }
        }

        class Cmd_test2 : CCommand
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

        class Cmd_hidden : CCommand
        {
            public Cmd_hidden()
            {
                this.IsHidden = true;
            }

            void Execute()
            {
            }
        }

        class Cmd_debug : CCommand
        {
            public Cmd_debug()
            {
                this.IsDebug = true;
            }

            void Execute()
            {
            }
        }

        class Cmd_system : CCommand
        {
            public Cmd_system()
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

