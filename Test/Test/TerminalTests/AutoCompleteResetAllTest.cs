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
    public class AutoCompleteResetAllTest : AutoCompleteTestFixture
    {
        [Test]
        public void TestEmpty()
        {
            string suggestion = DoAutoComplete("resetAll ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestEmptyDoubleTab()
        {
            string suggestion = DoAutoComplete("resetAll ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("debug", "foo", "test1", "test12", "test2");
        }

        [Test]
        public void TestEmptyReleaseDoubleTab()
        {
            OverrideDebugMode(false);

            string suggestion = DoAutoComplete("resetAll ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("foo", "test1", "test12", "test2");
        }

        [Test]
        public void TestFiltered()
        {
            string suggestion = DoAutoComplete("resetAll t");

            Assert.AreEqual("resetAll test", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredDoubleTab()
        {
            string suggestion = DoAutoComplete("resetAll t", true);

            Assert.AreEqual("resetAll test", suggestion);
            AssertDoubleTabSuggestions("test1", "test12", "test2");
        }

        [Test]
        public void TestFilteredSingle()
        {
            string suggestion = DoAutoComplete("resetAll test2");

            Assert.AreEqual("resetAll test2 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredSingleDoubleTab()
        {
            string suggestion = DoAutoComplete("resetAll test2", true);

            Assert.AreEqual("resetAll test2 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredCompleteSingle()
        {
            string suggestion = DoAutoComplete("resetAll test2 ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredCompleteDoubleTab()
        {
            string suggestion = DoAutoComplete("resetAll test2 ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        [SetUp]
        protected override void RunSetUp()
        {
            base.RunSetUp();

            new CVar("test1", 0);
            new CVar("test12", 0);
            new CVar("test2", 0);
            new CVar("foo", 0);
            new CVar("debug", 0, CFlags.Debug);

            RegisterCommand(typeof(Cmd_test3), false);
            RegisterCommand(typeof(Cmd_resetAll));
            RegisterCommand(typeof(Cmd_alias));

            Execute("alias test4 test1");

            Lunar.RegisterCommand("test5", delegate() {}); // delegate should be ignored
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Classes

        class Cmd_test3 : CCommand
        {
            void Execute()
            {
            }
        }

        #endregion
    }
}

