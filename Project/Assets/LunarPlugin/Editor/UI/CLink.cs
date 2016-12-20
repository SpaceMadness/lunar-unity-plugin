//
//  Link.cs
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

using UnityEngine;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class Link : View
    {
        private GUIContent m_content;
        private string m_href;

        public Link(string text, string href)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (href == null)
            {
                throw new ArgumentNullException("href");
            }

            m_content = new GUIContent(StringUtils.NonNullOrEmpty(text));
            m_href = href;

            Vector2 size = this.Style.CalcSize(m_content);
            this.Frame = new Rect(0, 0, size.x, size.y);
        }

        protected override GUIStyle CreateGUIStyle()
        {
            return SharedStyles.linkTextStyle;
        }

        protected override void DrawGUI()
        {
            BeginGroup(Frame);
            {
                if (GUI.Button(this.Bounds, m_content, this.Style))
                {
                    EditorApp.HandleURL(m_href);
                }
            }
            EndGroup();

            #if !(UNITY_4_7 || UNITY_4_6 || UNITY_4_5 || UNITY_4_4 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4)
            Rect linkFrame = this.Frame;
            linkFrame.height = this.Style.font.ascent;
            UIHelper.DrawUnderLine(linkFrame, this.Style.normal.textColor);
            #endif
        }
    }
}

