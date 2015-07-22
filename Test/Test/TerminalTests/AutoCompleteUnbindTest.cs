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
    public class AutoCompleteUnbindTest : AutoCompleteTestFixture
    {
        [Test]
        public void TestEmpty()
        {
            string suggestion = DoAutoComplete("unbind ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestEmptyDoubleTab()
        {
            string suggestion = DoAutoComplete("unbind ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("ctrl+k", "ctrl+t", "mouse0", "mouse1", "mouse2");
        }

        [Test]
        public void TestFiltered()
        {
            string suggestion = DoAutoComplete("unbind mous");

            Assert.AreEqual("unbind mouse", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredDoubleTab()
        {
            string suggestion = DoAutoComplete("unbind mous", true);

            Assert.AreEqual("unbind mouse", suggestion);
            AssertDoubleTabSuggestions("mouse0", "mouse1", "mouse2");
        }

        [Test]
        public void TestFilteredRestricted()
        {
            string suggestion = DoAutoComplete("unbind mouse");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredRestrictedDoubleTab()
        {
            string suggestion = DoAutoComplete("unbind mouse", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("mouse0", "mouse1", "mouse2");
        }

        [Test]
        public void TestFilteredShortcut()
        {
            string suggestion = DoAutoComplete("unbind ctrl+");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredShortcutDoubleTab()
        {
            string suggestion = DoAutoComplete("unbind ctrl+", true);

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
            RegisterCommand(typeof(Cmd_unbind));

            Execute("bind mouse0 test");
            Execute("bind mouse1 test");
            Execute("bind mouse2 test");
            Execute("bind ctrl+t t");
            Execute("bind ctrl+k k");
        }

        #endregion
    }
}

