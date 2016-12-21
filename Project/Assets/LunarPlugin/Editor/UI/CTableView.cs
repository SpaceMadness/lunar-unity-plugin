//
//  CTableView.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

﻿using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LunarPluginInternal;

namespace LunarEditor
{
    using TableViewCellList = CFastList<CTableViewCell>;

    interface ICTableViewDataSource
    {
        CTableViewCell TableCellForRow(CTableView table, int rowIndex);
        int NumberOfRows(CTableView table);
    }

    interface ICTableViewDelegate
    {
        float HeightForTableCell(int rowIndex);
        void OnTableCellSelected(CTableView table, int rowIndex);
        void OnTableCellDeselected(CTableView table, int rowIndex);
    }

    interface ICTableViewScrollDelegate
    {
        void OnTableScroll(CTableView table, float oldPos);
    }

    enum CTableViewSelectionMode
    {
        None,
        Single,
        Multiple
    }

    class CTableView : CView
    {
        private IDictionary<Type, TableViewCellList> m_reusableCellsLists;
        private CCycleArray<CTableViewCellEntry> m_cellsEntries;
        private CFastList<CTableViewCell> m_visibleCells;

        private float m_totalHeight;
        private float m_contentOffset;

        private float m_scrollPos;
        private Vector3 m_guiScrollPos;
        private float m_horizontalScrollPos;

        private int m_lastSelectedIndex;

        public CTableView(int capacity, float width, float height)
            : base(width, height)
        {
            if (capacity < 0)
            {
                throw new ArgumentException("Negative capacity: " + capacity);
            }

            m_reusableCellsLists = new Dictionary<Type, TableViewCellList>();
            m_cellsEntries = new CCycleArray<CTableViewCellEntry>(capacity);
            m_visibleCells = new CFastList<CTableViewCell>();

            this.DataSource = CTableViewNullDataSource.Instance; // don't do null reference checks
            this.SelectionMode = CTableViewSelectionMode.None;

            this.IsMouseEventsEnabled = true;
            this.MouseDown = MouseDownEventHandler;
            this.MouseDoubleClick = MouseDoubleClickHandler;

            this.IsFocusable = true;
            this.KeyDown = KeyDownEventHandler;
            this.KeyUp = KeyUpEventHandler;
        }

        #region GUI

        protected override void DrawGUI()
        {
            BeginGroup(Frame);
            {
                DrawBackgroud();

                BeginScrollView();
                {
                    DrawVisibleCells();
                }
                EndScrollView();
            }
            EndGroup();
        }

        protected virtual void DrawVisibleCells()
        {
            #if LUNAR_DEVELOPMENT
            if (CSystemVars.g_drawVisibleCells.BoolValue)
            {
                if (FirstVisibleCell != null)
                {
                    GUI.Box(FirstVisibleCell.Frame, GUIContent.none);
                }

                if (LastVisibleCell != null)
                {
                    GUI.Box(LastVisibleCell.Frame, GUIContent.none);
                }
            }
            #endif // LUNAR_DEVELOPMENT

            for (CTableViewCell cell = FirstVisibleCell; cell != null; cell = cell.NextCell)
            {
                if (IsCellSelected(cell.CellIndex))
                {
                    CUIHelper.DrawRect(cell.Frame, CSharedStyles.SelectedCellBackStyle);
                }
                cell.OnGUI();
            }
        }

        protected virtual void BeginScrollView()
        {
            float contentWidth = this.ContentWidth;
            float contentHeight = this.ContentHeight;

            Rect scrollRect = new Rect(0, 0, contentWidth, Mathf.Max(m_totalHeight - m_contentOffset, contentHeight));

            if (scrollRect.width > Width)
            {
                float barWidth = scrollRect.height > contentHeight ? Width - CUISize.ScrollBarWidth : Width;
                float minPos = 0;
                float maxPos = contentWidth - Width;
                float barSize = Width * (contentWidth - Width) / contentWidth;
                m_horizontalScrollPos = GUI.HorizontalScrollbar(new Rect(0, contentHeight, barWidth, CUISize.ScrollBarWidth), m_horizontalScrollPos, barSize, minPos, maxPos);
                m_guiScrollPos.x = m_horizontalScrollPos;
            }

            m_guiScrollPos.y = m_scrollPos - m_contentOffset;
            float oldGuiScrollPos = m_guiScrollPos.y;
            m_guiScrollPos = GUI.BeginScrollView(new Rect(0, 0, Width, contentHeight), m_guiScrollPos, scrollRect, GUIStyle.none, GUI.skin.verticalScrollbar);

            if (oldGuiScrollPos != m_guiScrollPos.y)
            {
                Scroll(m_guiScrollPos.y - oldGuiScrollPos);
            }
        }

        protected virtual void EndScrollView()
        {
            GUI.EndScrollView();
        }

        #endregion

        public T DequeueReusableCell<T>() where T : CTableViewCell
        {
            return (T) DequeueReusableCell(typeof(T));
        }

        public CTableViewCell DequeueReusableCell(Type cellType)
        {
            CTableViewCell cell = TryGetUsedCellForType(cellType);
            if (cell != null)
            {
                cell.PrepareForReuse();
                return cell;
            }

            return null;
        }

        private CTableViewCell TryGetUsedCellForType(Type type)
        {
            TableViewCellList cellList;
            if (m_reusableCellsLists.TryGetValue(type, out cellList))
            {
                return cellList.RemoveLastItem();
            }

            return null;
        }

        private void RecycleCell(CTableViewCell cell)
        {
            Type cellType = cell.GetType();

            TableViewCellList cellList;
            if (!m_reusableCellsLists.TryGetValue(cellType, out cellList))
            {
                cellList = new TableViewCellList();
                m_reusableCellsLists[cellType] = cellList;
            }

            cell.PrepareForReuse();
            cellList.AddLastItem(cell);
        }

        public void ReloadData()
        {
            // clear old data
            RemoveVisibleCells();

            // cell location
            m_cellsEntries.Clear();

            // clear fields
            m_totalHeight = 0;
            m_contentOffset = 0;
            m_scrollPos = 0;
            m_horizontalScrollPos = 0;
            m_lastSelectedIndex = -1;
            m_guiScrollPos = Vector3.zero;

            ReloadNewData();
        }

        public virtual void ReloadNewData()
        {
            int rowsCount = this.RowsCount;
            while (m_cellsEntries.Length < rowsCount)
            {
                AddCell();
            }

            if (IsScrollLocked)
            {
                ScrollToBottom();
            }
        }

        private void AddCell()
        {
            // the "head" index will be moved after new element is added
            if (m_cellsEntries.RealLength == m_cellsEntries.Capacity)
            {
                // "shift" content to compensate the lost cell
                int headIndex = m_cellsEntries.HeadIndex;
                ShiftContent(m_cellsEntries[headIndex].Height);
            }

            int cellIndex = m_cellsEntries.Length;
            float cellHeight = Delegate.HeightForTableCell(cellIndex);
            m_cellsEntries.Add(new CTableViewCellEntry(m_totalHeight, cellHeight));

            if (IsCellVisible(cellIndex, m_scrollPos))
            {
                CTableViewCell cell = TableCellForRow(cellIndex);
                m_visibleCells.AddLastItem(cell);
            }

            m_totalHeight += cellHeight;
        }

        internal void TrimCellsToHead(int newHeadIndex)
        {
            float contentOffset = 0;
            for (int i = m_cellsEntries.HeadIndex; i < newHeadIndex; ++i)
            {
                contentOffset += m_cellsEntries[i].Height;
            }
            ShiftContent(contentOffset);

            m_cellsEntries.TrimToHeadIndex(newHeadIndex);
        }

        private void ShiftContent(float offset)
        {
            // we should scroll first and then shift cells
            Scroll(offset); // this will remove a cell if it's not visible
            for (CTableViewCell cell = FirstVisibleCell; cell != null; cell = cell.NextCell)
            {
                cell.Y -= offset;
            }

            m_contentOffset += offset;
        }

        private void RemoveVisibleCell(CTableViewCell cell)
        {
            m_visibleCells.RemoveItem(cell);
            RecycleCell(cell);
        }

        private void RemoveVisibleCells()
        {
            CTableViewCell cell;
            while ((cell = m_visibleCells.RemoveFirstItem()) != null)
            {
                RecycleCell(cell);
            }
        }

        #region Scrolling

        public void ScrollToBottom()
        {
            int rowsCount = RowsCount;
            if (rowsCount > 0)
            {
                ScrollUntilRowVisible(rowsCount - 1);
            }
        }

        public virtual void ScrollUntilRowVisible(int rowIndex)
        {
            CheckRowIndex(rowIndex);

            if (!IsCellFullyVisible(rowIndex)) // do we need scrolling at all?
            {
                float newPos = Mathf.Clamp(m_scrollPos, m_cellsEntries[rowIndex].Bottom - ContentHeight, m_cellsEntries[rowIndex].Top);
                ScrollTo(newPos);
            }
        }

        public void Scroll(float delta)
        {
            ScrollTo(m_scrollPos + delta);
        }

        public void ScrollTo(float newPos)
        {
            float oldPos = m_scrollPos;
            m_scrollPos = newPos;

            OnScroll(oldPos, newPos);
        }

        protected void OnScroll(float oldPos, float newPos)
        {
            float delta = newPos - oldPos;

            int firstVisibleCellIndex = FirstVisibleCellIndex;
            int lastVisibleCellIndex = LastVisibleCellIndex;

            if (delta > 0)
            {
                int rowsCount = RowsCount;

                // wipe invisible rows
                while (!IsCellVisible(firstVisibleCellIndex, newPos) && VisibleCellsCount > 0)
                {
                    RemoveVisibleCell(FirstVisibleCell);
                    ++firstVisibleCellIndex;
                }

                if (VisibleCellsCount > 0) // not all rows were wiped: just append more visible cells at the end
                {
                    for (int cellIndex = lastVisibleCellIndex + 1; cellIndex < rowsCount && IsCellVisible(cellIndex, newPos); ++cellIndex)
                    {
                        CTableViewCell cell = TableCellForRow(cellIndex);
                        m_visibleCells.AddLastItem(cell);
                    }
                }
                else
                {
                    int rowIndex = FindFirstVisibleRow(newPos, lastVisibleCellIndex + 1, rowsCount - 1);
                    while (rowIndex < rowsCount && IsCellVisible(rowIndex, newPos))
                    {
                        CTableViewCell cell = TableCellForRow(rowIndex);
                        m_visibleCells.AddLastItem(cell);
                        ++rowIndex;
                    }
                }
            }
            else if (delta < 0)
            {
                while (this.VisibleCellsCount > 0 && !IsCellVisible(lastVisibleCellIndex, newPos))
                {
                    RemoveVisibleCell(LastVisibleCell);
                    --lastVisibleCellIndex;
                }

                if (VisibleCellsCount > 0) // not all rows were wiped: just prepend more visible cells at the beginning
                {
                    for (int cellIndex = firstVisibleCellIndex - 1; cellIndex >= m_cellsEntries.HeadIndex && IsCellVisible(cellIndex, newPos); --cellIndex)
                    {
                        CTableViewCell cell = TableCellForRow(cellIndex);
                        m_visibleCells.AddFirstItem(cell);
                    }
                }
                else
                {
                    int rowIndex = FindLastVisibleRow(newPos, m_cellsEntries.HeadIndex, firstVisibleCellIndex - 1);
                    while (rowIndex >= m_cellsEntries.HeadIndex && IsCellVisible(rowIndex, newPos))
                    {
                        CTableViewCell cell = TableCellForRow(rowIndex);
                        m_visibleCells.AddFirstItem(cell);
                        --rowIndex;
                    }
                }
            }

            if (ScrollDelegate != null)
            {
                ScrollDelegate.OnTableScroll(this, oldPos);
            }
        }

        private int FindFirstVisibleRow(float scrollPos, int lo, int hi)
        {
            bool top, bottom;
            while (hi > lo + 1)
            {
                int med = (hi + lo) / 2;
                top = m_cellsEntries[med].Top <= scrollPos;
                bottom = m_cellsEntries[med].Bottom > scrollPos;
                if (top && bottom)
                {
                    return med;
                }

                if (top) lo = med;
                else hi = med;
            }

            return m_cellsEntries[lo].Top <= scrollPos && m_cellsEntries[lo].Bottom > scrollPos ? lo : hi;
        }

        private int FindLastVisibleRow(float scrollPos, int lo, int hi)
        {
            float scrollPosBottom = scrollPos + ContentHeight;
            bool top, bottom;
            while (hi > lo + 1)
            {
                int med = (hi + lo) / 2;
                top = m_cellsEntries[med].Top < scrollPosBottom;
                bottom = m_cellsEntries[med].Bottom >= scrollPosBottom;
                if (top && bottom)
                {
                    return med;
                }

                if (top) lo = med;
                else hi = med;
            }

            return m_cellsEntries[lo].Top <= scrollPosBottom && m_cellsEntries[lo].Bottom > scrollPosBottom ? lo : hi;
        }

        public CTableViewCell GetVisibleCell(int index)
        {
            if (index >= FirstVisibleCellIndex && index <= LastVisibleCellIndex)
            {
                for (CTableViewCell cell = FirstVisibleCell; cell != null; cell = cell.NextCell)
                {
                    if (cell.CellIndex == index)
                    {
                        return cell;
                    }
                }
            }

            return null;
        }

        private bool IsCellVisible(int index)
        {
            return IsCellVisible(index, this.ScrollPosTop);
        }

        private bool IsCellVisible(int index, float scrollPos)
        {
            return m_cellsEntries[index].Bottom > scrollPos && m_cellsEntries[index].Top < scrollPos + ContentHeight;
        }

        private bool IsCellFullyVisible(int rowIndex)
        {
            return IsCellFullyVisible(rowIndex, m_scrollPos);
        }

        private bool IsCellFullyVisible(int rowIndex, float scrollPos)
        {
            return m_cellsEntries[rowIndex].Top >= scrollPos && m_cellsEntries[rowIndex].Bottom <= scrollPos + ContentHeight;
        }

        private CTableViewCell TableCellForRow(int index)
        {
            CTableViewCell cell = DataSource.TableCellForRow(this, index);
            if (cell == null)
            {
                throw new NullReferenceException("Table view cell is null");
            }
            cell.Y = m_cellsEntries[index].Top - m_contentOffset;
            cell.Table = this;
            cell.CellIndex = index;

            return cell;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Resize

        protected override void OnResize(float dx, float dy)
        {
            base.OnResize(dx, dy);

            if (dy != 0)
            {
                OnChangeHeight(dy);
            }

            if (dx != 0)
            {
                OnChangeWidth(dx);
            }
        }

        private void OnChangeWidth(float delta)
        {
            float totalHeight = 0;
            for (int cellIndex = m_cellsEntries.HeadIndex; cellIndex < m_cellsEntries.Length; ++cellIndex)
            {
                float cellHeight = Delegate.HeightForTableCell(cellIndex);

                int arrayIndex = m_cellsEntries.ToArrayIndex(cellIndex);
                m_cellsEntries.InternalArray[arrayIndex].Height = cellHeight;
                m_cellsEntries.InternalArray[arrayIndex].Top = totalHeight;

                totalHeight += cellHeight;
            }

            float diff = totalHeight - m_totalHeight;
            if (diff != 0)
            {
                m_totalHeight = totalHeight;

                CTableViewCell lastVisibleCell = this.LastVisibleCell;
                if (lastVisibleCell != null)
                {
                    float screenDiff = (lastVisibleCell.Bottom + m_contentOffset) - this.ScrollPosTop;
                    int lastVisibleCellIndex = lastVisibleCell.CellIndex;

                    m_scrollPos = Mathf.Max(0, m_cellsEntries[lastVisibleCellIndex].Bottom - screenDiff);
                    m_contentOffset = 0;

                    // remove old visible cells
                    RemoveVisibleCells();

                    // add new visible cells
                    int rowIndex = FindFirstVisibleRow(m_scrollPos, m_cellsEntries.HeadIndex, m_cellsEntries.Length - 1);
                    while (rowIndex < m_cellsEntries.Length && IsCellVisible(rowIndex, m_scrollPos))
                    {
                        CTableViewCell cell = TableCellForRow(rowIndex);
                        m_visibleCells.AddLastItem(cell);
                        ++rowIndex;
                    }
                }
                else
                {
                    m_contentOffset = 0;
                    m_scrollPos = 0;
                }

                m_lastSelectedIndex = -1;
                m_guiScrollPos = new Vector3(0, m_scrollPos, 0);
            }
        }

        private void OnChangeHeight(float delta)
        {
            if (delta > 0)
            {
                int rowsCount = this.RowsCount;

                // append more visible cells at the end
                for (int cellIndex = this.LastVisibleCellIndex + 1; cellIndex < rowsCount && IsCellVisible(cellIndex); ++cellIndex)
                {
                    CTableViewCell cell = TableCellForRow(cellIndex);
                    m_visibleCells.AddLastItem(cell);
                }

            }
            else if (delta < 0)
            {
                // wipe invisible cells
                while (this.VisibleCellsCount > 0 && !IsCellVisible(this.LastVisibleCellIndex))
                {
                    RemoveVisibleCell(this.LastVisibleCell);
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Mouse clicks

        private bool MouseDownEventHandler(CView view, CEvent evt)
        {
            FocusControl();

            if (OnMouseDown(evt))
            {
                Repaint();
                return true;
            }

            return false;
        }

        private bool MouseDoubleClickHandler(CView view, CEvent evt)
        {
            FocusControl();

            if (OnMouseDoubleClick(evt))
            {
                Repaint();
                return true;
            }

            return false;
        }

        protected bool OnMouseDown(CEvent evt)
        {
            float clickX = evt.mousePosition.x;
            float clickY = evt.mousePosition.y + m_scrollPos - m_contentOffset;
            CTableViewCell cell = FindMouseCell(clickX, clickY);
            if (cell != null)
            {
                Vector2 oldPos = evt.mousePosition;
                evt.mousePosition = new Vector2(clickX - cell.X, clickY - cell.Y);
                bool result = OnMouseDown(evt, cell);
                evt.mousePosition = oldPos;

                return result;
            }

            return false;
        }

        protected bool OnMouseDoubleClick(CEvent evt)
        {
            float clickX = evt.mousePosition.x;
            float clickY = evt.mousePosition.y + m_scrollPos - m_contentOffset;
            CTableViewCell cell = FindMouseCell(clickX, clickY);
            if (cell != null)
            {
                Vector2 oldPos = evt.mousePosition;
                evt.mousePosition = new Vector2(clickX - cell.X, clickY - cell.Y);
                bool result = OnMouseDoubleClick(evt, cell);
                evt.mousePosition = oldPos;

                return result;
            }

            return false;
        }

        protected virtual bool OnMouseDown(CEvent evt, CTableViewCell cell)
        {
            return this.SelectionMode != CTableViewSelectionMode.None && OnCellSelected(cell);
        }

        protected virtual bool OnMouseDoubleClick(CEvent evt, CTableViewCell cell)
        {
            return false;
        }

        protected virtual bool KeyDownEventHandler(CView view, CEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.UpArrow:
                {
                    if (m_lastSelectedIndex > m_cellsEntries.HeadIndex && !evt.control)
                    {
                        int nextSelectedIndex = m_lastSelectedIndex - 1;
                        ScrollUntilRowVisible(nextSelectedIndex);
                        SwitchSelectedCell(nextSelectedIndex);

                        return true;
                    }

                    if (FirstVisibleCellIndex > m_cellsEntries.HeadIndex)
                    {
                        if (IsCellFullyVisible(FirstVisibleCellIndex))
                        {
                            ScrollUntilRowVisible(FirstVisibleCellIndex - 1);
                        }
                        else
                        {
                            ScrollUntilRowVisible(FirstVisibleCellIndex);
                        }
                        return true;
                    }

                    return false;
                }

                case KeyCode.DownArrow:
                {
                    if (m_lastSelectedIndex != -1 && m_lastSelectedIndex < m_cellsEntries.Length - 1 && !evt.control)
                    {
                        int nextSelectedIndex = m_lastSelectedIndex + 1;
                        ScrollUntilRowVisible(nextSelectedIndex);
                        SwitchSelectedCell(nextSelectedIndex);
                        return true;
                    }

                    if (LastVisibleCellIndex != 1 && LastVisibleCellIndex < m_cellsEntries.Length - 1)
                    {
                        if (IsCellFullyVisible(LastVisibleCellIndex))
                        {
                            ScrollUntilRowVisible(LastVisibleCellIndex + 1);
                        }
                        else
                        {
                            ScrollUntilRowVisible(LastVisibleCellIndex);
                        }
                        return true;
                    }
                    
                    return false;
                }
            }

            return false;
        }

        private bool KeyUpEventHandler(CView view, CEvent evt)
        {
            return false;
        }

        private bool OnCellSelected(CTableViewCell cell)
        {
            int cellIndex = cell.CellIndex;

            if (this.SelectionMode == CTableViewSelectionMode.Multiple)
            {
                SetCellSelected(cellIndex, !IsCellSelected(cellIndex));
                return true;
            }

            if (!IsCellSelected(cellIndex))
            {
                SwitchSelectedCell(cellIndex);
                return true;
            }

            return false;
        }

        private void SwitchSelectedCell(int cellIndex)
        {
            if (m_lastSelectedIndex != -1)
            {
                SetCellSelected(m_lastSelectedIndex, false);
            }
            
            SetCellSelected(cellIndex, true);
            m_lastSelectedIndex = cellIndex;
        }

        private void SetCellSelected(CTableViewCell cell, bool flag)
        {
            SetCellSelected(cell.CellIndex, flag);
        }

        private void SetCellSelected(int cellIndex, bool flag)
        {
            bool wasSelected = IsCellSelected(cellIndex);
            if (wasSelected ^ flag)
            {
                int arrayIndex = m_cellsEntries.ToArrayIndex(cellIndex);
                m_cellsEntries.InternalArray[arrayIndex].IsSelected = flag;

                if (flag)
                {
                    Delegate.OnTableCellSelected(this, cellIndex);
                }
                else
                {
                    Delegate.OnTableCellDeselected(this, cellIndex);
                }
            }
        }

        private bool IsCellSelected(CTableViewCell cell)
        {
            return IsCellSelected(cell.CellIndex);
        }

        private bool IsCellSelected(int index)
        {
            return m_cellsEntries[index].IsSelected;
        }

        private CTableViewCell FindMouseCell(float clickX, float clickY)
        {
            for (CTableViewCell cell = FirstVisibleCell; cell != null; cell = cell.NextCell)
            {
                if (cell.ContainsPoint(clickX, clickY))
                {
                    return cell;
                }
            }

            return null;
        }

        #endregion

        #region Helpers

        private void CheckRowIndex(int rowIndex) // TODO: rename
        {
            int rowsCount = RowsCount;
            if (rowIndex >= rowsCount || rowIndex < 0)
            {
                throw new IndexOutOfRangeException("Row index " + rowIndex + " is out of range 0.." + (rowsCount - 1));
            }
        }

        #endregion

        #region Properties

        public ICTableViewDataSource DataSource { get; set; }
        public ICTableViewDelegate Delegate { get; set; }
        public ICTableViewScrollDelegate ScrollDelegate { get; set; }

        public int RowsCount
        {
            get { return DataSource.NumberOfRows(this); }
        }

        public CTableViewCell FirstVisibleCell { get { return m_visibleCells.ListFirst; } }
        public CTableViewCell LastVisibleCell { get { return m_visibleCells.ListLast; } }

        public int FirstVisibleCellIndex { get { return FirstVisibleCell != null ? FirstVisibleCell.CellIndex : -1; } }
        public int LastVisibleCellIndex { get { return LastVisibleCell != null ? LastVisibleCell.CellIndex : -1; } }

        public int VisibleCellsCount { get { return m_visibleCells.Count; } }

        public CTableViewSelectionMode SelectionMode { get; set; }

        public float ScrollPosTop { get { return m_scrollPos; } }
        public float ScrollPosBottom { get { return m_scrollPos + ContentHeight; } }

        public float ScrollGUIPosTop 
        {
            get
            {
                return m_guiScrollPos.y;
            }
            protected set
            {
                m_guiScrollPos.y = value;
            }
        }
        public float ScrollGUIPosBottom { get { return m_guiScrollPos.y + ContentHeight; } }
        protected float ContentOffset { get { return m_contentOffset; } }

        public bool IsScrollLocked { get; set; }

        protected IDictionary<Type, TableViewCellList> ReusableCellsLists { get { return m_reusableCellsLists; } }
        protected float TotalHeight { get { return m_totalHeight; } }
        protected virtual float ContentWidth { get { return Width - CUISize.ScrollBarWidth; } }
        protected virtual float ContentHeight { get { return Height; } }

        #endregion

        struct CTableViewCellEntry
        {
            private float m_top;
            private float m_height;
            private bool m_selected;

            public CTableViewCellEntry(float top, float height)
            {
                m_top = top;
                m_height = height;
                m_selected = false;
            }

            public float Top 
            { 
                get { return m_top; }
                set { m_top = value; }
            }

            public float Bottom 
            { 
                get { return m_top + m_height; }
                set { m_top = value - m_height; }
            }

            public float Height
            {
                get { return m_height; }
                set { m_height = value; }
            }

            public bool IsSelected 
            {
                get { return m_selected; }
                set { m_selected = value; }
            }
        }
    }

    class CTableViewCell : CView
    {
        public CTableViewCell()
            : this(0, 0)
        {
        }

        public CTableViewCell(float width, float height)
            : base(width, height)
        {
            CellIndex = -1;
        }

        //////////////////////////////////////////////////////////////////////////////

        protected internal virtual void OnTableResized(float dx, float dy)
        {
            this.Width += dx;
        }

        protected internal virtual void PrepareForReuse()
        {
            // TODO
        }

        //////////////////////////////////////////////////////////////////////////////

        #region FastListNode

        public CTableViewCell NextCell
        {
            get { return (CTableViewCell)ListNodeNext; }
        }

        public CTableViewCell PrevCell
        {
            get { return (CTableViewCell)ListNodePrev; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public int CellIndex { get; internal set; }
        public CTableView Table { get; internal set; }

        #endregion
    }

    internal class CTableViewNullDataSource : ICTableViewDataSource
    {
        private static CTableViewNullDataSource m_instance;

        #region ITableViewDataSource implementation

        public CTableViewCell TableCellForRow(CTableView table, int rowIndex)
        {
            throw new InvalidOperationException("Null data source");
        }

        public int NumberOfRows(CTableView table)
        {
            return 0;
        }

        #endregion

        #region Properties

        public static CTableViewNullDataSource Instance
        {
            get 
            {
                if (m_instance == null)
                {
                    m_instance = new CTableViewNullDataSource();
                }

                return m_instance;
            }
        }

        #endregion
    }
}
