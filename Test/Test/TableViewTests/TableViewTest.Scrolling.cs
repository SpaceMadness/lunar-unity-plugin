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
        [Test]
        public void TestItemsScroll()
        {
            TestCellPredefinedAdapter adapter = new TestCellPredefinedAdapter(new TableViewCell[0]);
            adapter.Add(new TableViewCellMock(320, 10));
            adapter.Add(new TableViewCellMock(320, 15));
            adapter.Add(new TableViewCellMock(320, 10));
            adapter.Add(new TableViewCellMock(320, 15));
            adapter.Add(new TableViewCellMock(320, 10));
            adapter.Add(new TableViewCellMock(320, 15));

            TableViewMock table = new TableViewMock(320, 30);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            // scroll forward

            table.ScrollUntilRowVisible(0);
            AssertVisibleRows(table, 0, 1, 2);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(30, table.ScrollPosBottom);

            table.ScrollUntilRowVisible(1);
            AssertVisibleRows(table, 0, 1, 2);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(30, table.ScrollPosBottom);

            table.ScrollUntilRowVisible(2);
            AssertVisibleRows(table, 0, 1, 2);
            Assert.AreEqual(5, table.ScrollPosTop);
            Assert.AreEqual(35, table.ScrollPosBottom);

            table.ScrollUntilRowVisible(3);
            AssertVisibleRows(table, 1, 2, 3);
            Assert.AreEqual(20, table.ScrollPosTop);
            Assert.AreEqual(50, table.ScrollPosBottom);

            table.ScrollUntilRowVisible(4);
            AssertVisibleRows(table, 2, 3, 4);
            Assert.AreEqual(30, table.ScrollPosTop);
            Assert.AreEqual(60, table.ScrollPosBottom);

            table.ScrollUntilRowVisible(5);
            AssertVisibleRows(table, 3, 4, 5);
            Assert.AreEqual(45, table.ScrollPosTop);
            Assert.AreEqual(75, table.ScrollPosBottom);

            // scroll back

            table.ScrollUntilRowVisible(5);
            AssertVisibleRows(table, 3, 4, 5);
            Assert.AreEqual(45, table.ScrollPosTop);
            Assert.AreEqual(75, table.ScrollPosBottom);

            table.ScrollUntilRowVisible(4);
            AssertVisibleRows(table, 3, 4, 5);
            Assert.AreEqual(45, table.ScrollPosTop);
            Assert.AreEqual(75, table.ScrollPosBottom);

            table.ScrollUntilRowVisible(3);
            AssertVisibleRows(table, 3, 4, 5);
            Assert.AreEqual(35, table.ScrollPosTop);
            Assert.AreEqual(65, table.ScrollPosBottom);

            table.ScrollUntilRowVisible(2);
            AssertVisibleRows(table, 2, 3, 4);
            Assert.AreEqual(25, table.ScrollPosTop);
            Assert.AreEqual(55, table.ScrollPosBottom);

            table.ScrollUntilRowVisible(1);
            AssertVisibleRows(table, 1, 2, 3);
            Assert.AreEqual(10, table.ScrollPosTop);
            Assert.AreEqual(40, table.ScrollPosBottom);

            table.ScrollUntilRowVisible(0);
            AssertVisibleRows(table, 0, 1, 2);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(30, table.ScrollPosBottom);
        }

        [Test]
        public void TestLoadItemsScrollLock()
        {
            TestCellPredefinedAdapter adapter = new TestCellPredefinedAdapter(new TableViewCell[0]);
            adapter.Add(new TableViewCellMock(320, 10));
            adapter.Add(new TableViewCellMock(320, 15));
            adapter.Add(new TableViewCellMock(320, 10));
            adapter.Add(new TableViewCellMock(320, 15));
            adapter.Add(new TableViewCellMock(320, 10));
            adapter.Add(new TableViewCellMock(320, 15));

            TableViewMock table = new TableViewMock(320, 30);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.IsScrollLocked = true;
            table.ReloadData();

            AssertVisibleRows(table, 3, 4, 5);
            Assert.AreEqual(45, table.ScrollPosTop);
            Assert.AreEqual(75, table.ScrollPosBottom);
        }

        [Test]
        public void TestAddItemsScrollLock()
        {
            TestCellPredefinedAdapter adapter = new TestCellPredefinedAdapter(new TableViewCell[0]);

            TableViewMock table = new TableViewMock(320, 30);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.IsScrollLocked = true;
            table.ReloadData();

            adapter.Add(new TableViewCellMock(table.Width, 10));
            table.ReloadNewData();
            AssertVisibleRows(table, 0);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(30, table.ScrollPosBottom);

            adapter.Add(new TableViewCellMock(table.Width, 15));
            table.ReloadNewData();
            AssertVisibleRows(table, 0, 1);
            Assert.AreEqual(0, table.ScrollPosTop);
            Assert.AreEqual(30, table.ScrollPosBottom);

            adapter.Add(new TableViewCellMock(table.Width, 10));
            table.ReloadNewData();
            AssertVisibleRows(table, 0, 1, 2);
            Assert.AreEqual(5, table.ScrollPosTop);
            Assert.AreEqual(35, table.ScrollPosBottom);

            adapter.Add(new TableViewCellMock(table.Width, 15));
            table.ReloadNewData();
            AssertVisibleRows(table, 1, 2, 3);
            Assert.AreEqual(20, table.ScrollPosTop);
            Assert.AreEqual(50, table.ScrollPosBottom);

            adapter.Add(new TableViewCellMock(table.Width, 10));
            table.ReloadNewData();
            AssertVisibleRows(table, 2, 3, 4);
            Assert.AreEqual(30, table.ScrollPosTop);
            Assert.AreEqual(60, table.ScrollPosBottom);

            adapter.Add(new TableViewCellMock(table.Width, 15));
            table.ReloadNewData();
            AssertVisibleRows(table, 3, 4, 5);
            Assert.AreEqual(45, table.ScrollPosTop);
            Assert.AreEqual(75, table.ScrollPosBottom);
        }
    }
}

