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
        public void TestReusableCells()
        {
            MockCellEntry[] cells =
            {
                new MockCellEntry(typeof(TableViewCellMock1), 10),
                new MockCellEntry(typeof(TableViewCellMock2), 10),
                new MockCellEntry(typeof(TableViewCellMock3), 10),
                new MockCellEntry(typeof(TableViewCellMock1), 10),
                new MockCellEntry(typeof(TableViewCellMock2), 10),
                new MockCellEntry(typeof(TableViewCellMock3), 10),
            };

            TableViewAdapter adapter = new TestCellsHeightTableAdapter(cells);

            TableViewMock table = new TableViewMock(320, 25);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            TableViewCell a1 = table.FirstVisibleCell;
            TableViewCell a2 = a1.NextCell;
            TableViewCell a3 = a2.NextCell;

            Assert.AreEqual(typeof(TableViewCellMock1), a1.GetType());
            Assert.AreEqual(typeof(TableViewCellMock2), a2.GetType());
            Assert.AreEqual(typeof(TableViewCellMock3), a3.GetType());

            table.Scroll(10);

            TableViewCell b1 = table.FirstVisibleCell;
            TableViewCell b2 = b1.NextCell;
            TableViewCell b3 = b2.NextCell;

            Assert.AreSame(a2, b1);
            Assert.AreSame(a3, b2);
            Assert.AreSame(a1, b3);

            table.Scroll(10);

            b1 = table.FirstVisibleCell;
            b2 = b1.NextCell;
            b3 = b2.NextCell;

            Assert.AreSame(a3, b1);
            Assert.AreSame(a1, b2);
            Assert.AreSame(a2, b3);

            table.Scroll(10);

            b1 = table.FirstVisibleCell;
            b2 = b1.NextCell;
            b3 = b2.NextCell;

            Assert.AreSame(a1, b1);
            Assert.AreSame(a2, b2);
            Assert.AreSame(a3, b3);

            table.Scroll(5);

            b1 = table.FirstVisibleCell;
            b2 = b1.NextCell;
            b3 = b2.NextCell;

            Assert.AreSame(a1, b1);
            Assert.AreSame(a2, b2);
            Assert.AreSame(a3, b3);

            table.Scroll(-5);

            b1 = table.FirstVisibleCell;
            b2 = b1.NextCell;
            b3 = b2.NextCell;

            Assert.AreSame(a1, b1);
            Assert.AreSame(a2, b2);
            Assert.AreSame(a3, b3);

            table.Scroll(-10);

            b1 = table.FirstVisibleCell;
            b2 = b1.NextCell;
            b3 = b2.NextCell;

            Assert.AreSame(a3, b1);
            Assert.AreSame(a1, b2);
            Assert.AreSame(a2, b3);

            table.Scroll(-10);

            b1 = table.FirstVisibleCell;
            b2 = b1.NextCell;
            b3 = b2.NextCell;

            Assert.AreSame(a2, b1);
            Assert.AreSame(a3, b2);
            Assert.AreSame(a1, b3);

            table.Scroll(-10);

            b1 = table.FirstVisibleCell;
            b2 = b1.NextCell;
            b3 = b2.NextCell;

            Assert.AreSame(a1, b1);
            Assert.AreSame(a2, b2);
            Assert.AreSame(a3, b3);

            Assert.AreEqual(1, TableViewCellMock1.instanceCount);
            Assert.AreEqual(1, TableViewCellMock2.instanceCount);
            Assert.AreEqual(1, TableViewCellMock3.instanceCount);
        }

        [Test()]
        public void TestReusableCellsMultipleOccurrence()
        {
            MockCellEntry[] cells = new MockCellEntry[10];
            for (int i = 0; i < cells.Length; ++i)
            {
                cells[i] = new MockCellEntry(typeof(TableViewCellMock1), 10);
            };

            TableViewAdapter adapter = new TestCellsHeightTableAdapter(cells);

            TableViewMock table = new TableViewMock(320, 50);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            TableViewCell a1 = table.FirstVisibleCell;
            TableViewCell a2 = a1.NextCell;
            TableViewCell a3 = a2.NextCell;
            TableViewCell a4 = a3.NextCell;
            TableViewCell a5 = a4.NextCell;

            table.Scroll(50);

            TableViewCell b1 = table.FirstVisibleCell;
            TableViewCell b2 = b1.NextCell;
            TableViewCell b3 = b2.NextCell;
            TableViewCell b4 = b3.NextCell;
            TableViewCell b5 = b4.NextCell;

            Assert.AreSame(a1, b5);
            Assert.AreSame(a2, b4);
            Assert.AreSame(a3, b3);
            Assert.AreSame(a4, b2);
            Assert.AreSame(a5, b1);

            table.Scroll(-50);

            b1 = table.FirstVisibleCell;
            b2 = b1.NextCell;
            b3 = b2.NextCell;
            b4 = b3.NextCell;
            b5 = b4.NextCell;

            Assert.AreSame(a1, b1);
            Assert.AreSame(a2, b2);
            Assert.AreSame(a3, b3);
            Assert.AreSame(a4, b4);
            Assert.AreSame(a5, b5);

            Assert.AreEqual(5, TableViewCellMock1.instanceCount);
        }

        [Test()]
        public void TestReusableCellsMultipleOccurrenceBigScroll()
        {
            MockCellEntry[] cells = new MockCellEntry[15];
            for (int i = 0; i < cells.Length; ++i)
            {
                cells[i] = new MockCellEntry(typeof(TableViewCellMock1), 10);
            };

            TableViewAdapter adapter = new TestCellsHeightTableAdapter(cells);

            TableViewMock table = new TableViewMock(320, 50);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            TableViewCell a1 = table.FirstVisibleCell;
            TableViewCell a2 = a1.NextCell;
            TableViewCell a3 = a2.NextCell;
            TableViewCell a4 = a3.NextCell;
            TableViewCell a5 = a4.NextCell;

            table.Scroll(100);

            TableViewCell b1 = table.FirstVisibleCell;
            TableViewCell b2 = b1.NextCell;
            TableViewCell b3 = b2.NextCell;
            TableViewCell b4 = b3.NextCell;
            TableViewCell b5 = b4.NextCell;

            Assert.AreSame(a1, b5);
            Assert.AreSame(a2, b4);
            Assert.AreSame(a3, b3);
            Assert.AreSame(a4, b2);
            Assert.AreSame(a5, b1);

            table.Scroll(-100);

            b1 = table.FirstVisibleCell;
            b2 = b1.NextCell;
            b3 = b2.NextCell;
            b4 = b3.NextCell;
            b5 = b4.NextCell;

            Assert.AreSame(a1, b1);
            Assert.AreSame(a2, b2);
            Assert.AreSame(a3, b3);
            Assert.AreSame(a4, b4);
            Assert.AreSame(a5, b5);

            Assert.AreEqual(5, TableViewCellMock1.instanceCount);
        }
    }
}

