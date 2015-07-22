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
    public class AutoCompleteUnbindAllTest : AutoCompleteTestFixture
    {
        [Test]
        public void TestEmpty()
        {
            string suggestion = DoAutoComplete("unbindAll ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestEmptylDoubleTab()
        {
            string suggestion = DoAutoComplete("unbindAll ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("ctrl+k", "ctrl+t", "mouse0", "mouse1", "mouse2");
        }

        [Test]
        public void TestFiltered()
        {
            string suggestion = DoAutoComplete("unbindAll mous");

            Assert.AreEqual("unbindAll mouse", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredDoubleTab()
        {
            string suggestion = DoAutoComplete("unbindAll mous", true);

            Assert.AreEqual("unbindAll mouse", suggestion);
            AssertDoubleTabSuggestions("mouse0", "mouse1", "mouse2");
        }

        [Test]
        public void TestFilteredRestricted()
        {
            string suggestion = DoAutoComplete("unbindAll mouse");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredRestrictedDoubleTab()
        {
            string suggestion = DoAutoComplete("unbindAll mouse", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("mouse0", "mouse1", "mouse2");
        }

        [Test]
        public void TestFilteredShortcut()
        {
            string suggestion = DoAutoComplete("unbindAll ctrl+");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredShortcutDoubleTab()
        {
            string suggestion = DoAutoComplete("unbindAll ctrl+", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("ctrl+k", "ctrl+t");
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        [SetUp]
        protected override void RunSetUp()
        {
            base.RunSetUp();

            RegisterCommand(typeof(Cmd_bind));
            RegisterCommand(typeof(Cmd_unbindAll));

            Execute("bind mouse0 test");
            Execute("bind mouse1 test");
            Execute("bind mouse2 test");
            Execute("bind ctrl+t t");
            Execute("bind ctrl+k k");
        }

        #endregion
    }
}

