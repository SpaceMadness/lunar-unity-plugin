using System;
using UnityEngine;

using NUnit.Framework;

namespace TableViewTests
{
    using Assert = NUnit.Framework.Assert;

    public partial class TableViewTest
    {
        [Test()]
        public void TestVisibleRows()
        {
            TableViewAdapter adapter = new TestCellsHeightTableAdapter(new MockCellEntry[] { 
                new MockCellEntry(typeof(TableViewCellMock), 10)
            });

            TableViewMock table = new TableViewMock(320, 15);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            Assert.AreEqual(0, table.FirstVisibleCellIndex);
            Assert.AreEqual(0, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 0);
        }

        [Test()]
        public void TestLastVisibleRowTwoElements()
        {
            TableViewAdapter adapter = new TestCellsHeightTableAdapter(new MockCellEntry[] { 
                new MockCellEntry(typeof(TableViewCellMock), 10),
                new MockCellEntry(typeof(TableViewCellMock), 15)
            });

            TableViewMock table = new TableViewMock(320, 15);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            Assert.AreEqual(0, table.FirstVisibleCellIndex);
            Assert.AreEqual(1, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 0, 1);
        }

        [Test()]
        public void TestLastVisibleRowThreeElements()
        {
            TableViewAdapter adapter = new TestCellsHeightTableAdapter(new MockCellEntry[] {
                new MockCellEntry(typeof(TableViewCellMock), 10),
                new MockCellEntry(typeof(TableViewCellMock), 15),
                new MockCellEntry(typeof(TableViewCellMock), 10)
            });

            TableViewMock table = new TableViewMock(320, 15);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            Assert.AreEqual(0, table.FirstVisibleCellIndex);
            Assert.AreEqual(1, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 0, 1);
        }

        [Test()]
        public void TestLastVisibleRowFourElements()
        {
            TableViewAdapter adapter = new TestCellsHeightTableAdapter(new MockCellEntry[] {
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
        }

        [Test()]
        public void TestVisibleRowsScroll()
        {
            MockCellEntry[] cells = new MockCellEntry[10];
            for (int i = 0; i < cells.Length; ++i)
            {
                cells[i] = new MockCellEntry(typeof(TableViewCellMock), i % 2 == 0 ? 10 : 15);
            }

            TableViewAdapter adapter = new TestCellsHeightTableAdapter(cells);

            TableViewMock table = new TableViewMock(320, 15);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            Assert.AreEqual(0, table.FirstVisibleCellIndex);
            Assert.AreEqual(1, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 0, 1);

            table.Scroll(10);
            Assert.AreEqual(1, table.FirstVisibleCellIndex);
            Assert.AreEqual(1, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 1);

            table.Scroll(10);
            Assert.AreEqual(1, table.FirstVisibleCellIndex);
            Assert.AreEqual(2, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 1, 2);

            table.Scroll(10);
            Assert.AreEqual(2, table.FirstVisibleCellIndex);
            Assert.AreEqual(3, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 2, 3);

            table.Scroll(10);
            Assert.AreEqual(3, table.FirstVisibleCellIndex);
            Assert.AreEqual(4, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 3, 4);

            table.Scroll(10);
            Assert.AreEqual(4, table.FirstVisibleCellIndex);
            Assert.AreEqual(5, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 4, 5);

            table.Scroll(10);
            Assert.AreEqual(5, table.FirstVisibleCellIndex);
            Assert.AreEqual(5, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 5);

            table.Scroll(10);
            Assert.AreEqual(5, table.FirstVisibleCellIndex);
            Assert.AreEqual(6, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 5, 6);

            table.Scroll(10);
            Assert.AreEqual(6, table.FirstVisibleCellIndex);
            Assert.AreEqual(7, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 6, 7);

            table.Scroll(10);
            Assert.AreEqual(7, table.FirstVisibleCellIndex);
            Assert.AreEqual(8, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 7, 8);

            table.Scroll(10);
            Assert.AreEqual(8, table.FirstVisibleCellIndex);
            Assert.AreEqual(9, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 8, 9);

            table.Scroll(10);
            Assert.AreEqual(9, table.FirstVisibleCellIndex);
            Assert.AreEqual(9, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 9);

            table.Scroll(-10);
            Assert.AreEqual(8, table.FirstVisibleCellIndex);
            Assert.AreEqual(9, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 8, 9);

            table.Scroll(-10);
            Assert.AreEqual(7, table.FirstVisibleCellIndex);
            Assert.AreEqual(8, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 7, 8);

            table.Scroll(-10);
            Assert.AreEqual(6, table.FirstVisibleCellIndex);
            Assert.AreEqual(7, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 6, 7);

            table.Scroll(-10);
            Assert.AreEqual(5, table.FirstVisibleCellIndex);
            Assert.AreEqual(6, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 5, 6);

            table.Scroll(-10);
            Assert.AreEqual(5, table.FirstVisibleCellIndex);
            Assert.AreEqual(5, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 5);

            table.Scroll(-10);
            Assert.AreEqual(4, table.FirstVisibleCellIndex);
            Assert.AreEqual(5, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 4, 5);

            table.Scroll(-10);
            Assert.AreEqual(3, table.FirstVisibleCellIndex);
            Assert.AreEqual(4, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 3, 4);

            table.Scroll(-10);
            Assert.AreEqual(2, table.FirstVisibleCellIndex);
            Assert.AreEqual(3, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 2, 3);

            table.Scroll(-10);
            Assert.AreEqual(1, table.FirstVisibleCellIndex);
            Assert.AreEqual(2, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 1, 2);

            table.Scroll(-10);
            Assert.AreEqual(1, table.FirstVisibleCellIndex);
            Assert.AreEqual(1, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 1);

            table.Scroll(-10);
            Assert.AreEqual(0, table.FirstVisibleCellIndex);
            Assert.AreEqual(1, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 0, 1);
        }

        [Test()]
        public void TestVisibleRowsBigScroll()
        {
            MockCellEntry[] cells = new MockCellEntry[10];
            for (int i = 0; i < cells.Length; ++i)
            {
                cells[i] = new MockCellEntry(typeof(TableViewCellMock), 10);
            }

            TableViewAdapter adapter = new TestCellsHeightTableAdapter(cells);

            TableViewMock table = new TableViewMock(320, 50);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            table.Scroll(50);
            Assert.AreEqual(5, table.FirstVisibleCellIndex);
            Assert.AreEqual(9, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 5, 6, 7, 8, 9);

            table.Scroll(-50);
            Assert.AreEqual(0, table.FirstVisibleCellIndex);
            Assert.AreEqual(4, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 0, 1, 2, 3, 4);
        }

        [Test()]
        public void TestVisibleRowsEvenBiggerScroll()
        {
            MockCellEntry[] cells = new MockCellEntry[15];
            for (int i = 0; i < cells.Length; ++i)
            {
                cells[i] = new MockCellEntry(typeof(TableViewCellMock), 10);
            }

            TableViewAdapter adapter = new TestCellsHeightTableAdapter(cells);

            TableViewMock table = new TableViewMock(320, 50);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            table.Scroll(100);
            Assert.AreEqual(10, table.FirstVisibleCellIndex);
            Assert.AreEqual(14, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 10, 11, 12, 13, 14);

            table.Scroll(-100);
            Assert.AreEqual(0, table.FirstVisibleCellIndex);
            Assert.AreEqual(4, table.LastVisibleCellIndex);
            AssertVisibleRows(table, 0, 1, 2, 3, 4);
        }
    }
}

