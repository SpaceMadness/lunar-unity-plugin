//
//  UIHelper.cs
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

namespace LunarEditor
{
    static class UISize
    {
        public const float ToolbarHeight = 17;
        public const float ToolbarSpacing = 6;

        public const float ButtonHeight = 18;
        public const float ButtonBorder = 7;

        public const float TextFieldWidth = 120;
        public const float TextFieldHeight = 18;

        public const float ScrollBarWidth = 16;

        public const float ResizeCornerWidth = 12;
        public const float ResizeCornerHeight = 12;

        public const float CVarEntryWidth = 270;
    }

    static class UIHelper
    {
        private static Dictionary<Color, Texture2D> m_texturesLookup;

        static UIHelper()
        {
            m_texturesLookup = new Dictionary<Color, Texture2D>();
            m_texturesLookup[Color.clear] = null;
        }

        public static void RecycleTextures()
        {
            m_texturesLookup.Clear();
            m_texturesLookup[Color.clear] = null;
        }

        public static Texture2D Create1x1ColorTexture(Color color)
        {
            Texture2D texture;
            if (!m_texturesLookup.TryGetValue(color, out texture))
            {
                texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, color);
                texture.Apply();

                m_texturesLookup[color] = texture;
            }

            return texture;
        }

        public static void DrawRect(Rect rect, GUIStyle style)
        {
            GUI.Box(rect, GUIContent.none, style);
        }

        public static void DrawLine(float x, float y, float length, Color color)
        {
            Rect pos = new Rect(x, y, length, 0.5f);
            Texture2D tex = Create1x1ColorTexture(color);
            GUI.DrawTexture(pos, tex);
        }

        public static void DrawUnderLine(Rect rect, Color color)
        {
            Rect pos = new Rect(rect.x, rect.y + rect.height, rect.width, 0.5f);
            Texture2D tex = Create1x1ColorTexture(color);
            GUI.DrawTexture(pos, tex);
        }

        public static void DrawUnderLine(Rect rect, GUIStyle style)
        {
            #if !(UNITY_4_7 || UNITY_4_6 || UNITY_4_5 || UNITY_4_4 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4)
            Rect pos = new Rect(rect.x, rect.y + style.font.ascent, rect.width, 0.5f);
            Texture2D tex = Create1x1ColorTexture(style.normal.textColor);
            GUI.DrawTexture(pos, tex);
            #endif
        }
    }

    class UnityEvent : Event
    {
        private UnityEngine.Event m_internalEvent;

        protected override Event CurrentEvent
        {
            get
            {
                m_internalEvent = UnityEngine.Event.current;
                return m_internalEvent != null ? this : null;
            }
        }

        public override void Use()
        {
            if (m_internalEvent != null)
            {
                m_internalEvent.Use();
            }
        }

        public override EventType type
        {
            get { return m_internalEvent.type; }
            set { m_internalEvent.type = value; }
        }

        public override bool isMouse
        {
            get { return m_internalEvent.isMouse; }
        }

        public override bool isKey
        {
            get { return m_internalEvent.isKey; }
        }

        public override KeyCode keyCode
        {
            get { return m_internalEvent.keyCode; }
            set { m_internalEvent.keyCode = value; }
        }

        public override char character
        {
            get { return m_internalEvent.character; }
            set { m_internalEvent.character = value; }
        }

        public override bool control
        {
            get { return m_internalEvent.control; }
            set { m_internalEvent.control = value; }
        }

        public override bool shift
        {
            get { return m_internalEvent.shift; }
            set { m_internalEvent.shift = value; }
        }

        public override bool alt
        {
            get { return m_internalEvent.alt; }
            set { m_internalEvent.alt = value; }
        }

        public override bool command
        {
            get { return m_internalEvent.command; }
            set { m_internalEvent.command = value; }
        }

        public override Vector2 mousePosition
        {
            get { return m_internalEvent.mousePosition; }
            set { m_internalEvent.mousePosition = value; }
        }

        public override int button
        {
            get { return m_internalEvent.button; }
            set { m_internalEvent.button = value; }
        }

        public override int clickCount
        {
            get { return m_internalEvent.clickCount; }
            set { m_internalEvent.clickCount = value; }
        }

        protected override void DestroyEvent()
        {
            m_internalEvent = null;
        }
    }
}