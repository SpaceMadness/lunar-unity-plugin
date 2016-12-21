//
//  CConsoleView.cs
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

using UnityEditor;
using UnityEditorInternal;

using System;
using System.Collections.Generic;
using System.Text;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class CConsoleView : CTableView, ICTableViewDataSource, ICTableViewDelegate, ICConsoleDelegate
    {
        private readonly CAbstractConsole m_console;

        private CConsoleFilteredDelegate m_filteredDelegate;

        private ICTextMeasure m_textMeasure;
        private float m_overridenContentWidth;

        public CConsoleView(CAbstractConsole console, float width, float height)
            : base(console.Capacity, width, height)
        {
            m_console = console;

            this.ConsoleDelegate = this;
            this.DataSource = this;
            this.Delegate = this;

            m_textMeasure = CreateTextMeasure();

            m_filteredDelegate = new CConsoleFilteredDelegate(this);

            ReloadData();
        }

        protected virtual ICTextMeasure CreateTextMeasure()
        {
            return new CGUIStyleTextMeasure(CSharedStyles.consoleTextStyle);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IConsoleDelegate implementation

        public void OnConsoleEntryAdded(CAbstractConsole console, ref CConsoleViewCellEntry entry)
        {
            ReloadNewData();
            Repaint();
        }

        public void OnConsoleCleared(CAbstractConsole console)
        {
            ReloadData();
            Repaint();
        }

        #endregion

        #region ITableViewDataSource implementation

        public CTableViewCell TableCellForRow(CTableView table, int rowIndex)
        {
            int index = Entries.ToArrayIndex(rowIndex);
            return CreateTableCell(table, ref Entries.InternalArray[index]);
        }

        public int NumberOfRows(CTableView table)
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

        public void OnTableCellSelected(CTableView table, int rowIndex)
        {
            Repaint();
        }

        public void OnTableCellDeselected(CTableView table, int rowIndex)
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
                buffer.Append(CStringUtils.RemoveRichTextTags(Entries[i++].value));
                if (i < toLine)
                {
                    buffer.Append('\n');
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Cell clicks

        protected override bool OnMouseDown(CEvent evt, CTableViewCell cell)
        {
            CConsoleViewCell consoleCell = cell as CConsoleViewCell;
            if (consoleCell != null && consoleCell.OnMouseDown(evt))
            {
                return true;
            }

            return base.OnMouseDown(evt, cell);
        }

        protected override bool OnMouseDoubleClick(CEvent evt, CTableViewCell cell)
        {
            CConsoleViewCell consoleCell = cell as CConsoleViewCell;
            if (consoleCell != null && consoleCell.OnMouseDoubleClick(evt))
            {
                return true;
            }

            return base.OnMouseDoubleClick(evt, cell);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private CTableViewCell CreateTableCell(CTableView table, ref CConsoleViewCellEntry entry)
        {
            if (entry.IsPlain || entry.IsTable)
            {
                CConsoleTextEntryView cell;

                if (entry.level == CLogLevel.Exception)
                {
                    CConsoleTextEntryExceptionView exceptionCell = table.DequeueReusableCell<CConsoleTextEntryExceptionView>();
                    if (exceptionCell == null)
                    {
                        exceptionCell = new CConsoleTextEntryExceptionView();
                    }
                    exceptionCell.StackTraceLines = entry.data as CStackTraceLine[];

                    cell = exceptionCell;
                }
                else
                {
                    cell = table.DequeueReusableCell<CConsoleTextEntryView>();
                    if (cell == null)
                    {
                        cell = new CConsoleTextEntryView();
                    }
                }

                // set the size first to calculate individual lines height
                CColorCode colorCode = entry.level != null ? entry.level.Color : CColorCode.Plain;
                cell.TextColor = CEditorSkin.GetColor(colorCode);
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

        private float HeightForTableCell(ref CConsoleViewCellEntry entry)
        {
            if (entry.width != this.ContentWidth)
            {
                entry.Layout(m_textMeasure, this.Width - CUISize.ScrollBarWidth, this.ContentWidth);
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
                    for (CTableViewCell cell = this.FirstVisibleCell; cell != null; cell = cell.NextCell)
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
            get { return ContentWidth > Width ? Height - CUISize.ScrollBarWidth : Height; }
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

        public void SetFilterLogLevel(CLogLevel level)
        {
            bool needReload = m_filteredDelegate.SetFilterLogLevel(level);
            SetFilteredDelegate(m_filteredDelegate, needReload);
        }

        public void SetFilterTags(params CTag[] tags)
        {
            bool needReload = m_filteredDelegate.SetFilterTags(tags);
            SetFilteredDelegate(m_filteredDelegate, needReload);
        }

        public void AddFilterTags(params CTag[] tags)
        {
            bool needReload = m_filteredDelegate.AddFilterTags(tags);
            SetFilteredDelegate(m_filteredDelegate, needReload);
        }

        public void RemoveFilterTags(params CTag[] tags)
        {
            bool needReload = m_filteredDelegate.RemoveFilterTags(tags);
            SetFilteredDelegate(m_filteredDelegate, needReload);
        }

        private void SetFilteredDelegate(CConsoleFilteredDelegate del, bool needReload = true)
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

        internal ICConsoleDelegate ConsoleDelegate
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

        internal CCycleArray<CConsoleViewCellEntry> Entries { get { return m_console.Entries; } }

        #endregion
    }

    public struct CConsoleViewCellEntry
    {
        internal enum Flags : byte
        {
            Plain     = 1 << 1,
            Table     = 1 << 2
        }

        private static readonly string kColSeparator = "  ";

        public string value;
        public object data;

        public float width;
        public float height;

        public CTag tag;
        public CLogLevel level;
        public string stackTrace;

        internal Flags flags;

        public CConsoleViewCellEntry(string line, float width = 0, float height = 0)
        {
            this.value = line;
            this.width = width;
            this.height = height;
            this.flags = Flags.Plain;

            this.data = null;
            this.tag = null;
            this.level = null;
            this.stackTrace = null;
        }

        public CConsoleViewCellEntry(string[] lines, float width = 0, float height = 0)
        {
            if (lines.Length == 1)
            {
                this.value = lines[0];
                this.data = null;
                this.flags = Flags.Plain;
            }
            else
            {
                this.data = lines;
                this.value = null;
                this.flags = Flags.Table;
            }
            this.width = width;
            this.height = height;

            this.tag = null;
            this.level = null;
            this.stackTrace = null;
        }

        public CConsoleViewCellEntry(Exception e, string message, float width = 0, float height = 0)
        {
            this.value = message != null ? CStringUtils.TryFormat("{0} ({1})", message, e.Message) : e.Message;
            this.width = width;
            this.height = height;
            this.flags = Flags.Plain;

            this.data = null;
            this.tag = null;
            this.level = CLogLevel.Exception;
            this.stackTrace = e.StackTrace;
        }

        internal void Layout(ICTextMeasure measure, float maxWidth)
        {
            Layout(measure, maxWidth, maxWidth);
        }

        internal void Layout(ICTextMeasure measure, float contentWidth, float maxWidth)
        {
            if (this.IsPlain)
            {
                if (this.level == CLogLevel.Exception)
                {
                    CStackTraceLine[] stackLines = this.data as CStackTraceLine[];
                    if (stackLines == null && this.stackTrace != null)
                    {
                        stackLines = CEditorStackTrace.ParseStackTrace(this.stackTrace);
                        this.data = stackLines;
                    }

                    LayoutException(measure, stackLines, maxWidth);
                }
                else
                {
                    LayoutPlain(measure, maxWidth);
                }
            }
            else if (this.IsTable)
            {
                string[] table = this.Table;
                CAssert.IsNotNull(table);

                LayoutTable(measure, table, contentWidth, maxWidth);
            }
            else
            {
                throw new NotImplementedException("Unexpected entry type");
            }

        }

        private void LayoutPlain(ICTextMeasure measure, float maxWidth)
        {
            this.width = maxWidth;
            this.height = measure.CalcHeight(value, maxWidth);
        }

        private void LayoutTable(ICTextMeasure measure, string[] table, float contentWidth, float maxWidth)
        {
            int maxLength = 0;
            string longestString = null;

            for (int i = 0; i < table.Length; ++i)
            {
                string str = table[i];
                int len = CStringUtils.Strlen(str);
                if (len > maxLength)
                {
                    maxLength = len;
                    longestString = str;
                }
            }

            int colLength = maxLength + kColSeparator.Length;

            Vector2 colSize = measure.CalcSize(kColSeparator + longestString); 
            float colWidth = colSize.x;

            int numCols = Mathf.Max(1, (int)(contentWidth / colWidth));
            int numRows = table.Length / numCols + (table.Length % numCols != 0 ? 1 : 0);

            StringBuilder buffer = new StringBuilder(colLength * table.Length);

            for (int row = 0; row < numRows; ++row)
            {
                int appendSpacing = 0;
                for (int col = 0; col < numCols; ++col)
                {
                    int index = col * numRows + row;

                    if (index > table.Length - 1)
                    {
                        break;
                    }

                    buffer.Append(' ', appendSpacing);
                    if (col > 0)
                    {
                        buffer.Append(kColSeparator);
                    }

                    string str = table[index];
                    if (str != null)
                    {
                        buffer.Append(str);
                        appendSpacing = maxLength - str.Length;
                    }
                    else
                    {
                        appendSpacing = maxLength;
                    }
                }

                if (row < numRows - 1)
                {
                    buffer.Append('\n');
                }
            }

            this.value = buffer.ToString();

            Vector2 size = measure.CalcSize(this.value);
            this.width = maxWidth;
            this.height = size.y;
        }

        private void LayoutException(ICTextMeasure measure, CStackTraceLine[] stackLines, float maxWidth)
        {
            float nextX = 0.0f;
            float totalHeight = measure.CalcHeight(value, maxWidth);

            if (stackLines != null)
            {
                for (int i = 0; i < stackLines.Length; ++i)
                {
                    float lineHeight = measure.CalcHeight(stackLines[i].line, maxWidth);
                    stackLines[i].frame = new Rect(nextX, totalHeight, maxWidth, lineHeight);

                    if (stackLines[i].sourcePathStart != -1)
                    {
                        CGUIStyleTextMeasure styleMeasure = measure as CGUIStyleTextMeasure;
                        if (styleMeasure != null)
                        {
                            ResolveSourceLink(styleMeasure, ref stackLines[i]);
                        }
                    }

                    totalHeight += lineHeight;
                }
            }

            this.width = maxWidth;
            this.height = totalHeight;
        }

        private static void ResolveSourceLink(CGUIStyleTextMeasure measure, ref CStackTraceLine stackLine)
        {
            Color color = CEditorSkin.GetColor(stackLine.sourcePathExists ? CColorCode.Link : CColorCode.LinkInnactive);

            int sourceStart = stackLine.sourcePathStart;
            int sourceEnd = stackLine.sourcePathEnd;

            GUIStyle style = measure.Style;
            GUIContent content = new GUIContent(stackLine.line);

            float startPosX = style.GetCursorPixelPosition(stackLine.frame, content, sourceStart).x - 1;
            float endPosX = style.GetCursorPixelPosition(stackLine.frame, content, sourceEnd).x + 1;

            stackLine.sourceFrame = new Rect(startPosX, stackLine.frame.y, endPosX - startPosX, stackLine.frame.height);
            stackLine.line = CStringUtils.C(stackLine.line, color, sourceStart, sourceEnd);
        }
        
        private bool HasFlag(Flags flag)
        {
            return (flags & flag) != 0;
        }

        public bool IsPlain
        {
            get { return HasFlag(Flags.Plain); }
        }

        public bool IsTable
        {
            get { return HasFlag(Flags.Table); }
        }

        public string[] Table
        {
            get { return data as string[]; }
        }
    }

    class CConsoleViewCell : CTableViewCell
    {
        public virtual bool OnMouseDown(CEvent evt)
        {
            return false;
        }

        public virtual bool OnMouseDoubleClick(CEvent evt)
        {
            return false;
        }

        public virtual bool OnMouseUp(CEvent evt)
        {
            return false;
        }

        public virtual bool OnKeyDown(CEvent evt)
        {
            return false;
        }

        public CConsoleView ConsoleView { get { return Table as CConsoleView; } }
    }

    class CConsoleTextEntryView : CConsoleViewCell
    {
        protected string m_value;

        protected override void DrawGUI()
        {
            GUIStyle style = CSharedStyles.consoleTextStyle;
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

        public override bool OnMouseDoubleClick(CEvent evt)
        {
            if (!string.IsNullOrEmpty(StackTrace))
            {
                if (CEditor.OpenFileExternal(StackTrace))
                {
                    return true;
                }
            }
            else if (this.LogLevel == CLogLevel.Error || this.LogLevel == CLogLevel.Warn)
            {
                CSourcePathEntry element;
                if (CEditorStackTrace.TryParseCompilerMessage(m_value, out element))
                {
                    if (CEditor.OpenFileAtLineExternal(element.sourcePath, element.lineNumber))
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

        public CLogLevel LogLevel
        {
            get;
            set;
        }

        public Color TextColor { get; set; }

        #endregion
    }

    class CConsoleTextEntryExceptionView : CConsoleTextEntryView
    {
        private CStackTraceLine[] m_stackTraceLines = CStackTraceLine.kEmptyLinesArray;

        protected override void DrawGUI()
        {
            BeginGroup(Frame);
            {
                GUIStyle style = CSharedStyles.consoleTextStyle;
                Color oldColor = style.normal.textColor;
                style.normal.textColor = this.TextColor;

                // title
                GUI.Label(Bounds, this.Value, style);

                // stack trace
                for (int i = 0; i < m_stackTraceLines.Length; ++i)
                {
                    DrawStackLine(ref m_stackTraceLines[i], style);
                }

                style.normal.textColor = oldColor;
            }
            EndGroup();
        }

        private void DrawStackLine(ref CStackTraceLine stackLine, GUIStyle style)
        {
            if (stackLine.IsClickable)
            {
                GUIStyle linkStyle = stackLine.sourcePathExists ? CSharedStyles.consoleLinkStyle : CSharedStyles.consoleLinkInnactiveStyle;
                CUIHelper.DrawUnderLine(stackLine.sourceFrame, linkStyle);

                if (stackLine.sourcePathExists && GUI.Button(stackLine.sourceFrame, GUIContent.none, GUIStyle.none))
                {
                    CEditor.OpenFileAtLineExternal(stackLine.sourcePath, stackLine.lineNumber);
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

        public CStackTraceLine[] StackTraceLines
        {
            get { return m_stackTraceLines; }
            set { m_stackTraceLines = value; }
        }

        #endregion
    }
}