using System;
using System.Collections.Generic;
using System.Globalization;

namespace LunarEditor
{
    interface IConsoleViewFilter
    {
        bool Apply(ref ConsoleViewCellEntry entry);
    }

    abstract class ConsoleViewFilterBase : IConsoleViewFilter
    {
        private int m_priority;

        public ConsoleViewFilterBase(int priority = int.MinValue)
        {
            m_priority = priority;
        }

        public abstract bool Apply(ref ConsoleViewCellEntry entry);

        public int Priority
        {
            get { return m_priority; }
        }
    }

    class ConsoleViewCompositeFilter : IConsoleViewFilter
    {
        private List<ConsoleViewFilterBase> m_filters;

        public ConsoleViewCompositeFilter()
        {
            m_filters = new List<ConsoleViewFilterBase>();
        }

        #region IConsoleViewFilter implementation

        public bool Apply(ref ConsoleViewCellEntry entry)
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

        public void AddFilter(ConsoleViewFilterBase filter)
        {
            if (filter == null)
            {
                throw new NullReferenceException("Filter is null");
            }

            for (int i = 0; i < m_filters.Count; ++i)
            {
                ConsoleViewFilterBase f = m_filters[i];
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

        public void RemoveFilter(ConsoleViewFilterBase filter)
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
