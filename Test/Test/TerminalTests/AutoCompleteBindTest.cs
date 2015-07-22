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
    public class AutoCompleteBindTest : AutoCompleteTestFixture
    {
        [Test]
        public void TestEmpty()
        {
            string suggestion = DoAutoComplete("bind ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestEmptyDoubleTab()
        {
            string suggestion = DoAutoComplete("bind ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("-", ",", ".", "/", ";", "[", "\\", "]", "~", "=", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "backspace", "break", "c", "capslock", "d", "delete", "down", "e", "end", "enter", "escape", "f", "f1", "f10", "f11", "f12", "f13", "f14", "f15", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "g", "h", "home", "i", "insert", "j", "joy0", "joy1", "joy10", "joy11", "joy12", "joy13", "joy14", "joy15", "joy16", "joy17", "joy18", "joy19", "joy2", "joy3", "joy4", "joy5", "joy6", "joy7", "joy8", "joy9", "k", "kp_divide", "kp_enter", "kp_equals", "kp_minus", "kp_multiply", "kp_period", "kp_plus", "l", "left", "leftalt", "leftcmd", "leftctrl", "leftshift", "m", "mouse0", "mouse1", "mouse2", "mouse3", "mouse4", "mouse5", "mouse6", "n", "num0", "num1", "num2", "num3", "num4", "num5", "num6", "num7", "num8", "num9", "numlock", "o", "p", "pagedown", "pageup", "pause", "print", "q", "r", "right", "rightalt", "rightcmd", "rightctrl", "rightshift", "s", "scrolllock", "space", "t", "tab", "u", "up", "v", "w", "x", "y", "z");
        }

        [Test]
        public void TestFiltered()
        {
            string suggestion = DoAutoComplete("bind mous");

            Assert.AreEqual("bind mouse", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredDoubleTab()
        {
            string suggestion = DoAutoComplete("bind mous", true);

            Assert.AreEqual("bind mouse", suggestion);
            AssertDoubleTabSuggestions("mouse0", "mouse1", "mouse2", "mouse3", "mouse4", "mouse5", "mouse6");
        }

        [Test]
        public void TestFilteredRestricted()
        {
            string suggestion = DoAutoComplete("bind mouse");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestFilteredRestrictedDoubleTab()
        {
            string suggestion = DoAutoComplete("bind mouse", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("mouse0", "mouse1", "mouse2", "mouse3", "mouse4", "mouse5", "mouse6");
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        [SetUp]
        protected override void RunSetUp()
        {
            base.RunSetUp();

            RegisterCommand(typeof(Cmd_bind));
        }

        #endregion
    }
}

