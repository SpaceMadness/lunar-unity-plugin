//
//  CConsoleFilteredDelegate.cs
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

﻿using System;
using System.Collections.Generic;
using System.Globalization;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class CConsoleFilteredDelegate : CConsoleViewCompositeFilter, ICTableViewDataSource, ICTableViewDelegate, ICConsoleDelegate
    {
        private CConsoleView m_consoleView;

        private CCycleArray<int> m_filteredIndices;

        private CConsoleViewLogLevelFilter m_levelFilter;
        private CConsoleViewTagFilter m_tagFilter;
        private CConsoleViewTextFilter m_textFilter;

        private int m_oldConsoleEntriesHeadIndex;

        public CConsoleFilteredDelegate(CConsoleView consoleView)
        {
            m_consoleView = consoleView;
            m_oldConsoleEntriesHeadIndex = Entries.HeadIndex;

            m_filteredIndices = new CCycleArray<int>(consoleView.Entries.Capacity);
        }

        public bool SetFilterLogLevel(CLogLevel level)
        {
            if (level != null && level != CLogLevel.Verbose) // no need to filter on verbose
            {
                if (m_levelFilter == null)
                {
                    m_levelFilter = new CConsoleViewLogLevelFilter(level);
                    bool shouldAppend = this.HasFilters; // not the first filter

                    AddFilter(m_levelFilter);

                    return shouldAppend ? AppendFilter(m_levelFilter) : ApplyFilter(m_levelFilter);
                }

                CLogLevel oldLevel = m_levelFilter.Level;
                m_levelFilter.Level = level;

                if (oldLevel.Priority < level.Priority) // can only decrease filtered items count: e.g. 'debug' -> 'error'
                {
                    return AppendFilter(m_levelFilter);
                }

                if (oldLevel.Priority > level.Priority) // can increase filtered items count: e.g. 'error' -> 'debug'
                {
                    return ApplyFilter(this);
                }
            }
            else if (m_levelFilter != null) // remove existing filter
            {
                RemoveFilter(m_levelFilter);
                m_levelFilter = null;

                return ApplyFilter(this);
            }

            return false;
        }

        public bool AddFilterTags(params CTag[] tags)
        {
            if (tags == null)
            {
                throw new NullReferenceException("Tags reference is null");
            }

            if (m_tagFilter == null)
            {
                m_tagFilter = new CConsoleViewTagFilter();
                AddFilter(m_tagFilter);
            }

            bool changed = false;
            foreach (CTag tag in tags)
            {
                changed |= m_tagFilter.AddTag(tag);
            }

            return changed && ApplyFilter(this); // can increase filtered items count
        }

        public bool SetFilterTags(params CTag[] tags)
        {
            if (tags == null)
            {
                throw new NullReferenceException("Tags reference is null");
            }

            if (tags.Length == 0)
            {
                RemoveFilter(m_tagFilter);
                m_tagFilter = null;

                return ApplyFilter(this);
            }

            if (m_tagFilter == null)
            {
                m_tagFilter = new CConsoleViewTagFilter();
                AddFilter(m_tagFilter);
            }

            m_tagFilter.SetTags(tags);
            return ApplyFilter(this); // can increase filtered items count
        }

        public bool RemoveFilterTags(params CTag[] tags)
        {
            if (tags == null)
            {
                throw new NullReferenceException("Tags reference is null");
            }

            if (m_tagFilter != null)
            {
                bool changed = false;
                foreach (CTag tag in tags)
                {
                    changed |= m_tagFilter.RemoveTag(tag);
                }

                if (changed)
                {
                    if (!m_tagFilter.HasTags) // no tags left
                    {
                        RemoveFilter(m_tagFilter);
                        m_tagFilter = null;
                    }

                    return ApplyFilter(this);
                }
            }

            return false;
        }

        public bool SetFilterText(string filterText)
        {
            if (string.IsNullOrEmpty(filterText))
            {
                if (m_textFilter != null)
                {
                    RemoveFilter(m_textFilter);
                    m_textFilter = null;

                    return ApplyFilter(this);
                }
            }
            else
            {
                if (m_textFilter != null)
                {
                    string oldText = m_textFilter.Text;
                    m_textFilter.Text = filterText;

                    if (filterText.StartsWith(oldText)) // can only decrease filtered items count: e.g. 'tex' -> 'text'
                    {
                        return AppendFilter(m_textFilter);
                    }

                    return ApplyFilter(this);
                }

                m_textFilter = new CConsoleViewTextFilter(filterText);
                bool shouldAppend = this.HasFilters; // not the first filter

                AddFilter(m_textFilter);

                return shouldAppend ? AppendFilter(m_textFilter) : ApplyFilter(m_textFilter);
            }

            return false;
        }

        private bool ApplyFilter(ICConsoleViewFilter filter)
        {
            int oldIndicesCount = m_filteredIndices.RealLength;
            ClearIndices();

            CConsoleViewCellEntry[] entriesArray = Entries.InternalArray;
            for (int entryIndex = Entries.HeadIndex; entryIndex < Entries.Length; ++entryIndex)
            {
                int entryArrayIndex = Entries.ToArrayIndex(entryIndex);
                if (filter.Apply(ref entriesArray[entryArrayIndex]))
                {
                    m_filteredIndices.Add(entryIndex);
                }
            }

            // TODO: check filtered indices values (indices count can remain the same but lines can change)
            m_oldConsoleEntriesHeadIndex = Entries.HeadIndex;

            return m_filteredIndices.RealLength != oldIndicesCount ||
                m_filteredIndices.RealLength != Entries.RealLength;
        }

        private bool AppendFilter(ICConsoleViewFilter filter)
        {
            CConsoleViewCellEntry[] entriesArray = Entries.InternalArray;
            int toIndex = m_filteredIndices.HeadIndex;
            for (int fromIndex = toIndex; fromIndex < m_filteredIndices.Length; ++fromIndex)
            {
                int entryIndex = m_filteredIndices[fromIndex];
                int entryArrayIndex = Entries.ToArrayIndex(entryIndex);
                if (filter.Apply(ref entriesArray[entryArrayIndex]))
                {
                    m_filteredIndices[toIndex] = entryIndex;
                    ++toIndex;
                }
            }

            // TODO: check filtered indices values (indices count can remain the same but lines can change)
            m_oldConsoleEntriesHeadIndex = Entries.HeadIndex;

            if (toIndex != m_filteredIndices.Length)
            {
                m_filteredIndices.TrimToLength(toIndex);
                return true;
            }

            return false;
        }

        private void ClearIndices()
        {
            m_filteredIndices.Clear();
        }

        #region ITableViewDataSource implementation

        public CTableViewCell TableCellForRow(CTableView table, int rowIndex)
        {
            int index = ToTableRowIndex(rowIndex);
            return m_consoleView.TableCellForRow(table, index);
        }

        public int NumberOfRows(CTableView table)
        {
            return m_filteredIndices.Length;
        }

        #endregion

        #region ITableViewDelegate implementation

        public float HeightForTableCell(int rowIndex)
        {
            int index = ToTableRowIndex(rowIndex);
            return m_consoleView.HeightForTableCell(index);
        }

        public void OnTableCellSelected(CTableView table, int rowIndex)
        {
            int index = ToTableRowIndex(rowIndex);
            m_consoleView.OnTableCellSelected(table, index);
        }

        public void OnTableCellDeselected(CTableView table, int rowIndex)
        {
            int index = ToTableRowIndex(rowIndex);
            m_consoleView.OnTableCellDeselected(table, index);
        }

        #endregion

        #region IConsoleDelegate implementation

        public void OnConsoleEntryAdded(CAbstractConsole console, ref CConsoleViewCellEntry entry)
        {
            CCycleArray<CConsoleViewCellEntry> Entries = this.Entries;

            // if unfiltered console entries overflow - we need to adjust indices and visible lines
            if (m_oldConsoleEntriesHeadIndex < Entries.HeadIndex)
            {
                m_oldConsoleEntriesHeadIndex = Entries.HeadIndex;

                int indicesHeadIndex = m_filteredIndices.HeadIndex;
                while(indicesHeadIndex < m_filteredIndices.Length && m_filteredIndices[indicesHeadIndex] < Entries.HeadIndex)
                {
                    ++indicesHeadIndex;
                }

                m_filteredIndices.TrimToHeadIndex(indicesHeadIndex);
                m_consoleView.TrimCellsToHead(indicesHeadIndex);
            }

            int entryIndex = Entries.Length - 1;
            int entryArrayIndex = Entries.ToArrayIndex(entryIndex);
            if (Apply(ref Entries.InternalArray[entryArrayIndex]))
            {
                m_filteredIndices.Add(entryIndex);
                m_consoleView.OnConsoleEntryAdded(console, ref entry);
            }
        }

        public void OnConsoleCleared(CAbstractConsole console)
        {
            ClearIndices();
            m_consoleView.OnConsoleCleared(console);
        }

        #endregion

        #region Helpers

        private int ToTableRowIndex(int index)
        {
            return m_filteredIndices[index];
        }

        #endregion

        #region Properties

        private CCycleArray<CConsoleViewCellEntry> Entries
        {
            get { return m_consoleView.Entries; }
        }

        #endregion
    }

    class CConsoleViewTextFilter : CConsoleViewFilterBase
    {
        private string m_text;

        public CConsoleViewTextFilter(string text)
            : base(0)
        {
            this.Text = text;
        }

        public override bool Apply(ref CConsoleViewCellEntry entry)
        {
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(entry.value, m_text, CompareOptions.IgnoreCase) != -1;
        }

        public string Text
        {
            get { return m_text; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("text");
                }
                m_text = value;
            }
        }
    }

    class CConsoleViewTagFilter : CConsoleViewFilterBase
    {
        private HashSet<CTag> m_tags;

        public CConsoleViewTagFilter()
            : base(1)
        {
            m_tags = new HashSet<CTag>();
        }

        public override bool Apply(ref CConsoleViewCellEntry entry)
        {
            return entry.tag != null && m_tags.Contains(entry.tag);
        }

        public bool SetTags(params CTag[] tags)
        {
            if (tags == null)
            {
                throw new ArgumentNullException("tags");
            }

            m_tags.Clear();

            foreach (CTag tag in tags)
            {
                AddTag(tag);
            }

            return true;
        }

        public bool AddTag(CTag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            return m_tags.Add(tag);
        }

        public bool RemoveTag(CTag tag)
        {
            return m_tags.Remove(tag);
        }

        public bool HasTags
        {
            get { return m_tags.Count > 0; }
        }
    }

    class CConsoleViewLogLevelFilter : CConsoleViewFilterBase
    {
        private CLogLevel m_level;

        public CConsoleViewLogLevelFilter(CLogLevel level)
            : base(2)
        {
            this.Level = level;
        }

        public override bool Apply(ref CConsoleViewCellEntry entry)
        {
            return entry.level != null && entry.level.Priority >= m_level.Priority;
        }

        public CLogLevel Level
        {
            get { return m_level; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("level");
                }

                m_level = value;
            }
        }
    }
}

