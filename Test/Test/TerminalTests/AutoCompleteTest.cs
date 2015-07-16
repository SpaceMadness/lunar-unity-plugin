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
    public class AutoCompleteTest : CCommandTestFixture, IConsoleDelegate
    {
        private Terminal terminal;
        private List<string> terminalTableOutput;

        #region Command Autocompletion

        [Test]
        public void TestAutoCompleteCommandsEmptyLine()
        {
            string suggestion = DoAutoComplete("");

            Assert.AreEqual("test", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCommandsDoubleTabEmptyLine()
        {
            string suggestion = DoAutoComplete("", true);

            Assert.AreEqual("test", suggestion);
            AssertSuggestions("test1", "test12", "test2");
        }

        [Test]
        public void TestAutoCompleteCommands1()
        {
            string suggestion = DoAutoComplete("t");

            Assert.AreEqual("test", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCommandsDoubleTab1()
        {
            string suggestion = DoAutoComplete("t", true);

            Assert.AreEqual("test", suggestion);
            AssertSuggestions("test1", "test12", "test2");
        }

        [Test]
        public void TestAutoCompleteCommands2()
        {
            string suggestion = DoAutoComplete("test");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCommandsDoubleTab2()
        {
            string suggestion = DoAutoComplete("test", true);

            Assert.IsNull(suggestion);
            AssertSuggestions("test1", "test12", "test2");
        }

        [Test]
        public void TestAutoCompleteCommands3()
        {
            string suggestion = DoAutoComplete("test1");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCommandsDoubleTab3()
        {
            string suggestion = DoAutoComplete("test1", true);

            Assert.IsNull(suggestion);
            AssertSuggestions("test1", "test12");
        }

        [Test]
        public void TestAutoCompleteCommands4()
        {
            string suggestion = DoAutoComplete("test12");

            Assert.AreEqual("test12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCommandsDoubleTab4()
        {
            string suggestion = DoAutoComplete("test12", true);

            Assert.AreEqual("test12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCommands5()
        {
            string suggestion = DoAutoComplete("test123");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCommandsDoubleTab5()
        {
            string suggestion = DoAutoComplete("test123", true);

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Arguments autocompletion

        [Test]
        public void TestAutoCompleteArgumentsEmptyLine()
        {
            string suggestion = DoAutoComplete("test1 ");

            Assert.AreEqual("test1 arg", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteArgumentsDoubleTabEmptyLine()
        {
            string suggestion = DoAutoComplete("test1 ", true);

            Assert.AreEqual("test1 arg", suggestion);
            AssertSuggestions("arg1", "arg12", "arg2");
        }

        [Test]
        public void TestAutoCompleteArguments1()
        {
            string suggestion = DoAutoComplete("test1 a");

            Assert.AreEqual("test1 arg", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteArgumentsDoubleTab1()
        {
            string suggestion = DoAutoComplete("test1 a", true);

            Assert.AreEqual("test1 arg", suggestion);
            AssertSuggestions("arg1", "arg12", "arg2");
        }

        [Test]
        public void TestAutoCompleteArguments2()
        {
            string suggestion = DoAutoComplete("test1 arg1");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteArgumentsDoubleTab2()
        {
            string suggestion = DoAutoComplete("test1 arg1", true);

            Assert.IsNull(suggestion);
            AssertSuggestions("arg1", "arg12");
        }

        [Test]
        public void TestAutoCompleteArguments3()
        {
            string suggestion = DoAutoComplete("test1 arg12");

            Assert.AreEqual("test1 arg12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteArgumentsDoubleTab3()
        {
            string suggestion = DoAutoComplete("test1 arg12", true);

            Assert.AreEqual("test1 arg12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteArguments4()
        {
            string suggestion = DoAutoComplete("test1 arg123");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteArgumentsDoubleTab4()
        {
            string suggestion = DoAutoComplete("test1 arg123", true);

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteArguments5()
        {
            string suggestion = DoAutoComplete("test1 arg1 ");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteArgumentsDoubleTab5()
        {
            string suggestion = DoAutoComplete("test1 arg1 ", true);

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCustomArguments1()
        {
            string suggestion = DoAutoComplete("test12 a");

            Assert.AreEqual("test12 arg", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCustomArgumentsDoubleTab1()
        {
            string suggestion = DoAutoComplete("test12 a", true);

            Assert.AreEqual("test12 arg", suggestion);
            AssertSuggestions("arg1", "arg12", "arg2");
        }

        [Test]
        public void TestAutoCompleteCustomArguments2()
        {
            string suggestion = DoAutoComplete("test12 arg1");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCustomArgumentsDoubleTab2()
        {
            string suggestion = DoAutoComplete("test12 arg1", true);

            Assert.IsNull(suggestion);
            AssertSuggestions("arg1", "arg12");
        }

        [Test]
        public void TestAutoCompleteCustomArguments3()
        {
            string suggestion = DoAutoComplete("test12 arg12");

            Assert.AreEqual("test12 arg12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCustomArgumentsDoubleTab3()
        {
            string suggestion = DoAutoComplete("test12 arg12", true);

            Assert.AreEqual("test12 arg12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCustomArguments4()
        {
            string suggestion = DoAutoComplete("test12 arg123");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCustomArgumentsDoubleTab4()
        {
            string suggestion = DoAutoComplete("test12 arg123", true);

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCustomArguments5()
        {
            string suggestion = DoAutoComplete("test12 arg1 ");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteCustomArgumentsDoubleTab5()
        {
            string suggestion = DoAutoComplete("test12 arg1 ", true);

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteArgumentsWithException()
        {
            string suggestion = DoAutoComplete("test2 a");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteArgumentsWithExceptionDoubleTab()
        {
            string suggestion = DoAutoComplete("test2 a", true);

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Auto

        [Test]
        public void TestAutoCompleteShortOptionsEmptyLine()
        {
            string suggestion = DoAutoComplete("test1 -");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsEmptyLineDoubleTab()
        {
            string suggestion = DoAutoComplete("test1 -", true);

            Assert.IsNull(suggestion);
            AssertSuggestions("-e", "-o1", "-o12", "-o2");
        }

        [Test]
        public void TestAutoCompleteShortOptions1()
        {
            string suggestion = DoAutoComplete("test1 -o");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsDoubleTab1()
        {
            string suggestion = DoAutoComplete("test1 -o", true);

            Assert.IsNull(suggestion);
            AssertSuggestions("-o1", "-o12", "-o2");
        }

        [Test]
        public void TestAutoCompleteShortOptions2()
        {
            string suggestion = DoAutoComplete("test1 -o12");

            Assert.AreEqual("test1 -o12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsDoubleTab2()
        {
            string suggestion = DoAutoComplete("test1 -o12", true);

            Assert.AreEqual("test1 -o12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptions3()
        {
            string suggestion = DoAutoComplete("test1 -o123");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsDoubleTab3()
        {
            string suggestion = DoAutoComplete("test1 -o123", true);

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptions4()
        {
            string suggestion = DoAutoComplete("test1 -o123");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsDoubleTab4()
        {
            string suggestion = DoAutoComplete("test1 -o123", true);

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValue1()
        {
            string suggestion = DoAutoComplete("test1 -o2 ");

            Assert.AreEqual("test1 -o2 val", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueDoubleTab1()
        {
            string suggestion = DoAutoComplete("test1 -o2 ", true);

            Assert.AreEqual("test1 -o2 val", suggestion);
            AssertSuggestions("val1", "val12", "val2");
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValue2()
        {
            string suggestion = DoAutoComplete("test1 -o2 v");

            Assert.AreEqual("test1 -o2 val", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueDoubleTab2()
        {
            string suggestion = DoAutoComplete("test1 -o2 v", true);

            Assert.AreEqual("test1 -o2 val", suggestion);
            AssertSuggestions("val1", "val12", "val2");
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValue3()
        {
            string suggestion = DoAutoComplete("test1 -o2 val");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueDoubleTab3()
        {
            string suggestion = DoAutoComplete("test1 -o2 val", true);

            Assert.IsNull(suggestion);
            AssertSuggestions("val1", "val12", "val2");
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValue4()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueDoubleTab4()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1", true);

            Assert.IsNull(suggestion);
            AssertSuggestions("val1", "val12");
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValue5()
        {
            string suggestion = DoAutoComplete("test1 -o2 val12");

            Assert.AreEqual("test1 -o2 val12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueDoubleTab5()
        {
            string suggestion = DoAutoComplete("test1 -o2 val12", true);

            Assert.AreEqual("test1 -o2 val12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValue6()
        {
            string suggestion = DoAutoComplete("test1 -o2 x");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueDoubleTab6()
        {
            string suggestion = DoAutoComplete("test1 -o2 x", true);

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgs()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 ");

            Assert.AreEqual("test1 -o2 val1 arg", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgsDoubleTab()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 ", true);

            Assert.AreEqual("test1 -o2 val1 arg", suggestion);
            AssertSuggestions("arg1", "arg12", "arg2");
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgs1()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 a");

            Assert.AreEqual("test1 -o2 val1 arg", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgsDoubleTab1()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 a", true);

            Assert.AreEqual("test1 -o2 val1 arg", suggestion);
            AssertSuggestions("arg1", "arg12", "arg2");
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgs2()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg1");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgsDoubleTab2()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg1", true);

            Assert.IsNull(suggestion);
            AssertSuggestions("arg1", "arg12");
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgs3()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg12");

            Assert.AreEqual("test1 -o2 val1 arg12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgsDoubleTab3()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg12", true);

            Assert.AreEqual("test1 -o2 val1 arg12 ", suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgs4()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg123");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgsDoubleTab4()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg123", true);

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgs5()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg1 ");

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        [Test]
        public void TestAutoCompleteShortOptionsWithValueAndArgsDoubleTab5()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg1 ", true);

            Assert.IsNull(suggestion);
            AssertSuggestions();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        [SetUp]
        protected override void RunSetUp()
        {
            base.RunSetUp();

            RegisterCommand(typeof(Cmd_test1), false);
            RegisterCommand(typeof(Cmd_test12), false);
            RegisterCommand(typeof(Cmd_test2), false);

            terminal = new Terminal(1024);
            terminal.Delegate = this;
            terminalTableOutput = new List<string>();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private string DoAutoComplete(string text, bool isDoubleTap = false)
        {
            int index = text.IndexOf('¶');
            index = index == -1 ? text.Length : index;

            return terminal.DoAutoComplete(text, index, isDoubleTap);
        }

        private void AssertSuggestions(params string[] expected)
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
                    terminalTableOutput.Add(StringUtils.RemoveRichTextTags(item));
                }
            }
        }

        public void OnConsoleCleared(AbstractConsole console)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Test commands

        class Cmd_test1 : CCommand
        {
            [CCommandOption(ShortName="o1")]
            private bool opt1;

            [CCommandOption(ShortName="o12")]
            private String opt12;

            [CCommandOption(ShortName="o2", Values="val1,val12,val2")]
            private String opt2;

            [CCommandOption(ShortName="e")]
            private String ext;

            public Cmd_test1()
            {
                this.Values = new string[]
                {
                    "arg1",
                    "arg12",
                    "arg2",
                };
            }

            void Execute(string arg)
            {
            }
        }

        class Cmd_test12 : CCommand
        {
            [CCommandOption]
            private bool opt1;

            void Execute(string arg)
            {
            }

            protected override string[] AutoCompleteArgs(string commandLine, string token)
            {
                string[] values = new string[]
                {
                    "arg1",
                    "arg12",
                    "arg2",
                };

                List<string> suggestions = new List<string>();
                foreach (string val in values)
                {
                    if (StringUtils.StartsWithIgnoreCase(val, token))
                    {
                        suggestions.Add(val);
                    }
                }

                return suggestions.ToArray();
            }
        }

        class Cmd_test2 : CCommand
        {
            void Execute()
            {
            }

            protected override string[] AutoCompleteArgs(string commandLine, string token)
            {
                throw new Exception("Uh-oh");
            }
        }

        #endregion
    }
}

