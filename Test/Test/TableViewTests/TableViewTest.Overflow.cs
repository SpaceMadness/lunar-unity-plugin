using System;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using UnityEngine;

using NUnit.Framework;

namespace TableViewTests
{
    using Assert = NUnit.Framework.Assert;

    public partial class TableViewTest
    {
        [Test()]
        public void TestOverflowNoScrollLock()
        {
            int capacity = 7;
            float cellHeight = 16;
            float tableHeight = 85.5f;

            TestCellCapacityAdapter adapter = new TestCellCapacityAdapter(capacity);

            TableViewMock table = new TableViewMock(capacity, 320, tableHeight);
            table.IsScrollLocked = false;
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            adapter.Add(new TableViewCell(320, cellHeight)); // 0
            table.ReloadNewData();

            AssertVisibleRows(table, 0);
            Assert.AreEqual(1, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 1
            table.ReloadNewData();

            AssertVisibleRows(table, 0, 1);
            Assert.AreEqual(2, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 2
            table.ReloadNewData();

            AssertVisibleRows(table, 0, 1, 2);
            Assert.AreEqual(3, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 3
            table.ReloadNewData();

            AssertVisibleRows(table, 0, 1, 2, 3);
            Assert.AreEqual(4, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 4
            table.ReloadNewData();

            AssertVisibleRows(table, 0, 1, 2, 3, 4);
            Assert.AreEqual(5, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 5
            table.ReloadNewData();

            AssertVisibleRows(table, 0, 1, 2, 3, 4, 5);
            Assert.AreEqual(6, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 6
            table.ReloadNewData();

            AssertVisibleRows(table, 0, 1, 2, 3, 4, 5);
            Assert.AreEqual(7, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 7
            table.ReloadNewData();

            AssertVisibleRows(table, 1, 2, 3, 4, 5, 6);
            Assert.AreEqual(8, table.RowsCount);
            Assert.AreEqual(cellHeight, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 8
            table.ReloadNewData();

            AssertVisibleRows(table, 2, 3, 4, 5, 6, 7);
            Assert.AreEqual(9, table.RowsCount);
            Assert.AreEqual(2 * cellHeight, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 9
            table.ReloadNewData();

            AssertVisibleRows(table, 3, 4, 5, 6, 7, 8);
            Assert.AreEqual(10, table.RowsCount);
            Assert.AreEqual(3 * cellHeight, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 10
            table.ReloadNewData();

            AssertVisibleRows(table, 4, 5, 6, 7, 8, 9);
            Assert.AreEqual(11, table.RowsCount);
            Assert.AreEqual(4 * cellHeight, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 11
            table.ReloadNewData();

            AssertVisibleRows(table, 5, 6, 7, 8, 9, 10);
            Assert.AreEqual(12, table.RowsCount);
            Assert.AreEqual(5 * cellHeight, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 12
            table.ReloadNewData();

            AssertVisibleRows(table, 6, 7, 8, 9, 10, 11);
            Assert.AreEqual(13, table.RowsCount);
            Assert.AreEqual(6 * cellHeight, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 13
            table.ReloadNewData();

            AssertVisibleRows(table, 7, 8, 9, 10, 11, 12);
            Assert.AreEqual(14, table.RowsCount);
            Assert.AreEqual(7 * cellHeight, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 14
            table.ReloadNewData();

            AssertVisibleRows(table, 8, 9, 10, 11, 12, 13);
            Assert.AreEqual(15, table.RowsCount);
            Assert.AreEqual(8 * cellHeight, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 15
            table.ReloadNewData();

            AssertVisibleRows(table, 9, 10, 11, 12, 13, 14);
            Assert.AreEqual(16, table.RowsCount);
            Assert.AreEqual(9 * cellHeight, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);
        }

        [Test()]
        public void TestOverflowScrollLock()
        {
            int capacity = 7;
            float cellHeight = 16;
            float tableHeight = 85.5f;

            TestCellCapacityAdapter adapter = new TestCellCapacityAdapter(capacity);

            TableViewMock table = new TableViewMock(capacity, 320, tableHeight);
            table.IsScrollLocked = true;
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            adapter.Add(new TableViewCell(320, cellHeight)); // 0
            table.ReloadNewData();

            AssertVisibleRows(table, 0);
            Assert.AreEqual(1, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 1
            table.ReloadNewData();

            AssertVisibleRows(table, 0, 1);
            Assert.AreEqual(2, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 2
            table.ReloadNewData();

            AssertVisibleRows(table, 0, 1, 2);
            Assert.AreEqual(3, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 3
            table.ReloadNewData();

            AssertVisibleRows(table, 0, 1, 2, 3);
            Assert.AreEqual(4, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 4
            table.ReloadNewData();

            AssertVisibleRows(table, 0, 1, 2, 3, 4);
            Assert.AreEqual(5, table.RowsCount);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(0, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 5
            table.ReloadNewData();

            AssertVisibleRows(table, 0, 1, 2, 3, 4, 5);
            Assert.AreEqual(6, table.RowsCount);
            Assert.AreEqual(10.5, table.ScrollPosTop);
            Assert.AreEqual(10.5, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 6
            table.ReloadNewData();

            AssertVisibleRows(table, 1, 2, 3, 4, 5, 6);
            Assert.AreEqual(7, table.RowsCount);
            Assert.AreEqual(26.5, table.ScrollPosTop);
            Assert.AreEqual(26.5, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 7
            table.ReloadNewData();

            AssertVisibleRows(table, 2, 3, 4, 5, 6, 7);
            Assert.AreEqual(8, table.RowsCount);
            Assert.AreEqual(42.5, table.ScrollPosTop);
            Assert.AreEqual(26.5, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 8
            table.ReloadNewData();

            AssertVisibleRows(table, 3, 4, 5, 6, 7, 8);
            Assert.AreEqual(9, table.RowsCount);
            Assert.AreEqual(58.5, table.ScrollPosTop);
            Assert.AreEqual(26.5, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 9
            table.ReloadNewData();

            AssertVisibleRows(table, 4, 5, 6, 7, 8, 9);
            Assert.AreEqual(10, table.RowsCount);
            Assert.AreEqual(74.5, table.ScrollPosTop);
            Assert.AreEqual(26.5, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 10
            table.ReloadNewData();

            AssertVisibleRows(table, 5, 6, 7, 8, 9, 10);
            Assert.AreEqual(11, table.RowsCount);
            Assert.AreEqual(90.5, table.ScrollPosTop);
            Assert.AreEqual(26.5, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 11
            table.ReloadNewData();

            AssertVisibleRows(table, 6, 7, 8, 9, 10, 11);
            Assert.AreEqual(12, table.RowsCount);
            Assert.AreEqual(106.5, table.ScrollPosTop);
            Assert.AreEqual(26.5, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 12
            table.ReloadNewData();

            AssertVisibleRows(table, 7, 8, 9, 10, 11, 12);
            Assert.AreEqual(13, table.RowsCount);
            Assert.AreEqual(122.5, table.ScrollPosTop);
            Assert.AreEqual(26.5, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 13
            table.ReloadNewData();

            AssertVisibleRows(table, 8, 9, 10, 11, 12, 13);
            Assert.AreEqual(14, table.RowsCount);
            Assert.AreEqual(138.5, table.ScrollPosTop);
            Assert.AreEqual(26.5, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 14
            table.ReloadNewData();

            AssertVisibleRows(table, 9, 10, 11, 12, 13, 14);
            Assert.AreEqual(15, table.RowsCount);
            Assert.AreEqual(154.5, table.ScrollPosTop);
            Assert.AreEqual(26.5, table.ScrollGUIPosTop);

            adapter.Add(new TableViewCell(320, cellHeight)); // 15
            table.ReloadNewData();

            AssertVisibleRows(table, 10, 11, 12, 13, 14, 15);
            Assert.AreEqual(16, table.RowsCount);
            Assert.AreEqual(170.5, table.ScrollPosTop);
            Assert.AreEqual(26.5, table.ScrollGUIPosTop);
        }
    }
}