//
//  ToolBar.cs
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
using System.Collections;

namespace LunarEditor
{
    class ToolBar : View
    {
        public ToolBar(float width)
            : base(width, UISize.ToolbarHeight)
        {
            this.AutoresizeMask = ViewAutoresizing.FlexibleWidth;
        }

        public override void OnGUI()
        {
            GUILayout.BeginHorizontal(SharedStyles.toolbar, GUILayout.Width(this.Width));
            {
                DrawChildren();
            }
            GUILayout.EndHorizontal();
        }

        public ToolBarButton AddButton(string title, ButtonDelegate buttonDelegate)
        {
            ToolBarButton button = new ToolBarButton(title, buttonDelegate);
            AddSubview(button);
            return button;
        }

        public ToolBarLabel AddLabel(string text)
        {
            ToolBarLabel label = new ToolBarLabel(text);
            AddSubview(label);
            return label;
        }

        public ToolBarToggle AddToggle(string title, ToggleButtonDelegate buttonDelegate)
        {
            ToolBarToggle button = new ToolBarToggle(title, buttonDelegate);
            AddSubview(button);
            return button;
        }

        public ToolBarCheckbox AddCheckbox(string title, ToggleButtonDelegate buttonDelegate)
        {
            ToolBarCheckbox button = new ToolBarCheckbox(title, buttonDelegate);
            AddSubview(button);
            return button;
        }

        public ToolBarDropList AddDropList(string[] values, int selectedValue = 0)
        {
            return AddDropList(null, values, selectedValue);
        }

        public ToolBarDropList AddDropList(string title, string[] values, int selectedValue = 0)
        {
            ToolBarDropList list = new ToolBarDropList(title, values, selectedValue);
            AddSubview(list);
            return list;
        }

        public void AddSpace(float pixels = UISize.ToolbarSpacing)
        {
            AddSubview(new ToolBarSpace(pixels));
        }

        public void AddFlexibleSpace()
        {
            AddSubview(new ToolBarFlexibleSpace());
        }
    }

    class ToolBarButton : Button
    {
        public ToolBarButton(string title, ButtonDelegate buttonDelegate)
            : base(title, buttonDelegate)
        {
            Width = SharedStyles.MeasureWidth(SharedStyles.toolbarButton, title);
        }

        public override void OnGUI()
        {
            if (GUILayout.Button(Title, SharedStyles.toolbarButton, GUILayout.Width(Frame.width)))
            {
                if (ButtonDelegate != null)
                    ButtonDelegate(this);
            }
        }
    }

    class ToolBarLabel : View
    {
        private string m_text;

        public ToolBarLabel(string text)
            : this(text, EditorStyles.miniLabel)
        {
        }

        public ToolBarLabel(string text, GUIStyle style)
        {
            m_text = text;
            this.Style = style;
        }

        public override void OnGUI()
        {
            GUILayout.Label(m_text, this.Style);
        }

        public string Text
        {
            get { return m_text; }
            set { m_text = value != null ? value : string.Empty; }
        }
    }

    class ToolBarToggle : ToggleButton
    {
        public ToolBarToggle(string title, ToggleButtonDelegate buttonDelegate)
            : base(title, buttonDelegate)
        {
            Width = SharedStyles.MeasureWidth(SharedStyles.toolbarButton, title);
        }

        public override void OnGUI()
        {
            bool oldFlag = this.IsOn;
            this.IsOn = GUILayout.Toggle(oldFlag, Title, SharedStyles.toolbarButton, GUILayout.Width(Frame.width));
            if (oldFlag ^ this.IsOn)
            {
                if (ButtonDelegate != null)
                    ButtonDelegate(this);
            }
        }
    }

    class ToolBarCheckbox : ToggleButton
    {
        public ToolBarCheckbox(string title, ToggleButtonDelegate buttonDelegate)
            : base(title, buttonDelegate)
        {
            Width = SharedStyles.MeasureWidth(GUI.skin.toggle, title);
        }

        public override void OnGUI()
        {
            bool oldFlag = this.IsOn;
            this.IsOn = GUILayout.Toggle(this.IsOn, Title, GUI.skin.toggle, GUILayout.Width(Frame.width));
            if (oldFlag ^ this.IsOn)
            {
                if (ButtonDelegate != null)
                    ButtonDelegate(this);
            }
        }
    }

    delegate void ToolBarDropListDelegate(ToolBarDropList list);

    class ToolBarDropList : View
    {
        private string m_title;
        private int m_selectedValue;
        private string[] m_displayedOptions;
        private int[] m_optionsValues;

        private float m_titleWith;
        private float m_popupWidth;

        public ToolBarDropList(string[] displayedOptions, int selectedValue = 0)
            : this(null, displayedOptions, selectedValue)
        {
        }

        public ToolBarDropList(string title, string[] displayedOptions, int selectedValue = 0)
        {
            if (displayedOptions == null)
            {
                throw new ArgumentNullException("displayedOptions");
            }

            if (selectedValue < 0 || selectedValue > displayedOptions.Length)
            {
                throw new ArgumentOutOfRangeException("Selected value is out of range 0.." + (displayedOptions.Length - 1));
            }

            m_title = title;
            m_displayedOptions = displayedOptions;

            m_optionsValues = new int[displayedOptions.Length];
            for (int i = 0; i < m_optionsValues.Length; ++i)
            {
                m_optionsValues[i] = i;
            }
        }

        protected override GUIStyle CreateGUIStyle()
        {
            return EditorStyles.toolbarDropDown;
        }

        public override void OnGUI()
        {
            if (m_title != null)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUIStyle style = EditorStyles.miniLabel;

                    if (m_titleWith == 0)
                    {
                        m_titleWith = SharedStyles.MeasureWidth(style, m_title);
                    }

                    GUILayout.Label(m_title, style, GUILayout.Width(m_titleWith));
                    DrawPopup();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                DrawPopup();
            }
        }

        private void DrawPopup()
        {
            if (m_popupWidth == 0)
            {
                m_popupWidth = CalcPopupWidth(m_displayedOptions, this.Style);
            }

            int selectedValue = EditorGUILayout.IntPopup(m_selectedValue, m_displayedOptions, m_optionsValues, this.Style, GUILayout.Width(m_popupWidth));
            if (selectedValue != m_selectedValue)
            {
                m_selectedValue = selectedValue;
                if (Delegate != null)
                {
                    Delegate(this);
                }
            }
        }

        private float CalcPopupWidth(string[] options, GUIStyle style)
        {
            float width = 0;
            for (int i = 0; i < options.Length; ++i)
            {
                width = Mathf.Max(width, SharedStyles.MeasureWidth(style, options[i]));
            }

            return width;
        }

        public int SelectedValue
        {
            get { return m_selectedValue; }
            set
            {
                if (value < 0 || value > m_optionsValues.Length)
                {
                    throw new ArgumentOutOfRangeException("Selected value is out of range 0.." + (m_optionsValues.Length-1));
                }

                if (m_selectedValue != value)
                {
                    m_selectedValue = value;
                    Repaint();
                }
            }
        }

        public ToolBarDropListDelegate Delegate
        {
            get;
            set;
        }
    }

    class ToolBarSpace : View
    {
        public ToolBarSpace(float pixels)
            : base(pixels, 0)
        {
        }

        public override void OnGUI()
        {
            GUILayout.Label(GUIContent.none, GUIStyle.none, GUILayout.Width(this.Width));
        }
    }

    class ToolBarFlexibleSpace : View
    {
        public override void OnGUI()
        {
            GUILayout.Label(GUIContent.none, GUIStyle.none);
        }
    }
}