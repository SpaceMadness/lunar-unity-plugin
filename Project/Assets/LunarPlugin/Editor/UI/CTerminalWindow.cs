//
//  TerminalWindow.cs
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

using System;
using System.Text;
using System.Collections.Generic;

using LunarPluginInternal;

namespace LunarEditor
{
    class TerminalWindow : Window, ITerminalCompositeViewDelegate
    {
        public TerminalWindow()
            : base("Terminal")
        {
            this.minSize = new Vector2(320, 240);
        }

        //////////////////////////////////////////////////////////////////////////////

        protected override void CreateUI()
        {
            TerminalCompositeView terminalView = new TerminalCompositeView(this, this.Width, this.Height);
            AddSubview(terminalView);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region ITerminalCompositeViewDelegate

        public void ExecCommand(string commandLine)
        {
            EditorApp.ExecCommand(commandLine, true);
        }

        public Terminal Terminal
        {
            get { return EditorApp.Terminal; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
        
        #region Menu item
        
        internal static void ShowWindow()
        {
            EditorWindow.GetWindow<TerminalWindow>();
        }

        #endregion
    }
}