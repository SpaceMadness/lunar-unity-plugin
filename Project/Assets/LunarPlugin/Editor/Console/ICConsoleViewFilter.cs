//
//  ICConsoleViewFilter.cs
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

using System;
using System.Collections.Generic;
using System.Globalization;

namespace LunarEditor
{
    interface ICConsoleViewFilter
    {
        bool Apply(ref CConsoleViewCellEntry entry);
    }

    abstract class CConsoleViewFilterBase : ICConsoleViewFilter
    {
        private int m_priority;

        public CConsoleViewFilterBase(int priority = int.MinValue)
        {
            m_priority = priority;
        }

        public abstract bool Apply(ref CConsoleViewCellEntry entry);

        public int Priority
        {
            get { return m_priority; }
        }
    }

    class CConsoleViewCompositeFilter : ICConsoleViewFilter
    {
        private List<CConsoleViewFilterBase> m_filters;

        public CConsoleViewCompositeFilter()
        {
            m_filters = new List<CConsoleViewFilterBase>();
        }

        #region IConsoleViewFilter implementation

        public bool Apply(ref CConsoleViewCellEntry entry)
        {
            for (int i = 0; i < m_filters.Count; ++i)
            {
                if (!m_filters[i].Apply(ref entry))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Filters

        public void AddFilter(CConsoleViewFilterBase filter)
        {
            if (filter == null)
            {
                throw new NullReferenceException("Filter is null");
            }

            for (int i = 0; i < m_filters.Count; ++i)
            {
                CConsoleViewFilterBase f = m_filters[i];
                if (f.Priority < filter.Priority)
                {
                    m_filters.Insert(i, filter);
                    return;
                }

                if (f == filter)
                {
                    return;
                }
            }

            m_filters.Add(filter);
        }

        public void RemoveFilter(CConsoleViewFilterBase filter)
        {
            m_filters.Remove(filter);
        }

        public bool HasFilters
        {
            get { return m_filters.Count > 0; }
        }

        #endregion

    }
}
