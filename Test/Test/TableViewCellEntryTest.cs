using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Text;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using UnityEngine;

namespace TableViewTest
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture()]
    public class TableViewCellEntryTest : ITextMeasure
    {
        private static readonly string kSpace = "  ";

        #region Table layout

        [Test]
        [Ignore("Text wrapping had changed: need to review this test")]
        public void TestTableLayoutSingleItem()
        {
            Vector2 size = new Vector2(14, 1);

            string[] lines = { "1111" };

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines);
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111"
            );
        }

        [Test()]
        public void TestTableLayoutTwoItemsSingleRow()
        {
            Vector2 size = new Vector2(14, 1);

            string[] lines = { "111", "2222" };

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines);
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "111 " + kSpace + "2222"
            );
        }

        [Test()]
        public void TestTableLayoutThreeItemsSingleRow()
        {
            Vector2 size = new Vector2(18, 1);

            string[] lines = { "111", "22", "3333" };

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines);
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "111 " + kSpace + "22  " + kSpace + "3333"
            );
        }

        [Test()]
        public void TestTableLayoutFourItemsTwoRows()
        {
            Vector2 size = new Vector2(14, 2);

            string[] lines = { "1111", "2222", "3333", "4444" };

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines);
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111" + kSpace + "3333" + "\n" +
                "2222" + kSpace + "4444"
            );
        }

        [Test()]
        public void TestTableLayoutFiveItemsTwoRows()
        {
            Vector2 size = new Vector2(18, 2);

            List<string> lines = new List<string>();
            lines.Add("1111");
            lines.Add("2222");
            lines.Add("3333");
            lines.Add("4444");
            lines.Add("5555");

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines.ToArray());
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111" + kSpace + "3333" + kSpace + "5555" + "\n" +
                "2222" + kSpace + "4444"
            );
        }

        [Test()]
        public void TestTableLayoutSixItemsTwoRows()
        {
            Vector2 size = new Vector2(18, 2);

            List<string> lines = new List<string>();
            lines.Add("1111");
            lines.Add("2222");
            lines.Add("3333");
            lines.Add("4444");
            lines.Add("5555");
            lines.Add("6666");

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines.ToArray());
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111" + kSpace + "3333" + kSpace + "5555" + "\n" +
                "2222" + kSpace + "4444" + kSpace + "6666" 
            );
        }

        [Test()]
        public void TestTableLayoutSevenItemsTwoRows()
        {
            Vector2 size = new Vector2(18, 3);

            List<string> lines = new List<string>();
            lines.Add("1111");
            lines.Add("2222");
            lines.Add("3333");
            lines.Add("4444");
            lines.Add("5555");
            lines.Add("6666");
            lines.Add("7777");

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines.ToArray());
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111" + kSpace + "4444" + kSpace + "7777" + "\n" +
                "2222" + kSpace + "5555" + "\n" +
                "3333" + kSpace + "6666" 
            );
        }

        [Test()]
        public void TestTableLayoutEightItemsTwoRows()
        {
            Vector2 size = new Vector2(18, 3);

            List<string> lines = new List<string>();
            lines.Add("1111");
            lines.Add("2222");
            lines.Add("3333");
            lines.Add("4444");
            lines.Add("5555");
            lines.Add("6666");
            lines.Add("7777");
            lines.Add("8888");

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines.ToArray());
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111" + kSpace + "4444" + kSpace + "7777" + "\n" +
                "2222" + kSpace + "5555" + kSpace + "8888" + "\n" +
                "3333" + kSpace + "6666" 
            );
        }

        [Test()]
        public void TestTableLayoutNineItemsTwoRows()
        {
            Vector2 size = new Vector2(18, 3);

            List<string> lines = new List<string>();
            lines.Add("1111");
            lines.Add("2222");
            lines.Add("3333");
            lines.Add("4444");
            lines.Add("5555");
            lines.Add("6666");
            lines.Add("7777");
            lines.Add("8888");
            lines.Add("9999");

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines.ToArray());
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111" + kSpace + "4444" + kSpace + "7777" + "\n" +
                "2222" + kSpace + "5555" + kSpace + "8888" + "\n" +
                "3333" + kSpace + "6666" + kSpace + "9999" 
            );
        }

        [Test]
        [Ignore("Text wrapping had changed: need to review this test")]
        public void TestTableLayoutSingleLongItemExactFit()
        {
            Vector2 size = new Vector2(4, 1);

            List<string> lines = new List<string>();
            lines.Add("1111");

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines.ToArray());
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111"
            );
        }

        [Test]
        [Ignore("Text wrapping had changed: need to review this test")]
        public void TestTableLayoutSingleLongItemDontFit()
        {
            Vector2 size = new Vector2(3, 1);

            List<string> lines = new List<string>();
            lines.Add("1111");

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines.ToArray());
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111"
            );
        }

        [Test()]
        public void TestTableLayoutTwoItemsDifferentSizeSingleRow()
        {
            Vector2 size = new Vector2(18, 1);

            List<string> lines = new List<string>();
            lines.Add("1111");
            lines.Add("222222");

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines.ToArray());
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111  " + kSpace + "222222"
            );
        }

        [Test()]
        public void TestTableLayoutTwoItemsDifferentSizeTwoRows()
        {
            Vector2 size = new Vector2(18, 2);

            List<string> lines = new List<string>();
            lines.Add("1111");
            lines.Add("222222");
            lines.Add("33333");

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines.ToArray());
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111  " + kSpace + "33333" + "\n" +
                "222222"
            );
        }

        [Test()]
        public void TestTableLayoutTwoItemsDifferentSizeThreeRows()
        {
            Vector2 size = new Vector2(14, 3);

            List<string> lines = new List<string>();
            lines.Add("1111");
            lines.Add("222222");
            lines.Add("333333333");

            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(lines.ToArray());
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "1111"   + "\n" +
                "222222" + "\n" +
                "333333333"
            );
        }

        #endregion

        #region Flow layout

        [Test]
        [Ignore("Text wrapping had changed: need to review this test")]
        public void TestFlowLayoutSinglelWordLessThanMaxWidth()
        {
            Vector2 size = new Vector2(5, 1);

            string line = "12345";
            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(line);
            entry.Layout(this, size.x + 1);

            AssertCellEntryLayout(entry, size,
                "12345"
            );
        }

        [Test]
        [Ignore("Text wrapping had changed: need to review this test")]
        public void TestFlowLayoutSinglelWordEqualToMaxWidth()
        {
            Vector2 size = new Vector2(5, 1);

            string line = "12345";
            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(line);
            entry.Layout(this, size.x);

            AssertCellEntryLayout(entry, size,
                "12345"
            );
        }

        [Test]
        [Ignore("Text wrapping had changed: need to review this test")]
        public void TestFlowLayoutSinglelWordMoreThanMaxWidth()
        {
            Vector2 size = new Vector2(5, 1);

            string line = "12345";
            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(line);
            entry.Layout(this, size.x - 1);

            AssertCellEntryLayout(entry, size,
                "12345"
            );
        }
       
        #endregion

        #region Helpers

        private void AssertCellEntryLayout(ConsoleViewCellEntry entry, Vector2 expectedSize, string expectedValue)
        {
            Assert.AreEqual(expectedValue, entry.value);
            Assert.AreEqual(expectedSize.x, entry.width);
            Assert.AreEqual(expectedSize.y, entry.height);
        }

        #endregion

        #region ITextMeasure implementation

        public Vector2 CalcSize(string text)
        {
            float width = 0;
            float height = 1;

            float lineWidth = 0;
            for (int i = 0; i < text.Length; ++i)
            {
                char chr = text[i];
                if (chr == '\n')
                {
                    width = Mathf.Max(width, lineWidth);
                    lineWidth = 0;
                    height += 1;
                } 
                else
                {
                    lineWidth += 1;
                }
            }

            width = Mathf.Max(width, lineWidth);
            return new Vector2(width, height);
        }

        public float CalcHeight(string text, float width)
        {
            throw new NotImplementedException();
        }

        public float LineHeight
        {
            get { return 1; }
        }

        #endregion
    }
}

