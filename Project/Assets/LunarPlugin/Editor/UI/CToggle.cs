//
//  CToggle.cs
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
using System.Collections;

namespace LunarEditor
{
    delegate void CToggleChangeDelegate(CToggle toggle);

    class CToggle : CView 
    {
        private GUIContent m_content;

        public CToggle(string title, bool isOn = false)
        {
            m_content = new GUIContent(title);
            IsOn = isOn;

            Vector2 size = Style.CalcSize(m_content);
            Width = size.x;
            Height = size.y;
        }

        public override void OnGUI()
        {
            bool oldFlag = IsOn;
            IsOn = GUI.Toggle(Frame, IsOn, m_content);
            if ((oldFlag ^ IsOn) && Delegate != null)
            {
                Delegate(this);
            }
        }

        protected override GUIStyle CreateGUIStyle()
        {
            return new GUIStyle("toggle");
        }

        #region Properties

        public bool IsOn { get; set; }
        public CToggleChangeDelegate Delegate { get; set; }

        #endregion
    }
}