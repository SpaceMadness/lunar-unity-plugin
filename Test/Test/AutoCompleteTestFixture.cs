using NUnit.Framework;

using System;
using System.Collections.Generic;

using LunarPlugin;
using LunarPluginInternal;

using LunarEditor;

using UnityEngine;
using CCommandTests;

namespace LunarPlugin.Test
{
    public class AutoCompleteTestFixture : CCommandTestFixture, IConsoleDelegate
    {
        private CTerminal terminal;
        private List<string> terminalTableOutput;

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        [SetUp]
        protected override void RunSetUp()
        {
            base.RunSetUp();

            terminal = new CTerminal(1024);
            terminal.Delegate = this;
            terminalTableOutput = new List<string>();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        protected string DoAutoComplete(string text, bool isDoubleTap = false)
        {
            int index = text.IndexOf('¶');
            index = index == -1 ? text.Length : index;

            return terminal.DoAutoComplete(text, index, isDoubleTap);
        }

        protected void AssertDoubleTabSuggestions(params string[] expected)
        {
            AssertList(terminalTableOutput, expected);
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
                    terminalTableOutput.Add(CStringUtils.RemoveRichTextTags(item));
                }
            }
        }

        public void OnConsoleCleared(AbstractConsole console)
        {
        }

        #endregion
    }
}

