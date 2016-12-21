//
//  CTerminal.cs
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
using System.Text;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class CTerminal : CAbstractConsole
    {
        public CTerminal(int capacity)
            : base(capacity)
        {
            History = new CTerminalHistory(100);
        }

        public virtual void Add(string line)
        {
            Add(new CConsoleViewCellEntry(line));
        }
        
        public virtual void Add(string[] lines)
        {
            Add(new CConsoleViewCellEntry(lines));
        }

        public virtual void Add(Exception e, string message)
        {
            Add(new CConsoleViewCellEntry(e, message));
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Autocomplete

        public string DoAutoComplete(string line, int index, bool isDoubleTab)
        {
            try
            {
                CTerminalAutocompletion.Result result = CTerminalAutocompletion.Autocomplete(line, index);

                if (isDoubleTab && result.suggestions != null)
                {
                    Add(CCommand.Prompt(line));
                    Add(result.suggestions);
                }

                return result.line;
            }
            catch (CCommandAutoCompleteException e)
            {
                Add(CCommand.Prompt(line));
                Add(e.InnerException, "Exception while auto completing args");
            }
            catch (Exception e)
            {
                Add(CCommand.Prompt(line));
                Add(e, "Inner command auto completion error");
            }

            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public CTerminalHistory History { get; private set; }

        #endregion
    }

    class CFormattedTerminal : CTerminal
    {
        public CFormattedTerminal(int capacity)
            : base(capacity)
        {
        }

        #region Inheritance

        public override void Add(string line)
        {
            base.Add(FormatLine(line));
        }

        public override void Add(string[] lines)
        {
            base.Add(FormatLines(lines));
        }

        #endregion

        #region Lines format
        
        private string FormatLine(string line)
        {
            return CEditorSkin.SetColors(line);
        }
        
        private string[] FormatLines(string[] lines)
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                lines[i] = CEditorSkin.SetColors(lines[i]);
            }
            return lines;
        }

        #endregion
    }

    class CTerminalHistory
    {
        private CCycleArray<string> m_entries;
        private int m_currentIndex;

        public CTerminalHistory(int capacity)
        {
            m_entries = new CCycleArray<string>(capacity);
            m_currentIndex = -1;
        }

        public string this[int index]
        {
            get
            {
                int entryIndex = m_entries.HeadIndex + index;
                return m_entries[entryIndex];
            }
        }

        public void Push(string line)
        {
            if (m_entries.Length == 0 || m_entries[m_entries.Length - 1] != line)
            {
                m_entries.Add(line);
            }

            Reset();
        }

        public void Reset()
        {
            m_currentIndex = m_entries.Length;
        }

        public string Next()
        {
            int nextIndex = m_currentIndex + 1;
            if (nextIndex < m_entries.Length)
            {
                m_currentIndex = nextIndex;
                return m_entries[m_currentIndex];
            }

            return null;
        }

        public string Prev()
        {
            int prevIndex = m_currentIndex - 1;
            if (prevIndex >= m_entries.HeadIndex)
            {
                m_currentIndex = prevIndex;
                return m_entries[m_currentIndex];
            }
            
            return null;
        }

        public void Clear()
        {
            m_currentIndex = -1;
            m_entries.Clear();
        }

        public int Count
        {
            get { return m_entries.RealLength; }
        }
    }
}