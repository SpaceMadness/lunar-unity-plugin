using System;
using System.Collections.Generic;

using NUnit.Framework;

using LunarPlugin;
using LunarPlugin.Test;
using LunarEditor;
using LunarPluginInternal;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;

    // FIXME: test terminal auto completion instead

    [TestFixture()]
    public class CCommandTestAutoComplete : TestFixtureBase
    {
        #region Options single tab

        [Test]
        public void TestSingleTabEmptyOptionsSingleChoice()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_SingleOptionsTest cmd = new cmd_SingleOptionsTest();
            cmd.Delegate = del;

            string commandLine = "test --";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.AreEqual("test --boolOpt ", cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test()]
        public void TestSingleTabEmptyOptions()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test()]
        public void TestSingleTabOptions()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --o";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test --op1", cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test()]
        public void TestSingleTabOptions2()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --op1";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test --op1", cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test()]
        public void TestSingleTabOptions3()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --op11";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test --op11", cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test()]
        public void TestSingleTabOptions4()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --op111";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test --op111 ", cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test()]
        public void TestSingleTabOptions5()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --op111 ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test()]
        public void TestSingleTabOptions6()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --op111  ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test --op111 ", cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test]
        public void TestSingleTabBoolOption()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test --bool";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.AreEqual("test --boolOpt", cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test]
        public void TestSingleTabBoolOptionSingleChoice()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test --boolOpt2";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.AreEqual("test --boolOpt2 ", cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test]
        public void TestValuesSingleTabNoOptions()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test]
        public void TestValuesSingleTabPartialName()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test v";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.AreEqual("test val", cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test]
        public void TestValuesSingleTabPartialNameSingleChoice()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test f";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.AreEqual("test foo ", cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test]
        public void TestValuesSingleTabPartialNameNoChoice()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test t";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, false));
        }

        #endregion

        #region Options double tab

        [Test()]
        public void TestDoubleTabEmptyOptions()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "--boolOpt1", "--boolOpt12", "--boolOpt2", "--op1", "--op11", "--op111", "--op112", "--op113", "--op12", "--op13");
        }

        [Test()]
        public void TestDoubleTabOptions()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --o";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test --op1", cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "--op1", "--op11", "--op111", "--op112", "--op113", "--op12", "--op13");
        }
        
        [Test()]
        public void TestDoubleTabOptions2()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --op1";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test --op1", cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "--op1", "--op11", "--op111", "--op112", "--op113", "--op12", "--op13");
        }
        
        [Test()]
        public void TestDoubleTabOptions3()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --op11";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test --op11", cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "--op11", "--op111", "--op112", "--op113");
        }
        
        [Test()]
        public void TestDoubleTabOptions4()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --op111";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test --op111 ", cmd.AutoComplete(commandLine, tokens, true));
            Assert.IsNull(del.table);
        }
        
        [Test()]
        public void TestDoubleTabOptions5()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --op111 ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, true));
            Assert.IsNull(del.table);
        }
        
        [Test()]
        public void TestDoubleTabOptions6()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test --op111  ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test --op111 ", cmd.AutoComplete(commandLine, tokens, false));
        }

        [Test]
        public void TestDoubleTabBoolOption()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test --bool";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.AreEqual("test --boolOpt", cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "--boolOpt1", "--boolOpt12", "--boolOpt2");
        }

        [Test]
        public void TestDoubleTabBoolOptionSingleChoice()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test --boolOpt2";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.AreEqual("test --boolOpt2 ", cmd.AutoComplete(commandLine, tokens, true));
        }

        [Test]
        public void TestValuesDoubleTabNoOptions()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "foo", "val1", "val12", "val123", "val2");
        }

        [Test]
        public void TestValuesDoubleTabPartialName()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test v";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.AreEqual("test val", cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "val1", "val12", "val123", "val2");
        }

        [Test]
        public void TestValuesDoubleTabPartialNameSingleChoice()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test f";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.AreEqual("test foo ", cmd.AutoComplete(commandLine, tokens, true));
            Assert.IsNull(del.table);
        }

        [Test]
        public void TestValuesDoubleTabPartialNameNoChoice()
        {
            CommandDelegate del = new CommandDelegate();

            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;

            string commandLine = "test t";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);

            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, true));
            Assert.IsNull(del.table);
        }

        #endregion

        #region Option single short tab
        
        [Test()]
        public void TestSingleTabEmptyShortOptions()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, false));
        }
        
        [Test()]
        public void TestSingleTabShortOptions()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test -o1", cmd.AutoComplete(commandLine, tokens, false));
        }
        
        [Test()]
        public void TestSingleTabShortOptions2()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o1";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test -o1", cmd.AutoComplete(commandLine, tokens, false));
        }
        
        [Test()]
        public void TestSingleTabShortOptions3()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o11";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test -o11", cmd.AutoComplete(commandLine, tokens, false));
        }
        
        [Test()]
        public void TestSingleTabShortOptions4()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o111";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test -o111 ", cmd.AutoComplete(commandLine, tokens, false));
        }
        
        [Test()]
        public void TestSingleTabShortOptions5()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o111 ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, false));
        }
        
        [Test()]
        public void TestSingleTabShortOptions6()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o111  ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test -o111 ", cmd.AutoComplete(commandLine, tokens, false));
        }
        
        #endregion
        
        #region Options double short tab
        
        [Test()]
        public void TestDoubleTabEmptyShortOptions()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "-b1", "-b12", "-b2", "-o1", "-o11", "-o111", "-o112", "-o113", "-o12", "-o13");
        }
        
        [Test()]
        public void TestDoubleTabShortOptions()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test -o1", cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "-o1", "-o11", "-o111", "-o112", "-o113", "-o12", "-o13");
        }
        
        [Test()]
        public void TestDoubleTabShortOptions2()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o1";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test -o1", cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "-o1", "-o11", "-o111", "-o112", "-o113", "-o12", "-o13");
        }
        
        [Test()]
        public void TestDoubleTabShortOptions3()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o11";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test -o11", cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "-o11", "-o111", "-o112", "-o113");
        }
        
        [Test()]
        public void TestDoubleTabShortOptions4()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o111";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test -o111 ", cmd.AutoComplete(commandLine, tokens, true));
            Assert.IsNull(del.table);
        }
        
        [Test()]
        public void TestDoubleTabShortOptions5()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o111 ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, true));
            Assert.IsNull(del.table);
        }
        
        [Test()]
        public void TestDoubleTabShortOptions6()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsTest cmd = new cmd_OptionsTest();
            cmd.Delegate = del;
            
            string commandLine = "test -o111  ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.AreEqual("test -o111 ", cmd.AutoComplete(commandLine, tokens, false));
        }
        
        #endregion

        #region Autocomple options

        [Test()]
        public void TestOptionsValueSingleTab()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsCompleteTest cmd = new cmd_OptionsCompleteTest();
            cmd.Delegate = del;
            
            string commandLine = "test --opt ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, false));
            Assert.IsNull(del.table);
        }

        [Test()]
        public void TestOptionsValueDoubleTab()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsCompleteTest cmd = new cmd_OptionsCompleteTest();
            cmd.Delegate = del;
            
            string commandLine = "test --opt ";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "a2", "aa1", "aa11", "aa111", "aa112", "aa113", "aa12", "aa13", "b");
        }

        [Test()]
        public void TestOptionsValueSingleTab2()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsCompleteTest cmd = new cmd_OptionsCompleteTest();
            cmd.Delegate = del;
            
            string commandLine = "test --opt a";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, false));
            Assert.IsNull(del.table);
        }
        
        [Test()]
        public void TestOptionsValueDoubleTab2()
        {
            CommandDelegate del = new CommandDelegate();
            
            cmd_OptionsCompleteTest cmd = new cmd_OptionsCompleteTest();
            cmd.Delegate = del;
            
            string commandLine = "test --opt a";
            IList<string> tokens = CommandTokenizer.Tokenize(commandLine);
            
            Assert.IsNull(cmd.AutoComplete(commandLine, tokens, true));
            AssertArray(del.table, "a2", "aa1", "aa11", "aa111", "aa112", "aa113", "aa12", "aa13");
        }

        #endregion
    }

    class cmd_OptionsTest : CCommand
    {
        [CCommandOption(ShortName="o1")]
        public string op1;

        [CCommandOption(ShortName="o11")]
        public string op11;

        [CCommandOption(ShortName="o12")]
        public string op12;

        [CCommandOption(ShortName="o13")]
        public string op13;

        [CCommandOption(ShortName="o111")]
        public string op111;

        [CCommandOption(ShortName="o112")]
        public string op112;

        [CCommandOption(ShortName="o113")]
        public string op113;

        [CCommandOption(ShortName="b1")]
        public bool boolOpt1;

        [CCommandOption(ShortName="b12")]
        public bool boolOpt12;

        [CCommandOption(ShortName="b2")]
        public bool boolOpt2;

        public cmd_OptionsTest()
        {
            ResolveOptions(this);
            this.Values = new string[] {
                "foo",
                "val1",
                "val123",
                "val12",
                "val2"
            };
        }

        void Execute()
        {
        }
    }

    class cmd_SingleOptionsTest : CCommand
    {
        [CCommandOption(ShortName="b")]
        public bool boolOpt;

        public cmd_SingleOptionsTest()
        {
            ResolveOptions(this);
        }

        void Execute()
        {
        }
    }

    class cmd_OptionsCompleteTest : CCommand
    {
        [CCommandOption(ShortName="o", Values="aa1,aa11,aa12,aa13,aa111,aa112,aa113,a2,b")]
        public string opt;

        [CCommandOption(ShortName="a", Values="aa1,aa11,aa12,aa13,aa111,aa112,aa113,a2,b")]
        public string act;
        
        public cmd_OptionsCompleteTest()
        {
            ResolveOptions(this);
        }
    }

    class CommandDelegate : ICCommandDelegate
    {
        public string message;
        public string[] table;

        public CommandDelegate Reset()
        {
            message = null;
            table = null;

            return this;
        }

        #region ICCommandDelegate implementation

        public void LogTerminal(string message)
        {
            this.message = StringUtils.RemoveRichTextTags(message);
        }

        public void LogTerminal(string[] t)
        {
            table = new string[t.Length];
            for (int i = 0; i < t.Length; ++i)
            {
                table[i] = StringUtils.RemoveRichTextTags(t[i]);
            }
        }

        public void LogTerminal(CVar[] cvars)
        {
            throw new NotImplementedException();
        }

        public void ClearTerminal()
        {
        }

        public bool ExecuteCommandLine(string commandLine, bool manualMode = false)
        {
            return false;
        }

        public void PostNotification(CCommand cmd, string name, params object[] data)
        {
        }

        public bool IsPromptEnabled
        {
            get { return false; }
        }

        #endregion
    }
}

