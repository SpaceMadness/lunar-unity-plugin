using System;
using NUnit.Framework;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using UnityEngine;

namespace LunarPlugin.Test
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture()]
    public class StringUtilsTest
    {
        #region Args

        [Test()]
        public void TestArgWithNoSpace()
        {
            string arg = "arg";
            string escaped = arg;
            Assert.AreEqual(escaped, StringUtils.Arg(arg));
            Assert.AreEqual(arg, StringUtils.UnArg(escaped));
        }

        [Test()]
        public void TestArgWithSpace()
        {
            string arg = "some arg with spaces";
            string escaped = '"' + arg + '"';
            Assert.AreEqual(escaped, StringUtils.Arg(arg));
            Assert.AreEqual(arg, StringUtils.UnArg(escaped));
        }

        [Test()]
        public void TestArgWithQuotes()
        {
            string arg = "\"quotes\"";
            string escaped = "\\\"quotes\\\"";
            Assert.AreEqual(escaped, StringUtils.Arg(arg));
            Assert.AreEqual(arg, StringUtils.UnArg(escaped));
        }

        [Test()]
        public void TestArgWithQuotesAndSpaces()
        {
            string arg = "spaces and \"quotes\"";
            string escaped = "\"spaces and \\\"quotes\\\"\"";
            Assert.AreEqual(escaped, StringUtils.Arg(arg));
            Assert.AreEqual(arg, StringUtils.UnArg(escaped));
        }

        [Test()]
        public void TestArgWithSpaceAndSingleQuotes()
        {
            string arg = "some 'arg with' spaces";
            string escaped = "\"some \\'arg with\\' spaces\"";
            Assert.AreEqual(escaped, StringUtils.Arg(arg));
            Assert.AreEqual(arg, StringUtils.UnArg(escaped));
        }

        [Test()]
        public void TestArgWithSingleQuotes()
        {
            string arg = "'quotes'";
            string escaped = "\\'quotes\\'";
            Assert.AreEqual(escaped, StringUtils.Arg(arg));
            Assert.AreEqual(arg, StringUtils.UnArg(escaped));
        }

        #endregion

        #region Words

        [Test]
        public void TestFindingWordStart()
        {
            string value = "abcd";
            AssertWordStartIndex(value, "abcd");

            value = "abcd efgh";
            AssertWordStartIndex(value, "abcd ");
            AssertWordStartIndex(value, "efgh");

            value = " a b c ";
            AssertWordStartIndex(value, "a ");
            AssertWordStartIndex(value, "b ");
            AssertWordStartIndex(value, "c ");

            value = "a def g hij  k";
            AssertWordStartIndex(value, "a ");
            AssertWordStartIndex(value, "def ");
            AssertWordStartIndex(value, "g ");
            AssertWordStartIndex(value, "hij  ");
            AssertWordStartIndex(value, "k");

            value = "a";
            AssertWordStartIndex(value, "a");

            value = "\n";
            AssertWordStartIndex(value, "\n");
        }

        [Test]
        public void TestFindingWordEnd()
        {
            string value = "abcd";
            AssertWordEndIndex(value, "abcd");

            value = "abcd efgh";
            AssertWordEndIndex(value, "abcd");
            AssertWordEndIndex(value, " efgh");

            value = " a b c ";
            AssertWordEndIndex(value, " a");
            AssertWordEndIndex(value, " b");
            AssertWordEndIndex(value, " c");

            value = "a def g hij  k";
            AssertWordEndIndex(value, "a");
            AssertWordEndIndex(value, " def");
            AssertWordEndIndex(value, " g");
            AssertWordEndIndex(value, " hij");
            AssertWordEndIndex(value, "  k");

            value = "a";
            AssertWordEndIndex(value, "a");

            value = "\n";
            AssertWordEndIndex(value, "\n");
        }

        private static void AssertWordStartIndex(string value, string word)
        {
            int startIndex = value.IndexOf(word);
            Assert.AreNotEqual(-1, startIndex);

            int endIndex = startIndex + word.Length;
            for (int i = startIndex + 1; i <= endIndex; ++i)
            {
                Assert.AreEqual(startIndex, StringUtils.StartOfTheWord(value, i), "\"" + word + "\":" + i);
            }
        }

        private static void AssertWordEndIndex(string value, string word)
        {
            int startIndex = value.IndexOf(word);
            Assert.AreNotEqual(-1, startIndex);

            int endIndex = startIndex + word.Length;
            for (int i = startIndex; i < endIndex; ++i)
            {
                Assert.AreEqual(endIndex, StringUtils.EndOfTheWord(value, i), "\"" + word + "\":" + i);
            }
        }

        #endregion

        #region Tags

        [Test]
        public void TestRemoveBoldStyleTag()
        {
            string expected = "text";
            string formatted = StringUtils.B(expected);
            Assert.AreEqual(expected, StringUtils.RemoveRichTextTags(formatted));
        }

        [Test]
        public void TestRemoveItalicStyleTag()
        {
            string expected = "text";
            string formatted = StringUtils.I(expected);
            Assert.AreEqual(expected, StringUtils.RemoveRichTextTags(formatted));
        }

        [Test]
        public void TestRemoveColorTag()
        {
            string expected = "text";
            string formatted = StringUtils.C(expected, ColorCode.Clear);
            Assert.AreEqual(expected, StringUtils.RemoveRichTextTags(formatted));
        }

        [Test]
        public void TestRemoveColorMultipleTags()
        {
            string expected = "red green blue";
            string formatted = string.Format("{0} {1} {2}", 
                StringUtils.C("red", ColorCode.Clear),
                StringUtils.C("green", ColorCode.Error),
                StringUtils.C("blue", ColorCode.LevelDebug)
            );
            Assert.AreEqual(expected, StringUtils.RemoveRichTextTags(formatted));
        }

        [Test]
        public void TestRemoveColorMultipleInnerTags()
        {
            string expected = "red green blue";
            string formatted = StringUtils.C(string.Format("{0} {1} {2}", 
                StringUtils.C("red", ColorCode.Clear),
                StringUtils.C("green", ColorCode.Error),
                StringUtils.C("blue", ColorCode.LevelDebug)
            ), ColorCode.LevelError);
            Assert.AreEqual(expected, StringUtils.RemoveRichTextTags(formatted));
        }

        [Test]
        public void TestRemoveColorMultipleInnerStyleTags()
        {
            string expected = "red green blue";
            string formatted = StringUtils.B(string.Format("{0} {1} {2}", 
                StringUtils.C("red", ColorCode.Clear),
                StringUtils.C("green", ColorCode.Error),
                StringUtils.C("blue", ColorCode.LevelDebug)
            ));
            Assert.AreEqual(expected, StringUtils.RemoveRichTextTags(formatted));
        }

        #endregion

        #region Colors

        [Test]
        public void TestInsertColors()
        {
            Color[] lookup =
            {
                Color.red,
                Color.green,
                Color.blue
            };

            string formatted = string.Format("{0} {1} {2}", 
                StringUtils.C("red", (ColorCode)0),
                StringUtils.C("green", (ColorCode)1),
                StringUtils.C("blue", (ColorCode)2)
            );

            string expected = string.Format("{0} {1} {2}", 
                StringUtils.C("red", lookup[0]),
                StringUtils.C("green", lookup[1]),
                StringUtils.C("blue", lookup[2])
            );

            string actual = StringUtils.SetColors(formatted, lookup);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestInsertColorsWrongLookup()
        {
            Color[] lookup =
            {
                Color.red,
                Color.green,
                Color.blue
            };

            string formatted = string.Format("{0} {1} {2}", 
                StringUtils.C("red", (ColorCode)0),
                StringUtils.C("green", (ColorCode)1),
                StringUtils.C("blue", (ColorCode)2),
                StringUtils.C("yellow", (ColorCode)3)
            );

            string expected = string.Format("{0} {1} {2}", 
                StringUtils.C("red", lookup[0]),
                StringUtils.C("green", lookup[1]),
                StringUtils.C("blue", lookup[2]),
                StringUtils.C("yellow", (ColorCode)3)
            );

            string actual = StringUtils.SetColors(formatted, lookup);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Lines

        [Test]
        public void TestCountLineBreaksSingleLine()
        {
            string line = "line";
            Assert.AreEqual(0, StringUtils.LinesBreaksCount(line));
        }

        [Test]
        public void TestCountLineBreaksTwoLines()
        {
            string line = "line1\n" +
                          "line2";

            Assert.AreEqual(1, StringUtils.LinesBreaksCount(line));
        }

        [Test]
        public void TestCountLineBreaksThreeLines()
        {
            string line = "line1\n" +
                          "line2\n" +
                          "line3";

            Assert.AreEqual(2, StringUtils.LinesBreaksCount(line));
        }

        [Test]
        public void TestCountLineBreaksNullLine()
        {
            string line = null;
            Assert.AreEqual(0, StringUtils.LinesBreaksCount(line));
        }

        [Test]
        public void TestMoveLineUpAscending()
        {
            string value = "\n" +
                           "a\n" +
                           "cd\n" +
                           "fgh\n" +
                           "jklm";

            int index = 0;
            Assert.AreEqual(index, StringUtils.MoveLineUp(value, index));

            // second line
            index = value.IndexOf('a');
            Assert.AreEqual(0, StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('a') + 1;
            Assert.AreEqual(0, StringUtils.MoveLineUp(value, index));

            // third line
            index = value.IndexOf('c');
            Assert.AreEqual(value.IndexOf('a'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('d');
            Assert.AreEqual(value.IndexOf('a') + 1, StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('d') + 1;
            Assert.AreEqual(value.IndexOf('a') + 1, StringUtils.MoveLineUp(value, index));

            // fourth line
            index = value.IndexOf('f');
            Assert.AreEqual(value.IndexOf('c'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('g');
            Assert.AreEqual(value.IndexOf('d'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('h');
            Assert.AreEqual(value.IndexOf('d') + 1, StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('h') + 1;
            Assert.AreEqual(value.IndexOf('d') + 1, StringUtils.MoveLineUp(value, index));

            // fifth line
            index = value.IndexOf('j');
            Assert.AreEqual(value.IndexOf('f'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('k');
            Assert.AreEqual(value.IndexOf('g'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('l');
            Assert.AreEqual(value.IndexOf('h'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('m');
            Assert.AreEqual(value.IndexOf('h') + 1, StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('m') + 1;
            Assert.AreEqual(value.IndexOf('h') + 1, StringUtils.MoveLineUp(value, index));
        }

        [Test]
        public void TestMoveLineDownAscending()
        {
            string value = "\n" +
                           "a\n" +
                           "cd\n" +
                           "fgh\n" +
                           "jklm";

            // first line
            int index = 0;
            Assert.AreEqual(value.IndexOf('a'), StringUtils.MoveLineDown(value, index));

            // second line
            index = value.IndexOf('a');
            Assert.AreEqual(value.IndexOf('c'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('a') + 1;
            Assert.AreEqual(value.IndexOf('d'), StringUtils.MoveLineDown(value, index));

            // third line
            index = value.IndexOf('c');
            Assert.AreEqual(value.IndexOf('f'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('d');
            Assert.AreEqual(value.IndexOf('g'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('d') + 1;
            Assert.AreEqual(value.IndexOf('h'), StringUtils.MoveLineDown(value, index));

            // fourth line
            index = value.IndexOf('f');
            Assert.AreEqual(value.IndexOf('j'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('g');
            Assert.AreEqual(value.IndexOf('k'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('h');
            Assert.AreEqual(value.IndexOf('l'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('h') + 1;
            Assert.AreEqual(value.IndexOf('m'), StringUtils.MoveLineDown(value, index));

            // fifth line
            index = value.IndexOf('j');
            Assert.AreEqual(index, StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('k');
            Assert.AreEqual(index, StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('l');
            Assert.AreEqual(index, StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('m');
            Assert.AreEqual(index, StringUtils.MoveLineDown(value, index));
        }

        [Test]
        public void TestMoveLineUpDescending()
        {
            string value = "abcd\n" +
                           "fgh\n" +
                           "jk\n" +
                           "m\n" +
                           "\n" +
                           "";

            int index = value.IndexOf('a');

            // first line
            Assert.AreEqual(index, StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('b');
            Assert.AreEqual(index, StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('c');
            Assert.AreEqual(index, StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('d');
            Assert.AreEqual(index, StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('d') + 1;
            Assert.AreEqual(index, StringUtils.MoveLineUp(value, index));

            // second line
            index = value.IndexOf('f');
            Assert.AreEqual(value.IndexOf('a'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('g');
            Assert.AreEqual(value.IndexOf('b'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('h');
            Assert.AreEqual(value.IndexOf('c'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('h') + 1;
            Assert.AreEqual(value.IndexOf('d'), StringUtils.MoveLineUp(value, index));

            // third line
            index = value.IndexOf('j');
            Assert.AreEqual(value.IndexOf('f'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('k');
            Assert.AreEqual(value.IndexOf('g'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('k') + 1;
            Assert.AreEqual(value.IndexOf('h'), StringUtils.MoveLineUp(value, index));

            // fourth line
            index = value.IndexOf('m');
            Assert.AreEqual(value.IndexOf('j'), StringUtils.MoveLineUp(value, index));

            index = value.IndexOf('m') + 1;
            Assert.AreEqual(value.IndexOf('k'), StringUtils.MoveLineUp(value, index));

            // fifth line
            index = value.LastIndexOf('\n');
            Assert.AreEqual(value.IndexOf('m'), StringUtils.MoveLineUp(value, index));

            index = value.LastIndexOf('\n') + 1;
            Assert.AreEqual(value.LastIndexOf('\n'), StringUtils.MoveLineUp(value, index));
        }

        [Test]
        public void TestMoveLineDownDescending()
        {
            string value = "abcd\n" +
                           "fgh\n" +
                           "jk\n" +
                           "m\n" +
                           "\n" +
                           "";

            // first line
            int index = value.IndexOf('a');
            Assert.AreEqual(value.IndexOf('f'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('b');
            Assert.AreEqual(value.IndexOf('g'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('c');
            Assert.AreEqual(value.IndexOf('h'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('d');
            Assert.AreEqual(value.IndexOf('h') + 1, StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('d') + 1;
            Assert.AreEqual(value.IndexOf('h') + 1, StringUtils.MoveLineDown(value, index));

            // second line
            index = value.IndexOf('f');
            Assert.AreEqual(value.IndexOf('j'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('g');
            Assert.AreEqual(value.IndexOf('k'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('h');
            Assert.AreEqual(value.IndexOf('k') + 1, StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('h') + 1;
            Assert.AreEqual(value.IndexOf('k') + 1, StringUtils.MoveLineDown(value, index));

            // third line
            index = value.IndexOf('j');
            Assert.AreEqual(value.IndexOf('m'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('k');
            Assert.AreEqual(value.IndexOf('m') + 1, StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('k') + 1;
            Assert.AreEqual(value.IndexOf('m') + 1, StringUtils.MoveLineDown(value, index));

            // fourth line
            index = value.IndexOf('m');
            Assert.AreEqual(value.LastIndexOf('\n'), StringUtils.MoveLineDown(value, index));

            index = value.IndexOf('m') + 1;
            Assert.AreEqual(value.LastIndexOf('\n'), StringUtils.MoveLineDown(value, index));

            // fifth line
            index = value.LastIndexOf('\n');
            Assert.AreEqual(value.LastIndexOf('\n') + 1, StringUtils.MoveLineDown(value, index));

            index = value.LastIndexOf('\n') + 1;
            Assert.AreEqual(value.LastIndexOf('\n') + 1, StringUtils.MoveLineDown(value, index));
        }

        [Test]
        public void TestMoveLineUpEmpty()
        {
            string value = "\n" +
                           "\n" +
                           "";

            // first line
            Assert.AreEqual(0, StringUtils.MoveLineUp(value, 0));

            // second line
            Assert.AreEqual(0, StringUtils.MoveLineUp(value, 1));

            // third line
            Assert.AreEqual(1, StringUtils.MoveLineUp(value, 2));
        }

        [Test]
        public void TestMoveLineDownEmpty()
        {
            string value = "\n" +
                           "\n" +
                           "";

            // first line
            Assert.AreEqual(1, StringUtils.MoveLineDown(value, 0));

            // second line
            Assert.AreEqual(2, StringUtils.MoveLineDown(value, 1));

            // third line
            Assert.AreEqual(2, StringUtils.MoveLineDown(value, 2));
        }

        #endregion

        #region Text suggestions

        [Test()]
        public void TestNullTextSuggestions()
        {
            string[] values = 
            {
                "aa1", "aa11", "aa12", "aa13", "aa111", "aa112", "aa113", "b"
            };

            Assert.IsNull(StringUtils.GetSuggestedText(null, values));
        }

        [Test()]
        public void TestEmptyTextSuggestions()
        {
            string[] values = 
            {
                "aa1", "aa11", "aa12", "aa13", "aa111", "aa112", "aa113", "b"
            };
            
            Assert.IsNull(StringUtils.GetSuggestedText("", values));
        }

        [Test()]
        public void TestOnlyTextSuggestions()
        {
            string[] values = 
            {
                "aa1", "aa11", "aa12", "aa13", "aa111", "aa112", "aa113", "b"
            };
            
            Assert.AreEqual("b", StringUtils.GetSuggestedText("b", values));
        }

        [Test()]
        public void TestTextSuggestions()
        {
            string[] values = 
            {
                "aa1", "aa11", "aa12", "aa13", "aa111", "aa112", "aa113", "aa2", "b"
            };
            
            Assert.AreEqual("aa", StringUtils.GetSuggestedText("a", values));
        }

        [Test()]
        public void TestTextSuggestions2()
        {
            string[] values = 
            {
                "aa1", "aa11", "aa12", "aa13", "aa111", "aa112", "aa113", "aa2", "b"
            };

            Assert.AreEqual("aa", StringUtils.GetSuggestedText("aa", values));
        }

        [Test()]
        public void TestTextSuggestions3()
        {
            string[] values = 
            {
                "aa1", "aa11", "aa12", "aa13", "aa111", "aa112", "aa113", "aa2", "b"
            };

            Assert.AreEqual("aa1", StringUtils.GetSuggestedText("aa1", values));
        }

        [Test()]
        public void TestTextSuggestions4()
        {
            string[] values = 
            {
                "aa1", "aa11", "aa12", "aa13", "aa111", "aa112", "aa113", "aa2", "b"
            };

            Assert.AreEqual("aa11", StringUtils.GetSuggestedText("aa11", values));
        }

        [Test()]
        public void TestTextSuggestions5()
        {
            string[] values = 
            {
                "aa1", "aa11", "aa12", "aa13", "aa111", "aa112", "aa113", "aa2", "b"
            };

            Assert.AreEqual("aa111", StringUtils.GetSuggestedText("aa111", values));
        }

        [Test()]
        public void TestTextSuggestions6()
        {
            string[] values = 
            {
                "aa1", "aa11", "aa12", "aa13", "aa111", "aa112", "aa113", "aa2", "b"
            };

            Assert.IsNull(StringUtils.GetSuggestedText("aa1111", values));
        }

        #endregion

        #region Filter

        [Test]
        public void TestFilter()
        {
            string[] strings =
            {
                "Line1",
                "LINE12",
                "line2",
                "line3",
                "foo"
            };

            string[] expected =
            {
                "Line1",
                "LINE12"
            };

            Assert.AreEqual(expected, StringUtils.Filter(strings, "LINE1"));
        }

        [Test]
        public void TestFilterEmptyPrefix()
        {
            string[] strings =
            {
                "Line1",
                "LINE12",
                "line2",
                "line3",
                "foo"
            };

            string[] expected =
            {
                "Line1",
                "LINE12",
                "line2",
                "line3",
                "foo"
            };

            Assert.AreEqual(expected, StringUtils.Filter(strings, ""));
        }

        [Test]
        public void TestFilterNullPrefix()
        {
            string[] strings =
            {
                "Line1",
                "LINE12",
                "line2",
                "line3",
                "foo"
            };

            string[] expected =
            {
                "Line1",
                "LINE12",
                "line2",
                "line3",
                "foo"
            };

            Assert.AreEqual(expected, StringUtils.Filter(strings, null));
        }

        #endregion
    }
}