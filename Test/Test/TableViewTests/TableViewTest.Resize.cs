using System;
using LunarPlugin;
using UnityEngine;

using NUnit.Framework;

namespace TableViewTests
{
    using Assert = NUnit.Framework.Assert;

    public partial class TableViewTest
    {
        [Test]
        [Ignore("Text wrapping had changed: need to review this test")]
        public void TestResizeChangeWidth()
        {
            MockCellEntry[] entries = new MockCellEntry[]
            {
                new MockCellEntry(typeof(TableViewCellMock), 10),
                new MockCellEntry(typeof(TableViewCellMock), 15), 
                new MockCellEntry(typeof(TableViewCellMock), 10),
                new MockCellEntry(typeof(TableViewCellMock), 15),
                new MockCellEntry(typeof(TableViewCellMock), 10),
                new MockCellEntry(typeof(TableViewCellMock), 15)
            };

            TableViewAdapter adapter = new TestCellsHeightTableAdapter(entries);

            TableViewMock table = new TableViewMock(320, 60);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            // descrease width
            for (int i = 0; i < entries.Length; ++i)
            {
                if (entries[i].height == 10)
                {
                    entries[i].height = 20;
                }
            }
            table.Resize(300, 60);

            AssertVisibleRows(table, 1, 2, 3, 4);
            Assert.AreEqual(30, table.ScrollPosTop);
            Assert.AreEqual(90, table.ScrollPosBottom);

            // increase width
            for (int i = 0; i < entries.Length; ++i)
            {
                if (entries[i].height == 20)
                {
                    entries[i].height = 10;
                }
            }
            table.Resize(320, 60);

            AssertVisibleRows(table, 0, 1, 2, 3, 4);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(60, table.ScrollPosBottom);
        }

        [Test]
        public void TestResizeChangeHeight()
        {
            TableViewAdapter adapter = new TestCellsHeightTableAdapter(new MockCellEntry[] {
                new MockCellEntry(typeof(TableViewCellMock), 10),
                new MockCellEntry(typeof(TableViewCellMock), 15), 
                new MockCellEntry(typeof(TableViewCellMock), 10),
                new MockCellEntry(typeof(TableViewCellMock), 15),
                new MockCellEntry(typeof(TableViewCellMock), 10),
                new MockCellEntry(typeof(TableViewCellMock), 15)
            });

            TableViewMock table = new TableViewMock(320, 30);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            Assert.AreEqual(0, table.FirstVisibleCellIndex);
            Assert.AreEqual(2, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 0, 1, 2);

            table.Resize(320, 35);

            AssertVisibleRows(table, 0, 1, 2);

            table.Resize(320, 45);

            AssertVisibleRows(table, 0, 1, 2, 3);

            table.Resize(320, 55);

            AssertVisibleRows(table, 0, 1, 2, 3, 4);

            table.Resize(320, 65);

            AssertVisibleRows(table, 0, 1, 2, 3, 4, 5);

            table.Resize(320, 75);

            AssertVisibleRows(table, 0, 1, 2, 3, 4, 5);

            table.Resize(320, 80);

            AssertVisibleRows(table, 0, 1, 2, 3, 4, 5);

            table.Resize(320, 85);

            AssertVisibleRows(table, 0, 1, 2, 3, 4, 5);

            table.Resize(320, 80);

            AssertVisibleRows(table, 0, 1, 2, 3, 4, 5);

            table.Resize(320, 75);

            AssertVisibleRows(table, 0, 1, 2, 3, 4, 5);

            table.Resize(320, 65);

            AssertVisibleRows(table, 0, 1, 2, 3, 4, 5);

            table.Resize(320, 55);

            AssertVisibleRows(table, 0, 1, 2, 3, 4);

            table.Resize(320, 45);

            AssertVisibleRows(table, 0, 1, 2, 3);

            table.Resize(320, 35);

            AssertVisibleRows(table, 0, 1, 2);

            table.Resize(320, 30);

            AssertVisibleRows(table, 0, 1, 2);

            table.Resize(320, 25);

            AssertVisibleRows(table, 0, 1);

            table.Resize(320, 20);

            AssertVisibleRows(table, 0, 1);

            table.Resize(320, 10);

            AssertVisibleRows(table, 0);

            table.Resize(320, 5);

            AssertVisibleRows(table, 0);

            table.Resize(320, 0);

            AssertVisibleRows(table, new int[0]);
        }
    }
}