using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

namespace TableViewTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture()]
    public partial class TableViewTest
    {
        public static List<string> result;

        [SetUp()]
        public void SetUp()
        {
            result = new List<string>();
            TableViewCellMock1.instanceCount = 0;
            TableViewCellMock2.instanceCount = 0;
            TableViewCellMock3.instanceCount = 0;
        }

        #region Helpers

        private void AssertVisibleRows(CTableView table, params int[] indices)
        {
            Assert.AreEqual(indices.Length, table.VisibleCellsCount);
            int index = 0;
            CTableViewCell cell = table.FirstVisibleCell;
            CTableViewCell lastCell = null;
            while (cell != null)
            {
                Assert.AreEqual(indices[index++], cell.CellIndex);
                lastCell = cell;
                cell = cell.NextCell;
            }
            Assert.AreEqual(indices.Length, index);
            Assert.AreSame(lastCell, table.LastVisibleCell);
        }

        private void AssertVisibleRows(CTableView table, params CTableViewCell[] cells)
        {
            Assert.AreEqual(cells.Length, table.VisibleCellsCount);
            int index = 0;
            CTableViewCell cell = table.FirstVisibleCell;
            CTableViewCell lastCell = null;
            while (cell != null)
            {
                Assert.AreSame(cells[index++], cell);
                lastCell = cell;
                cell = cell.NextCell;
            }
            Assert.AreEqual(cells.Length, index);
            Assert.AreSame(lastCell, table.LastVisibleCell);
        }

        #endregion

        #region Helper classes

        class TestCellsHeightTableAdapter : TableViewAdapter
        {
            private List<MockCellEntry> m_cellsEntries;

            public TestCellsHeightTableAdapter(MockCellEntry[] cellsHeights)
            {
                m_cellsEntries = new List<MockCellEntry>(cellsHeights);
            }

            public override int NumberOfRows(CTableView table)
            {
                return Count;
            }

            public override CTableViewCell TableCellForRow(CTableView table, int rowIndex)
            {
                CTableViewCell cell = table.DequeueReusableCell(m_cellsEntries[rowIndex].type);
                if (cell == null)
                {
                    float width = table.Width;
                    float height = m_cellsEntries[rowIndex].height;
                    return (CTableViewCell) Activator.CreateInstance(m_cellsEntries[rowIndex].type, width, height);
                }

                return cell;
            }

            public override float HeightForTableCell(int rowIndex)
            {
                return m_cellsEntries[rowIndex].height;
            }

            public void Add(params MockCellEntry[] entries)
            {
                m_cellsEntries.AddRange(entries);
            }

            public int Count
            {
                get { return m_cellsEntries.Count; }
            }
        }

        class TestCellPredefinedAdapter : TableViewAdapter
        {
            private List<CTableViewCell> m_cells;

            public TestCellPredefinedAdapter(CTableViewCell[] cells)
            {
                m_cells = new List<CTableViewCell>(cells);
            }

            public override int NumberOfRows(CTableView table)
            {
                return Count;
            }

            public override CTableViewCell TableCellForRow(CTableView table, int rowIndex)
            {
                CTableViewCell cell = m_cells[rowIndex];
                if (cell is TableViewCellMock)
                {
                    TableViewCellMock mockCell = cell as TableViewCellMock;
                    mockCell.DetachFromList();
                }

                return cell;
            }

            public override float HeightForTableCell(int rowIndex)
            {
                return m_cells[rowIndex].Height;
            }

            public void Add(params CTableViewCell[] cells)
            {
                m_cells.AddRange(cells);
            }

            public int Count
            {
                get { return m_cells.Count; }
            }
        }

        class TestCellCapacityAdapter : TableViewAdapter
        {
            private CCycleArray<CTableViewCell> m_cells;

            public TestCellCapacityAdapter(int capacity, params CTableViewCell[] cells)
            {
                m_cells = new CCycleArray<CTableViewCell>(capacity);
                Add(cells);
            }

            public override int NumberOfRows(CTableView table)
            {
                return Count;
            }

            public override CTableViewCell TableCellForRow(CTableView table, int rowIndex)
            {
                return m_cells[rowIndex];
            }

            public override float HeightForTableCell(int rowIndex)
            {
                return m_cells[rowIndex].Height;
            }

            public void Add(CTableViewCell cell)
            {
                m_cells.Add(cell);
            }

            public void Add(params CTableViewCell[] cells)
            {
                foreach (CTableViewCell cell in cells)
                {
                    Add(cell);
                }
            }

            public int Count
            {
                get { return m_cells.Length; }
            }
        }

        class MockCellEntry
        {
            public float height;
            public Type type;

            public MockCellEntry(Type type, float height)
            {
                this.type = type;
                this.height = height;
            }
        }
    }

    #endregion

    #region Mocks

    class TableViewMock : CTableView
    {
        public TableViewMock(float width, float height)
            : this(100, width, height)
        {
        }

        public TableViewMock(int capacity, float width, float height)
            : base(capacity, width, height)
        {
        }

        public override void ReloadNewData()
        {
            base.ReloadNewData();
            ScrollGUIPosTop = ScrollPosTop - ContentOffset;
        }

        protected override void BeginScrollView()
        {
        }

        protected override void EndScrollView()
        {
        }

        protected override void BeginGroup(Rect frame)
        {
        }

        protected override void EndGroup()
        {
        }

        public override void FocusControl()
        {
        }
    }

    class TableViewCellMock : CTableViewCell
    {
        private int instanceNumber;

        public TableViewCellMock(float width, float height)
            : this(0, width, height)
        {
        }

        public TableViewCellMock(int instanceNumber, float width, float height)
            : base(width, height)
        {
            this.instanceNumber = instanceNumber;
        }

        protected override void BeginGroup(Rect frame)
        {
        }

        protected override void EndGroup()
        {
        }

        public override string ToString()
        {
            return GetType().Name + "-" + instanceNumber;
        }
    }

    class TableViewCellMock1 : TableViewCellMock
    {
        public static int instanceCount;

        public TableViewCellMock1(float width, float height)
            : base(++instanceCount, width, height)
        {
        }
    }

    class TableViewCellMock2 : TableViewCellMock
    {
        public static int instanceCount;

        public TableViewCellMock2(float width, float height)
            : base(++instanceCount, width, height)
        {
        }
    }

    class TableViewCellMock3 : TableViewCellMock
    {
        public static int instanceCount;

        public TableViewCellMock3(float width, float height)
            : base(++instanceCount, width, height)
        {
        }
    }

    #endregion

    class TableViewAdapter : ICTableViewDataSource, ICTableViewDelegate
    {
        #region ITableViewDataSource implementation

        public virtual CTableViewCell TableCellForRow(CTableView table, int rowIndex)
        {
            return null;
        }
        public virtual int NumberOfRows(CTableView table)
        {
            return 0;
        }

        #endregion

        #region ITableViewDelegate implementation

        public virtual float HeightForTableCell(int rowIndex)
        {
            return 0;
        }

        public virtual void OnTableCellSelected(CTableView table, int rowIndex)
        {
        }

        public virtual void OnTableCellDeselected(CTableView table, int rowIndex)
        {
        }

        #endregion
    }
}

