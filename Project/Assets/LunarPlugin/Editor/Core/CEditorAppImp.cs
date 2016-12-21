//
//  CEditorAppImp.cs
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

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class CEditorAppImp : CDefaultAppImp
    {
        private readonly CTerminal m_terminal;

        public CEditorAppImp()
        {
            m_terminal = CreateTerminal(CSystemVars.c_historySize.IntValue);
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

        protected virtual CTerminal CreateTerminal(int capacity)
        {
            return new CFormattedTerminal(capacity);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public CTerminal Terminal
        {
            get { return m_terminal; }
        }

        #endregion
    }
}

