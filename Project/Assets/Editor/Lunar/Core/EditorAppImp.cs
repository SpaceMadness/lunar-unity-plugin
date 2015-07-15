using System;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class EditorAppImp : DefaultAppImp
    {
        private readonly Terminal m_terminal;

        public EditorAppImp()
        {
            m_terminal = CreateTerminal(CVars.c_historySize.IntValue);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Terminal

        public override void LogTerminal(string line)
        {
            m_terminal.Add(line);
        }

        public override void LogTerminal(string[] table)
        {
            m_terminal.Add(table);
        }

        public override void LogTerminal(Exception e, string message)
        {
            m_terminal.Add(e, message);
        }

        public override void ClearTerminal()
        {
            m_terminal.Clear();
        }

        public override bool IsPromptEnabled
        {
            get { return true; }
        }

        protected virtual Terminal CreateTerminal(int capacity)
        {
            return new FormattedTerminal(capacity);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public Terminal Terminal
        {
            get { return m_terminal; }
        }

        #endregion
    }
}

