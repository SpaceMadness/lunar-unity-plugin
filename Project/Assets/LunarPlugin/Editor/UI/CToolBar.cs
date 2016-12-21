//
//  CToolBar.cs
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
    class CToolBar : CView
    {
        public CToolBar(float width)
            : base(width, CUISize.ToolbarHeight)
        {
            this.AutoresizeMask = CViewAutoresizing.FlexibleWidth;
        }

        public override void OnGUI()
        {
            GUILayout.BeginHorizontal(CSharedStyles.toolbar, GUILayout.Width(this.Width));
            {
                DrawChildren();
            }
            GUILayout.EndHorizontal();
        }

        public CToolBarButton AddButton(string title, CButtonDelegate buttonDelegate)
        {
            CToolBarButton button = new CToolBarButton(title, buttonDelegate);
            AddSubview(button);
            return button;
        }

        public CToolBarLabel AddLabel(string text)
        {
            CToolBarLabel label = new CToolBarLabel(text);
            AddSubview(label);
            return label;
        }

        public CToolBarToggle AddToggle(string title, CToggleButtonDelegate buttonDelegate)
        {
            CToolBarToggle button = new CToolBarToggle(title, buttonDelegate);
            AddSubview(button);
            return button;
        }

        public CToolBarCheckbox AddCheckbox(string title, CToggleButtonDelegate buttonDelegate)
        {
            CToolBarCheckbox button = new CToolBarCheckbox(title, buttonDelegate);
            AddSubview(button);
            return button;
        }

        public CToolBarDropList AddDropList(string[] values, int selectedValue = 0)
        {
            return AddDropList(null, values, selectedValue);
        }

        public CToolBarDropList AddDropList(string title, string[] values, int selectedValue = 0)
        {
            CToolBarDropList list = new CToolBarDropList(title, values, selectedValue);
            AddSubview(list);
            return list;
        }

        public void AddSpace(float pixels = CUISize.ToolbarSpacing)
        {
            AddSubview(new CToolBarSpace(pixels));
        }

        public void AddFlexibleSpace()
        {
            AddSubview(new CToolBarFlexibleSpace());
        }
    }

    class CToolBarButton : CButton
    {
        public CToolBarButton(string title, CButtonDelegate buttonDelegate)
            : base(title, buttonDelegate)
        {
            Width = CSharedStyles.MeasureWidth(CSharedStyles.toolbarButton, title);
        }

        public override void OnGUI()
        {
            if (GUILayout.Button(Title, CSharedStyles.toolbarButton, GUILayout.Width(Frame.width)))
            {
                if (ButtonDelegate != null)
                    ButtonDelegate(this);
            }
        }
    }

    class CToolBarLabel : CView
    {
        private string m_text;

        public CToolBarLabel(string text)
            : this(text, EditorStyles.miniLabel)
        {
        }

        public CToolBarLabel(string text, GUIStyle style)
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

    class CToolBarToggle : CToggleButton
    {
        public CToolBarToggle(string title, CToggleButtonDelegate buttonDelegate)
            : base(title, buttonDelegate)
        {
            Width = CSharedStyles.MeasureWidth(CSharedStyles.toolbarButton, title);
        }

        public override void OnGUI()
        {
            bool oldFlag = this.IsOn;
            this.IsOn = GUILayout.Toggle(oldFlag, Title, CSharedStyles.toolbarButton, GUILayout.Width(Frame.width));
            if (oldFlag ^ this.IsOn)
            {
                if (ButtonDelegate != null)
                    ButtonDelegate(this);
            }
        }
    }

    class CToolBarCheckbox : CToggleButton
    {
        public CToolBarCheckbox(string title, CToggleButtonDelegate buttonDelegate)
            : base(title, buttonDelegate)
        {
            Width = CSharedStyles.MeasureWidth(GUI.skin.toggle, title);
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

    delegate void CToolBarDropListDelegate(CToolBarDropList list);

    class CToolBarDropList : CView
    {
        private string m_title;
        private int m_selectedValue;
        private string[] m_displayedOptions;
        private int[] m_optionsValues;

        private float m_titleWith;
        private float m_popupWidth;

        public CToolBarDropList(string[] displayedOptions, int selectedValue = 0)
            : this(null, displayedOptions, selectedValue)
        {
        }

        public CToolBarDropList(string title, string[] displayedOptions, int selectedValue = 0)
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
                        m_titleWith = CSharedStyles.MeasureWidth(style, m_title);
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
                width = Mathf.Max(width, CSharedStyles.MeasureWidth(style, options[i]));
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

        public CToolBarDropListDelegate Delegate
        {
            get;
            set;
        }
    }

    class CToolBarSpace : CView
    {
        public CToolBarSpace(float pixels)
            : base(pixels, 0)
        {
        }

        public override void OnGUI()
        {
            GUILayout.Label(GUIContent.none, GUIStyle.none, GUILayout.Width(this.Width));
        }
    }

    class CToolBarFlexibleSpace : CView
    {
        public override void OnGUI()
        {
            GUILayout.Label(GUIContent.none, GUIStyle.none);
        }
    }
}