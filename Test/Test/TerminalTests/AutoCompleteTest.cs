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
    public class AutoCompleteTest : AutoCompleteTestFixture
    {
        #region Command Autocompletion

        [Test]
        public void TestCommandsEmptyLine()
        {
            string suggestion = DoAutoComplete("");

            Assert.AreEqual("test", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCommandsDoubleTabEmptyLine()
        {
            string suggestion = DoAutoComplete("", true);

            Assert.AreEqual("test", suggestion);
            AssertDoubleTabSuggestions("test1", "test12", "test2");
        }

        [Test]
        public void TestCommands1()
        {
            string suggestion = DoAutoComplete("t");

            Assert.AreEqual("test", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCommandsDoubleTab1()
        {
            string suggestion = DoAutoComplete("t", true);

            Assert.AreEqual("test", suggestion);
            AssertDoubleTabSuggestions("test1", "test12", "test2");
        }

        [Test]
        public void TestCommands2()
        {
            string suggestion = DoAutoComplete("test");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCommandsDoubleTab2()
        {
            string suggestion = DoAutoComplete("test", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("test1", "test12", "test2");
        }

        [Test]
        public void TestCommands3()
        {
            string suggestion = DoAutoComplete("test1");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCommandsDoubleTab3()
        {
            string suggestion = DoAutoComplete("test1", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("test1", "test12");
        }

        [Test]
        public void TestCommands4()
        {
            string suggestion = DoAutoComplete("test12");

            Assert.AreEqual("test12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCommandsDoubleTab4()
        {
            string suggestion = DoAutoComplete("test12", true);

            Assert.AreEqual("test12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCommands5()
        {
            string suggestion = DoAutoComplete("test123");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCommandsDoubleTab5()
        {
            string suggestion = DoAutoComplete("test123", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Arguments autocompletion

        [Test]
        public void TestArgumentsEmptyLine()
        {
            string suggestion = DoAutoComplete("test1 ");

            Assert.AreEqual("test1 arg", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestArgumentsDoubleTabEmptyLine()
        {
            string suggestion = DoAutoComplete("test1 ", true);

            Assert.AreEqual("test1 arg", suggestion);
            AssertDoubleTabSuggestions("arg1", "arg12", "arg2");
        }

        [Test]
        public void TestArguments1()
        {
            string suggestion = DoAutoComplete("test1 a");

            Assert.AreEqual("test1 arg", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestArgumentsDoubleTab1()
        {
            string suggestion = DoAutoComplete("test1 a", true);

            Assert.AreEqual("test1 arg", suggestion);
            AssertDoubleTabSuggestions("arg1", "arg12", "arg2");
        }

        [Test]
        public void TestArguments2()
        {
            string suggestion = DoAutoComplete("test1 arg1");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestArgumentsDoubleTab2()
        {
            string suggestion = DoAutoComplete("test1 arg1", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("arg1", "arg12");
        }

        [Test]
        public void TestArguments3()
        {
            string suggestion = DoAutoComplete("test1 arg12");

            Assert.AreEqual("test1 arg12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestArgumentsDoubleTab3()
        {
            string suggestion = DoAutoComplete("test1 arg12", true);

            Assert.AreEqual("test1 arg12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestArguments4()
        {
            string suggestion = DoAutoComplete("test1 arg123");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestArgumentsDoubleTab4()
        {
            string suggestion = DoAutoComplete("test1 arg123", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestArguments5()
        {
            string suggestion = DoAutoComplete("test1 arg1 ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestArgumentsDoubleTab5()
        {
            string suggestion = DoAutoComplete("test1 arg1 ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCustomArguments1()
        {
            string suggestion = DoAutoComplete("test12 a");

            Assert.AreEqual("test12 arg", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCustomArgumentsDoubleTab1()
        {
            string suggestion = DoAutoComplete("test12 a", true);

            Assert.AreEqual("test12 arg", suggestion);
            AssertDoubleTabSuggestions("arg1", "arg12", "arg2");
        }

        [Test]
        public void TestCustomArguments2()
        {
            string suggestion = DoAutoComplete("test12 arg1");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCustomArgumentsDoubleTab2()
        {
            string suggestion = DoAutoComplete("test12 arg1", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("arg1", "arg12");
        }

        [Test]
        public void TestCustomArguments3()
        {
            string suggestion = DoAutoComplete("test12 arg12");

            Assert.AreEqual("test12 arg12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCustomArgumentsDoubleTab3()
        {
            string suggestion = DoAutoComplete("test12 arg12", true);

            Assert.AreEqual("test12 arg12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCustomArguments4()
        {
            string suggestion = DoAutoComplete("test12 arg123");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCustomArgumentsDoubleTab4()
        {
            string suggestion = DoAutoComplete("test12 arg123", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCustomArguments5()
        {
            string suggestion = DoAutoComplete("test12 arg1 ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestCustomArgumentsDoubleTab5()
        {
            string suggestion = DoAutoComplete("test12 arg1 ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestArgumentsWithException()
        {
            string suggestion = DoAutoComplete("test2 a");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestArgumentsWithExceptionDoubleTab()
        {
            string suggestion = DoAutoComplete("test2 a", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Auto

        [Test]
        public void TestShortOptionsEmptyLine()
        {
            string suggestion = DoAutoComplete("test1 -");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsEmptyLineDoubleTab()
        {
            string suggestion = DoAutoComplete("test1 -", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("-e", "-o1", "-o12", "-o2");
        }

        [Test]
        public void TestShortOptions1()
        {
            string suggestion = DoAutoComplete("test1 -o");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsDoubleTab1()
        {
            string suggestion = DoAutoComplete("test1 -o", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("-o1", "-o12", "-o2");
        }

        [Test]
        public void TestShortOptions2()
        {
            string suggestion = DoAutoComplete("test1 -o12");

            Assert.AreEqual("test1 -o12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsDoubleTab2()
        {
            string suggestion = DoAutoComplete("test1 -o12", true);

            Assert.AreEqual("test1 -o12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptions3()
        {
            string suggestion = DoAutoComplete("test1 -o123");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsDoubleTab3()
        {
            string suggestion = DoAutoComplete("test1 -o123", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptions4()
        {
            string suggestion = DoAutoComplete("test1 -o123");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsDoubleTab4()
        {
            string suggestion = DoAutoComplete("test1 -o123", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValue1()
        {
            string suggestion = DoAutoComplete("test1 -o2 ");

            Assert.AreEqual("test1 -o2 val", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueDoubleTab1()
        {
            string suggestion = DoAutoComplete("test1 -o2 ", true);

            Assert.AreEqual("test1 -o2 val", suggestion);
            AssertDoubleTabSuggestions("val1", "val12", "val2");
        }

        [Test]
        public void TestShortOptionsWithValue2()
        {
            string suggestion = DoAutoComplete("test1 -o2 v");

            Assert.AreEqual("test1 -o2 val", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueDoubleTab2()
        {
            string suggestion = DoAutoComplete("test1 -o2 v", true);

            Assert.AreEqual("test1 -o2 val", suggestion);
            AssertDoubleTabSuggestions("val1", "val12", "val2");
        }

        [Test]
        public void TestShortOptionsWithValue3()
        {
            string suggestion = DoAutoComplete("test1 -o2 val");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueDoubleTab3()
        {
            string suggestion = DoAutoComplete("test1 -o2 val", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("val1", "val12", "val2");
        }

        [Test]
        public void TestShortOptionsWithValue4()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueDoubleTab4()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("val1", "val12");
        }

        [Test]
        public void TestShortOptionsWithValue5()
        {
            string suggestion = DoAutoComplete("test1 -o2 val12");

            Assert.AreEqual("test1 -o2 val12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueDoubleTab5()
        {
            string suggestion = DoAutoComplete("test1 -o2 val12", true);

            Assert.AreEqual("test1 -o2 val12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValue6()
        {
            string suggestion = DoAutoComplete("test1 -o2 x");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueDoubleTab6()
        {
            string suggestion = DoAutoComplete("test1 -o2 x", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueAndArgs()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 ");

            Assert.AreEqual("test1 -o2 val1 arg", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueAndArgsDoubleTab()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 ", true);

            Assert.AreEqual("test1 -o2 val1 arg", suggestion);
            AssertDoubleTabSuggestions("arg1", "arg12", "arg2");
        }

        [Test]
        public void TestShortOptionsWithValueAndArgs1()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 a");

            Assert.AreEqual("test1 -o2 val1 arg", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueAndArgsDoubleTab1()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 a", true);

            Assert.AreEqual("test1 -o2 val1 arg", suggestion);
            AssertDoubleTabSuggestions("arg1", "arg12", "arg2");
        }

        [Test]
        public void TestShortOptionsWithValueAndArgs2()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg1");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueAndArgsDoubleTab2()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg1", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions("arg1", "arg12");
        }

        [Test]
        public void TestShortOptionsWithValueAndArgs3()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg12");

            Assert.AreEqual("test1 -o2 val1 arg12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueAndArgsDoubleTab3()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg12", true);

            Assert.AreEqual("test1 -o2 val1 arg12 ", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueAndArgs4()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg123");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueAndArgsDoubleTab4()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg123", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueAndArgs5()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg1 ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsWithValueAndArgsDoubleTab5()
        {
            string suggestion = DoAutoComplete("test1 -o2 val1 arg1 ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsMultiple()
        {
            string suggestion = DoAutoComplete("test1 -o1 -o12 ");

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsMultipleDoubleTab()
        {
            string suggestion = DoAutoComplete("test1 -o1 -o12 ", true);

            Assert.IsNull(suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsMultipleWithValue()
        {
            string suggestion = DoAutoComplete("test1 -o1 -o2 ");

            Assert.AreEqual("test1 -o1 -o2 val", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsMultipleWithValueDoubleTab()
        {
            string suggestion = DoAutoComplete("test1 -o1 -o2 ", true);

            Assert.AreEqual("test1 -o1 -o2 val", suggestion);
            AssertDoubleTabSuggestions("val1", "val12", "val2");
        }

        [Test]
        public void TestShortOptionsMultipleWithValueAndArgs()
        {
            string suggestion = DoAutoComplete("test1 -o1 -o2 val1 ");

            Assert.AreEqual("test1 -o1 -o2 val1 arg", suggestion);
            AssertDoubleTabSuggestions();
        }

        [Test]
        public void TestShortOptionsMultipleWithValueAndArgsDoubleTab()
        {
            string suggestion = DoAutoComplete("test1 -o1 -o2 val1 ", true);

            Assert.AreEqual("test1 -o1 -o2 val1 arg", suggestion);
            AssertDoubleTabSuggestions("arg1", "arg12", "arg2");
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
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Test commands

        class Cmd_test1 : CCommand
        {
            [CCommandOption(ShortName="o1")]
            public bool opt1;

            [CCommandOption(ShortName="o12")]
            public String opt12;

            [CCommandOption(ShortName="o2", Values="val1,val12,val2")]
            public String opt2;

            [CCommandOption(ShortName="e")]
            public String ext;

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
            public bool opt1;

            void Execute(string arg)
            {
            }

            protected override IList<string> AutoCompleteArgs(string commandLine, string token)
            {
                string[] values = new string[]
                {
                    "arg1",
                    "arg12",
                    "arg2",
                };

                return CStringUtils.Filter(values, token);
            }
        }

        class Cmd_test2 : CCommand
        {
            void Execute()
            {
            }

            protected override IList<string> AutoCompleteArgs(string commandLine, string token)
            {
                throw new Exception("Uh-oh");
            }
        }

        #endregion
    }
}

