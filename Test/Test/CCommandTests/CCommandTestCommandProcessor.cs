using System;
using System.Collections.Generic;

using NUnit.Framework;

using LunarPlugin;
using LunarPlugin.Test;
using LunarEditor;
using LunarPluginInternal;

namespace CCommandTests
{
    [TestFixture()]
    public class CCommandTestCommandProcessor : TestFixtureBase
    {
        #region Tokenizer

        [Test()]
        public void TestTokenize()
        {
            string str = "one two three";

            IList<string> tokens = Tokenize(str);
            AssertList(tokens, "one", "two", "three");
        }

        [Test()]
        public void TestTokenizeWithSpaces()
        {
            string str = "\"one and a half\" two three";
            
            IList<string> tokens = Tokenize(str);
            AssertList(tokens, "one and a half", "two", "three");

            str = "one \"two and a half\" three";
            tokens = Tokenize(str);
            AssertList(tokens, "one", "two and a half", "three");

            str = "one two \"three and a half\"";
            tokens = Tokenize(str);
            AssertList(tokens, "one", "two", "three and a half");
        }

        [Test()]
        public void TestTokenizeWithMultipleSpaces()
        {
            string str = "  one  two   three  ";
            
            IList<string> tokens = Tokenize(str);
            AssertList(tokens, "one", "two", "three");
        }

        [Test()]
        public void TestTokenizeWithSpacesAndEscapedQuotes()
        {
            string str = "one \"two \\\"and a\\\" half\" three";
            IList<string> tokens = Tokenize(str);
            AssertList(tokens, "one", "two \\\"and a\\\" half", "three");
        }

        [Test]
        public void TestQuotesNoSpace()
        {
            string str = "alias a1=\"test\"";
            IList<string> tokens = Tokenize(str);
            AssertList(tokens, "alias", "a1=\"test\"");
        }

        [Test]
        public void TestSingleQuotesNoSpace()
        {
            string str = "alias a1='test'";
            IList<string> tokens = Tokenize(str);
            AssertList(tokens, "alias", "a1='test'");
        }

        [Test]
        public void TestSingleQuotesWithSpaces()
        {
            string str = "alias a1='echo \"alias 1\"'";
            IList<string> tokens = Tokenize(str);
            AssertList(tokens, "alias", "a1='echo \"alias 1\"'");
        }

        [Test]
        public void TestMore()
        {
            string str = "bind t 'echo \"test-1\" && echo \"test-2\"'";
            IList<string> tokens = Tokenize(str);
            AssertList(tokens, "bind", "t", "echo \"test-1\" && echo \"test-2\"");
        }

        #endregion

        #region Helpers

        private static IList<string> Tokenize(string str)
        {
            return CCommandTokenizer.Tokenize(str);
        }

        #endregion
    }
}

