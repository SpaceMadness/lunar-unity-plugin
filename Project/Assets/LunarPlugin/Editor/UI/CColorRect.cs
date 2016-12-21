//
//  CColorRect.cs
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
    class CColorRect : CView
    {
        private Color m_color;
        private GUIStyle m_style;

        public CColorRect(Rect frame, Color color)
            : base(frame)
        {
            m_style = new GUIStyle();
            this.Color = color;
        }

        public override void OnGUI()
        {
            GUI.Box(Frame, GUIContent.none, m_style);
        }

        public Color Color
        {
            get { return m_color; }
            set
            {
                if (m_color != value)
                {
                    m_color = value;
                    m_style.normal.background = CUIHelper.Create1x1ColorTexture(m_color);
                }
            }
        }
    }
}
