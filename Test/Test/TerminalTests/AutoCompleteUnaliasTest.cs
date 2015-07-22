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
    public class AutoCompleteUnaliasTest : AutoCompleteTestFixture
    {
        [Test]
        public void TestEmpty()
        {
            string suggestion = DoAutoComplete("unalias ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestEmptyDoubleTab()
        {
            string suggestion = DoAutoComplete("unalias ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("foo", "test1", "test12", "test2");
        }

        [Test]
        public void TestEmptyReleaseDoubleTab()
        {
            OverrideDebugMode(false);

            string suggestion = DoAutoComplete("unalias ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("foo", "test1", "test12", "test2");
        }

        [Test]
        public void TestFiltered()
        {
            string suggestion = DoAutoComplete("unalias t");

            Assert.AreEqual("unalias test", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredDoubleTab()
        {
            string suggestion = DoAutoComplete("unalias t", true);

            Assert.AreEqual("unalias test", suggestion);
            AssertDoubleTabSuggestions("test1", "test12", "test2");
        }

        [Test]
        public void TestFilteredSingle()
        {
            string suggestion = DoAutoComplete("unalias test2");

            Assert.AreEqual("unalias test2 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredSingleDoubleTab()
        {
            string suggestion = DoAutoComplete("unalias test2", true);

            Assert.AreEqual("unalias test2 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredCompleteSingle()
        {
            string suggestion = DoAutoComplete("unalias test2 ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredCompleteDoubleTab()
        {
            string suggestion = DoAutoComplete("unalias test2 ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        [SetUp]
        protected override void RunSetUp()
        {
            base.RunSetUp();

            RegisterCommand(typeof(Cmd_alias));
            RegisterCommand(typeof(Cmd_unalias));

            Execute("alias test1 test");
            Execute("alias test12 test");
            Execute("alias test2 test");
            Execute("alias foo test");

            RegisterCommand(typeof(Cmd_test3), false);
            new CVar("test4", 0);

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

