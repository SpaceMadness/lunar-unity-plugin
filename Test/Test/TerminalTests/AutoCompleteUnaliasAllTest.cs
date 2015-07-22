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
    public class AutoCompleteUnaliasAllTest : AutoCompleteTestFixture
    {
        [Test]
        public void TestEmpty()
        {
            string suggestion = DoAutoComplete("unaliasAll ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestEmptyDoubleTab()
        {
            string suggestion = DoAutoComplete("unaliasAll ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("foo", "test1", "test12", "test2");
        }

        [Test]
        public void TestEmptyReleaseDoubleTab()
        {
            OverrideDebugMode(false);

            string suggestion = DoAutoComplete("unaliasAll ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("foo", "test1", "test12", "test2");
        }

        [Test]
        public void TestFiltered()
        {
            string suggestion = DoAutoComplete("unaliasAll t");

            Assert.AreEqual("unaliasAll test", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredDoubleTab()
        {
            string suggestion = DoAutoComplete("unaliasAll t", true);

            Assert.AreEqual("unaliasAll test", suggestion);
            AssertDoubleTabSuggestions("test1", "test12", "test2");
        }

        [Test]
        public void TestFilteredSingle()
        {
            string suggestion = DoAutoComplete("unaliasAll test2");

            Assert.AreEqual("unaliasAll test2 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredSingleDoubleTab()
        {
            string suggestion = DoAutoComplete("unaliasAll test2", true);

            Assert.AreEqual("unaliasAll test2 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredCompleteSingle()
        {
            string suggestion = DoAutoComplete("unaliasAll test2 ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredCompleteDoubleTab()
        {
            string suggestion = DoAutoComplete("unaliasAll test2 ", true);

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
            RegisterCommand(typeof(Cmd_unaliasAll));

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

