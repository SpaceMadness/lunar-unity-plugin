//
//  ConsoleView.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2015 Alex Lementuev, SpaceMadness.
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

using UnityEditor;
using UnityEditorInternal;

using System;
using System.Collections.Generic;
using System.Text;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class ConsoleView : TableView, ITableViewDataSource, ITableViewDelegate, IConsoleDelegate
    {
        private readonly AbstractConsole m_console;

        private ConsoleFilteredDelegate m_filteredDelegate;

        private ITextMeasure m_textMeasure;
        private float m_overridenContentWidth;

        public ConsoleView(AbstractConsole console, float width, float height)
            : base(console.Capacity, width, height)
        {
            m_console = console;

            this.ConsoleDelegate = this;
            this.DataSource = this;
            this.Delegate = this;

            m_textMeasure = CreateTextMeasure();

            m_filteredDelegate = new ConsoleFilteredDelegate(this);

            ReloadData();
        }

        protected virtual ITextMeasure CreateTextMeasure()
        {
            return new GUIStyleTextMeasure(SharedStyles.consoleTextStyle);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IConsoleDelegate implementation

        public void OnConsoleEntryAdded(AbstractConsole console, ref ConsoleViewCellEntry entry)
        {
            ReloadNewData();
            Repaint();
        }

        public void OnConsoleCleared(AbstractConsole console)
        {
            ReloadData();
            Repaint();
        }

        #endregion

        #region ITableViewDataSource implementation

        public TableViewCell TableCellForRow(TableView table, int rowIndex)
        {
            int index = Entries.ToArrayIndex(rowIndex);
            return CreateTableCell(table, ref Entries.InternalArray[index]);
        }

        public int NumberOfRows(TableView table)
        {
            return Entries.Length;
        }

        #endregion

        #region ITableViewDelegate implementation

        public float HeightForTableCell(int rowIndex)
        {
            int index = Entries.ToArrayIndex(rowIndex);
            return HeightForTableCell(ref Entries.InternalArray[index]);
        }

        public void OnTableCellSelected(TableView table, int rowIndex)
        {
            Repaint();
        }

        public void OnTableCellDeselected(TableView table, int rowIndex)
        {
            Repaint();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void Clear()
        {
            m_console.Clear();
        }

        #region Text

        public string GetText()
        {
            return GetText(Entries.HeadIndex, Entries.RealLength);
        }

        public string GetText(int fromLine, int length)
        {
            StringBuilder buffer = new StringBuilder();
            GetText(buffer, fromLine, length);
            return buffer.ToString();
        }

        public void GetText(StringBuilder buffer)
        {
            GetText(buffer, Entries.HeadIndex, Entries.RealLength);
        }

        public void GetText(StringBuilder buffer, int fromLine, int length)
        {
            if (fromLine < Entries.HeadIndex)
            {
                throw new ArgumentOutOfRangeException("From line is out of range");
            }

            int toLine = fromLine + length;
            if (toLine > Entries.Length)
            {
                throw new ArgumentOutOfRangeException("To line is out of range");
            }

            for (int i = fromLine; i < toLine;)
            {
                buffer.Append(StringUtils.RemoveRichTextTags(Entries[i++].value));
                if (i < toLine)
                {
                    buffer.Append('\n');
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Cell clicks

        protected override bool OnMouseDown(Event evt, TableViewCell cell)
        {
            ConsoleViewCell consoleCell = cell as ConsoleViewCell;
            if (consoleCell != null && consoleCell.OnMouseDown(evt))
            {
                return true;
            }

            return base.OnMouseDown(evt, cell);
        }

        protected override bool OnMouseDoubleClick(Event evt, TableViewCell cell)
        {
            ConsoleViewCell consoleCell = cell as ConsoleViewCell;
            if (consoleCell != null && consoleCell.OnMouseDoubleClick(evt))
            {
                return true;
            }

            return base.OnMouseDoubleClick(evt, cell);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private TableViewCell CreateTableCell(TableView table, ref ConsoleViewCellEntry entry)
        {
            if (entry.IsPlain || entry.IsTable)
            {
                ConsoleTextEntryView cell;

                if (entry.level == LogLevel.Exception)
                {
                    ConsoleTextEntryExceptionView exceptionCell = table.DequeueReusableCell<ConsoleTextEntryExceptionView>();
                    if (exceptionCell == null)
                    {
                        exceptionCell = new ConsoleTextEntryExceptionView();
                    }
                    exceptionCell.StackTraceLines = entry.data as StackTraceLine[];

                    cell = exceptionCell;
                }
                else
                {
                    cell = table.DequeueReusableCell<ConsoleTextEntryView>();
                    if (cell == null)
                    {
                        cell = new ConsoleTextEntryView();
                    }
                }

                // set the size first to calculate individual lines height
                ColorCode colorCode = entry.level != null ? entry.level.Color : ColorCode.Plain;
                cell.TextColor = SkinColors.GetColor(colorCode);
                cell.Width = entry.width;
                cell.Height = entry.height;

                cell.Value = entry.value;
                cell.LogLevel = entry.level;
                cell.StackTrace = entry.stackTrace;

                return cell;
            }
            else
            {
                throw new NotImplementedException("Unexpected entry type");
            }
        }

        private float HeightForTableCell(ref ConsoleViewCellEntry entry)
        {
            if (entry.width != this.ContentWidth)
            {
                entry.Layout(m_textMeasure, this.Width - UISize.ScrollBarWidth, this.ContentWidth);
            }

            return entry.height;
        }

        //////////////////////////////////////////////////////////////////////////////

        protected override void OnResize(float dx, float dy)
        {
            if (dx != 0)
            {
                float oldTotalHeight = this.TotalHeight;
                base.OnResize(dx, dy);
                if (oldTotalHeight == this.TotalHeight) // no re-layout: adjust cells size
                {
                    for (TableViewCell cell = this.FirstVisibleCell; cell != null; cell = cell.NextCell)
                    {
                        cell.OnTableResized(dx, dy);
                    }
                }
            }
            else
            {
                base.OnResize(dx, dy);
            }
        }

        protected override float ContentWidth
        {
            get { return m_overridenContentWidth > 0 ? m_overridenContentWidth : base.ContentWidth; }
        }

        protected override float ContentHeight 
        {
            get { return ContentWidth > Width ? Height - UISize.ScrollBarWidth : Height; }
        }

        internal float OverridenContentWidth
        {
            get { return m_overridenContentWidth; }
            set { m_overridenContentWidth = value; }
        }

        //////////////////////////////////////////////////////////////////////////////

        public override void ScrollUntilRowVisible(int rowIndex) // TODO: rename
        {
            base.ScrollUntilRowVisible(rowIndex);
            Repaint();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Filtering

        public void SetFilterText(string filterText)
        {
            bool needReload = m_filteredDelegate.SetFilterText(filterText);
            SetFilteredDelegate(m_filteredDelegate, needReload);
        }

        public void SetFilterLogLevel(LogLevel level)
        {
            bool needReload = m_filteredDelegate.SetFilterLogLevel(level);
            SetFilteredDelegate(m_filteredDelegate, needReload);
        }

        public void SetFilterTags(params Tag[] tags)
        {
            bool needReload = m_filteredDelegate.SetFilterTags(tags);
            SetFilteredDelegate(m_filteredDelegate, needReload);
        }

        public void AddFilterTags(params Tag[] tags)
        {
            bool needReload = m_filteredDelegate.AddFilterTags(tags);
            SetFilteredDelegate(m_filteredDelegate, needReload);
        }

        public void RemoveFilterTags(params Tag[] tags)
        {
            bool needReload = m_filteredDelegate.RemoveFilterTags(tags);
            SetFilteredDelegate(m_filteredDelegate, needReload);
        }

        private void SetFilteredDelegate(ConsoleFilteredDelegate del, bool needReload = true)
        {
            if (del == null || !del.HasFilters)
            {
                this.Delegate = this;
                this.DataSource = this;
                m_console.Delegate = this;
            }
            else
            {
                this.Delegate = del;
                this.DataSource = del;
                m_console.Delegate = del;
            }

            if (needReload)
            {
                ReloadData();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        internal IConsoleDelegate ConsoleDelegate
        {
            get { return m_console.Delegate;  }
            set { m_console.Delegate = value; }
        }

        public override bool IsFocused
        {
            get { return RootView.IsFocused; }
        }

        public bool IsFiltering
        {
            get { return m_filteredDelegate != null && m_filteredDelegate.HasFilters; }
        }

        internal CycleArray<ConsoleViewCellEntry> Entries { get { return m_console.Entries; } }

        #endregion
    }

    class ConsoleViewCell : TableViewCell
    {
        public virtual bool OnMouseDown(Event evt)
        {
            return false;
        }

        public virtual bool OnMouseDoubleClick(Event evt)
        {
            return false;
        }

        public virtual bool OnMouseUp(Event evt)
        {
            return false;
        }

        public virtual bool OnKeyDown(Event evt)
        {
            return false;
        }

        public ConsoleView ConsoleView { get { return Table as ConsoleView; } }
    }

    class ConsoleTextEntryView : ConsoleViewCell
    {
        protected string m_value;

        protected override void DrawGUI()
        {
            GUIStyle style = SharedStyles.consoleTextStyle;
            Color oldColor = style.normal.textColor;
            style.normal.textColor = this.TextColor;
            GUI.Label(Frame, this.Value, style);
            style.normal.textColor = oldColor;
        }

        protected internal override void PrepareForReuse()
        {
            base.PrepareForReuse();
            m_value = null;
        }

        //////////////////////////////////////////////////////////////////////////////

        public override bool OnMouseDoubleClick(Event evt)
        {
            if (!string.IsNullOrEmpty(StackTrace))
            {
                if (Editor.OpenFileExternal(StackTrace))
                {
                    return true;
                }
            }
            else if (this.LogLevel == LogLevel.Error || this.LogLevel == LogLevel.Warn)
            {
                SourcePathEntry element;
                if (EditorStackTrace.TryParseCompilerMessage(m_value, out element))
                {
                    if (Editor.OpenFileAtLineExternal(element.sourcePath, element.lineNumber))
                    {
                        return true;
                    }
                }
            }

            return base.OnMouseDoubleClick(evt);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public string StackTrace { get; set; }

        public virtual string Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public LogLevel LogLevel
        {
            get;
            set;
        }

        public Color TextColor { get; set; }

        #endregion
    }

    class ConsoleTextEntryExceptionView : ConsoleTextEntryView
    {
        private StackTraceLine[] m_stackTraceLines = StackTraceLine.kEmptyLinesArray;

        protected override void DrawGUI()
        {
            BeginGroup(Frame);
            {
                GUIStyle style = SharedStyles.consoleTextStyle;
                Color oldColor = style.normal.textColor;
                style.normal.textColor = this.TextColor;

                // title
                GUI.Label(Bounds, this.Value, style);

                // stack trace
                if (m_stackTraceLines != null)
                {
                    for (int i = 0; i < m_stackTraceLines.Length; ++i)
                    {
                        DrawStackLine(ref m_stackTraceLines[i], style);
                    }
                }

                style.normal.textColor = oldColor;
            }
            EndGroup();
        }

        private void DrawStackLine(ref StackTraceLine stackLine, GUIStyle style)
        {
            if (stackLine.IsClickable)
            {
                GUIStyle linkStyle = stackLine.sourcePathExists ? SharedStyles.consoleLinkStyle : SharedStyles.consoleLinkInnactiveStyle;
                UIHelper.DrawUnderLine(stackLine.sourceFrame, linkStyle);

                if (stackLine.sourcePathExists && GUI.Button(stackLine.sourceFrame, GUIContent.none, GUIStyle.none))
                {
                    Editor.OpenFileAtLineExternal(stackLine.sourcePath, stackLine.lineNumber);
                }
            }
            GUI.Label(stackLine.frame, stackLine.line, style);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Mouse

//        public override bool OnMouseDoubleClick(Event evt)
//        {
//            StringEntry[] entries = this.StringEntries;
//            if (entries != null)
//            {
//                for (int i = 0; i < entries.Length; ++i)
//                {
//                    if (entries[i].frame.Contains(evt.mousePosition) &&
//                    entries[i].isStackTraceElement)
//                    {
//                        StackTrackElement element;
//                        if (DotNetStackTrackParser.Parse(entries[i].value, out element))
//                        {
//                            return UnityEditorUtility.OpenFileAtLineExternal(element.SourcePath, element.LineNumber);
//                        }
//
//                        return base.OnMouseDoubleClick(evt);
//                    }
//                }
//            }
//
//            return base.OnMouseDoubleClick(evt);
//        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public StackTraceLine[] StackTraceLines
        {
            get { return m_stackTraceLines; }
            set { m_stackTraceLines = value; }
        }

        #endregion
    }
}