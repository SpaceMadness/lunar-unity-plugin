using System;
using System.Collections.Generic;

using NUnit.Framework;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using LunarPlugin.Test;

namespace TerminalTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class AutoCompleteTest : TestFixtureBase, IConsoleDelegate
    {
        private Terminal terminal;
        private List<string> terminalTableOutput;

        [SetUp]
        public void SetUp()
        {
            CRegistery.Clear();

            terminal = new Terminal(1024);
            terminal.Delegate = this;
            terminalTableOutput = new List<string>();
        }

        [Test]
        public void TestEmptyTextCvarsAutoComplete()
        {
            new CVar("c1", "value");
            new CVar("c12", "value");
            new CVar("c123", "value");
            new CVar("c14", "value");

            string expected = "";
            string actual = DoAutoComplete("");

            Assert.AreEqual(expected, actual);
            AssertList(terminalTableOutput);
        }

        [Test]
        public void TestEmptyTextCvarsAutoCompleteDoubleTap()
        {
            new CVar("c14", "value");
            new CVar("c123", "value");
            new CVar("c1", "value");
            new CVar("c12", "value");

            string expected = "";
            string actual = DoAutoComplete("", true);

            Assert.AreEqual(expected, actual);
            AssertList(terminalTableOutput, "c1", "c12", "c123", "c14");
        }

        [Test]
        public void TestTextCvarsAutoComplete()
        {
            new CVar("c14", "value");
            new CVar("c123", "value");
            new CVar("c1", "value");
            new CVar("c12", "value");

            string expected = "c1";
            string actual = DoAutoComplete("c", false);

            Assert.AreEqual(expected, actual);
            AssertList(terminalTableOutput);
        }

        [Test]
        public void TestTextCvarsAutoCompleteDoubleTap()
        {
            new CVar("c14", "value");
            new CVar("c123", "value");
            new CVar("c1", "value");
            new CVar("c12", "value");

            string expected = "c1";
            string actual = DoAutoComplete("c", true);

            Assert.AreEqual(expected, actual);
            AssertList(terminalTableOutput, "c1", "c12", "c123", "c14");
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private string DoAutoComplete(string text, bool isDoubleTap = false)
        {
            return terminal.DoAutoComplete(text, isDoubleTap);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IConsoleDelegate implementation

        public void OnConsoleEntryAdded(AbstractConsole console, ref ConsoleViewCellEntry entry)
        {
            if (entry.IsTable)
            {
                string[] table = entry.Table;
                foreach (string item in table)
                {
                    terminalTableOutput.Add(StringUtils.RemoveRichTextTags(item));
                }
            }
        }

        public void OnConsoleCleared(AbstractConsole console)
        {
        }

        #endregion
    }
}

