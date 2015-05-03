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
        public void TestAddTwoRows()
        {
            TableViewMock table = new TableViewMock(320, 15);
            TableViewCell a1 = new TableViewCellMock1(table.Width, 10);
            TableViewCell a2 = new TableViewCellMock2(table.Width, 15);

            TestCellPredefinedAdapter adapter = new TestCellPredefinedAdapter(new TableViewCell[] { a1 });

            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            adapter.Add(a2);
            table.ReloadNewData();

            TableViewCell b1 = table.FirstVisibleCell;
            TableViewCell b2 = b1.NextCell;

            Assert.AreSame(a1, b1);
            Assert.AreSame(a2, b2);
            AssertVisibleRows(table, 0, 1);
        }

        [Test()]
        public void TestAddMultipleRows()
        {
            TableViewMock table = new TableViewMock(320, 15);
            TableViewCell a1 = new TableViewCellMock1(table.Width, 10);
            TableViewCell a2 = new TableViewCellMock2(table.Width, 15);
            TableViewCell a3 = new TableViewCellMock3(table.Width, 10);

            TestCellPredefinedAdapter adapter = new TestCellPredefinedAdapter(new TableViewCell[] { a1 });

            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            adapter.Add(a2);
            adapter.Add(a3);
            table.ReloadNewData();

            AssertVisibleRows(table, a1, a2);
        }

        [Test()]
        public void TestAddMultipleRowsAndScroll()
        {
            TableViewMock table = new TableViewMock(320, 15);
            TableViewCell c1 = new TableViewCellMock1(table.Width, 10);
            TableViewCell c2 = new TableViewCellMock2(table.Width, 15);
            TableViewCell c3 = new TableViewCellMock1(table.Width, 10);
            TableViewCell c4 = new TableViewCellMock2(table.Width, 15);
            TableViewCell c5 = new TableViewCellMock1(table.Width, 10);
            TableViewCell c6 = new TableViewCellMock2(table.Width, 15);

            TestCellPredefinedAdapter adapter = new TestCellPredefinedAdapter(new TableViewCell[0]);
            table.DataSource = adapter;
            table.Delegate = adapter;

            adapter.Add(c1, c2, c3, c4, c5, c6);

            table.ReloadNewData();
            AssertVisibleRows(table, c1, c2);

            table.Scroll(15);
            AssertVisibleRows(table, c2, c3);

            table.Scroll(10);
            AssertVisibleRows(table, c3, c4);

            table.Scroll(15);
            AssertVisibleRows(table, c4, c5);

            table.Scroll(10);
            AssertVisibleRows(table, c5, c6);

            table.Scroll(15);
            AssertVisibleRows(table, c6);

            table.Scroll(-15);
            AssertVisibleRows(table, c5, c6);

            table.Scroll(-10);
            AssertVisibleRows(table, c4, c5);

            table.Scroll(-15);
            AssertVisibleRows(table, c3, c4);

            table.Scroll(-10);
            AssertVisibleRows(table, c2, c3);

            table.Scroll(-15);
            AssertVisibleRows(table, c1, c2);
        }
    }
}

