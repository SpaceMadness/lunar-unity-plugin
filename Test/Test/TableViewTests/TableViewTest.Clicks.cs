using System;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using UnityEngine;

using NUnit.Framework;
using Event = LunarEditor.CEvent;

namespace TableViewTests
{
    using Assert = NUnit.Framework.Assert;
    using CEvent = LunarEditor.CEvent;

    public partial class TableViewTest
    {
        [Test()]
        public void TestClick()
        {
            MockCellEntry[] cells =
            {
                new MockCellEntry(typeof(TableViewCellMock1), 10),
                new MockCellEntry(typeof(TableViewCellMock2), 15),
                new MockCellEntry(typeof(TableViewCellMock3), 10),
                new MockCellEntry(typeof(TableViewCellMock1), 15),
                new MockCellEntry(typeof(TableViewCellMock2), 10),
                new MockCellEntry(typeof(TableViewCellMock3), 15),
            };

            TableViewAdapter adapter = new TestCellsHeightTableAdapter(cells);

            TableViewClickMock table = new TableViewClickMock(320, 90);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            table.Click(10, 5);
            Assert.AreEqual(0, table.ClickedCellIndex);

            table.Click(10, 20);
            Assert.AreEqual(1, table.ClickedCellIndex);

            table.Click(10, 30);
            Assert.AreEqual(2, table.ClickedCellIndex);

            table.Click(10, 45);
            Assert.AreEqual(3, table.ClickedCellIndex);

            table.Click(10, 55);
            Assert.AreEqual(4, table.ClickedCellIndex);

            table.Click(10, 70);
            Assert.AreEqual(5, table.ClickedCellIndex);

            table.Click(10, 80);
            Assert.AreEqual(-1, table.ClickedCellIndex);
        }

        [Test()]
        public void TestClickAfterScroll()
        {
            MockCellEntry[] cells =
            {
                new MockCellEntry(typeof(TableViewCellMock1), 10),
                new MockCellEntry(typeof(TableViewCellMock2), 15),
                new MockCellEntry(typeof(TableViewCellMock3), 10),
                new MockCellEntry(typeof(TableViewCellMock1), 15),
                new MockCellEntry(typeof(TableViewCellMock2), 10),
                new MockCellEntry(typeof(TableViewCellMock3), 15),
            };

            TableViewAdapter adapter = new TestCellsHeightTableAdapter(cells);

            TableViewClickMock table = new TableViewClickMock(320, 10);
            table.DataSource = adapter;
            table.Delegate = adapter;
            table.ReloadData();

            table.Click(10, 5);
            Assert.AreEqual(0, table.ClickedCellIndex);

            table.Scroll(10);
            table.Click(10, 5);
            Assert.AreEqual(1, table.ClickedCellIndex);

            table.Scroll(15);
            table.Click(10, 5);
            Assert.AreEqual(2, table.ClickedCellIndex);

            table.Scroll(10);
            table.Click(10, 5);
            Assert.AreEqual(3, table.ClickedCellIndex);

            table.Scroll(15);
            table.Click(10, 5);
            Assert.AreEqual(4, table.ClickedCellIndex);

            table.Scroll(10);
            table.Click(10, 5);
            Assert.AreEqual(5, table.ClickedCellIndex);
        }
    }

    class TableViewClickMock : TableViewMock
    {
        public TableViewClickMock(float width, float height)
            : base(width, height)
        {
        }

        public void Click(float x, float y)
        {
            ClickedCell = null;

            CEvent evt = new MockEvent();
            evt.mousePosition = new Vector2(x, y);

            evt.type = EventType.MouseDown;
            HandleEvent(evt);

            evt.type = EventType.MouseUp;
            HandleEvent(evt);
        }

        protected override bool OnMouseDown(CEvent evt, CTableViewCell cell)
        {
            ClickedCell = cell;
            return true;
        }

        public CTableViewCell ClickedCell { get; private set; }
        public int ClickedCellIndex
        {
            get { return ClickedCell != null ? ClickedCell.CellIndex : -1; }
        }
    }

    class MockEvent : CEvent
    {
        private bool m_used;

        public override void Use()
        {
            m_used = true;
        }

        public override EventType type { get; set; }

        public override bool isMouse
        {
            get
            {
                EventType type = this.type;
                return type == EventType.MouseMove || type == EventType.MouseDown || type == EventType.MouseUp || type == EventType.MouseDrag;
            }
        }
        public override bool isKey
        {
            get
            {
                EventType type = this.type;
                return type == EventType.KeyDown || type == EventType.KeyUp;
            }
        }

        public override KeyCode keyCode { get; set; }
        public override char character { get; set; }
        public override bool control { get; set; }
        public override bool shift { get; set; }
        public override bool alt { get; set; }
        public override bool command { get; set; }
        public override Vector2 mousePosition { get; set; }
        public override int button { get; set; }
        public override int clickCount { get; set; }

        protected override CEvent CurrentEvent
        {
            get { return m_used ? null : this; }
        }

        protected override void DestroyEvent()
        {
        }
    }
}

