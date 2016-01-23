//
//  View.cs
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
using System.Collections.Generic;

using LunarPluginInternal;

namespace LunarEditor
{
    enum ViewAutoresizing
    {
        None                 = 0,
        FlexibleLeftMargin   = 1 << 0,
        FlexibleWidth        = 1 << 1,
        FlexibleRightMargin  = 1 << 2,
        FlexibleTopMargin    = 1 << 3,
        FlexibleHeight       = 1 << 4,
        FlexibleBottomMargin = 1 << 5
    }

    class View : ObjectsPoolEntry, IDisposable
    {
        private static readonly Rect EmptyRect = new Rect(0, 0, 0, 0);

        internal delegate bool EventDelegate(View view, Event evt);

        public const float AlignMin = 0.0f;
        public const float AlignCenter = 0.5f;
        public const float AlignMax = 1.0f;

        private static readonly List<View> m_emptyList = new List<View>(0);

        private List<View> m_subviews;
        private GUIStyle m_style;

        private Vector2 m_origin;
        private Rect m_bounds = EmptyRect;

        private Color m_backColor = Color.clear;

        private static int m_nextFocusableViewId;
        private string m_focusableViewId;

        public View(float width, float height) 
            : this(0, 0, width, height)
        {
        }

        public View(float x, float y, float width, float height)
            : this(new Rect(x, y, width, height))
        {   
        }

        public View(Rect frame) 
            : this()
        {
            this.Frame = frame;
        }

        public View()
        {
            m_subviews = m_emptyList;
            this.IsKeyEventsEnabled = true;
            this.IsEnabled = true;
        }

        public virtual void OnGUI()
        {
            HandleEvent();
            // TODO: check event type
            DrawGUI();
        }

        protected virtual void DrawGUI()
        {
            BeginGroup(Frame);
            {
                DrawBackgroud();
                DrawChildren();
            }
            EndGroup();
        }

        protected virtual void BeginGroup(Rect frame)
        {
            GUI.BeginGroup(frame);
        }

        protected virtual void EndGroup()
        {
            GUI.EndGroup();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Events

        protected bool HandleEvent()
        {
            if (Event.current != null)
            {
                if (IsFocusable)
                {
                    SetFocusableControlName();
                }

                return HandleEvent(Event.current);
            }

            return false;
        }

        protected virtual bool HandleEvent(Event evt)
        {
            if (evt.isMouse)
            {
                if (IsMouseEventsEnabled && ContainsPoint(evt.mousePosition.x, evt.mousePosition.y))
                {
                    Vector2 oldPos = evt.mousePosition;
                    evt.mousePosition = new Vector2(oldPos.x - X, oldPos.y - Y);
                    bool result = HandleMouseEvent(evt);
                    evt.mousePosition = oldPos;

                    return result;
                }

                return false;
            }

            if (evt.isKey && IsFocused && IsKeyEventsEnabled)
            {
                if (HandleKeyEvent(evt))
                {
                    evt.Use();
                    return true; // don't let other controls use it
                }

                return false;
            }

            return false;
        }

        private bool HandleMouseEvent(Event evt)
        {
            if (MouseEvent != null)
            {
                MouseEvent(this, evt);
            }

            switch (evt.type)
            {
                case EventType.MouseDown:
                {
                    if (MouseDown != null)
                    {
                        MouseDown(this, evt);
                    }

                    if (evt.clickCount == 2 && MouseDoubleClick != null)
                    {
                        MouseDoubleClick(this, evt);
                    }
                    break;
                }

                case EventType.MouseMove:
                {
                    if (MouseMove != null)
                    {
                        MouseMove(this, evt);
                    }
                    break;
                }

                case EventType.MouseDrag:
                {
                    if (MouseDrag != null)
                    {
                        MouseDrag(this, evt);
                    }
                    break;
                }

                case EventType.MouseUp:
                {
                    if (MouseUp != null)
                    {
                        MouseUp(this, evt);
                    }
                    break;
                }
            }

            return true;
        }

        private bool HandleKeyEvent(Event evt)
        {
            switch (evt.type)
            {
                case EventType.KeyDown:
                {
                    if (KeyDown != null)
                    {
                        return KeyDown(this, evt);
                    }
                    break;
                }

                case EventType.KeyUp:
                {
                    if (KeyUp != null)
                    {
                        return KeyUp(this, evt);
                    }
                    break;
                }
            }

            return false;
        }

        public bool ContainsPoint(float pointX, float pointY)
        {
            return pointY >= Y && pointY <= Y + Height &&
                   pointX >= X && pointX <= X + Width;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Subviews

        public View AddSubview(View view)
        {
            if (m_subviews == m_emptyList)
            {
                m_subviews = new List<View>(1);
            }
            m_subviews.Add(view);
            view.ParentView = this;

            return view;
        }

        public void RemoveSubview(View view)
        {
            if (view.ParentView == this)
            {
                m_subviews.Remove(view);
                view.ParentView = null;
            }
        }

        public void RemoveSubviews()
        {
            for (int i = 0; i < m_subviews.Count; ++i)
            {
                m_subviews[i].ParentView = null;
            }
            m_subviews.Clear();
        }

        public void RemoveFromParentView()
        {
            if (ParentView != null)
            {
                ParentView.RemoveSubview(this);
            }
        }

        public View FindSubview(object tag)
        {
            for (int i = 0; i < m_subviews.Count; ++i)
            {
                if (m_subviews[i].Tag == tag)
                {
                    return m_subviews[i];
                }
            }

            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Alignment

        public void AlignX(float align)
        {
            X = align * (ParentView.Width - Width);
        }

        public void AlignY(float align)
        {
            Y = align * (ParentView.Height - Height);
        }

        public void Align(float alignX, float alignY)
        {
            AlignX(alignX);
            AlignY(alignY);
        }

        public View AlignTop(float indent = 0.0f)
        {
            Y = indent;
            return this;
        }

        public View AlignBottom(float indent = 0.0f)
        {
            Y = ParentView.Height - Height - indent;
            return this;
        }

        public View AlignLeft(float indent = 0.0f)
        {
            X = indent;
            return this;
        }

        public View AlignRight(float indent = 0.0f)
        {
            X = ParentView.Width - Width - indent;
            return this;
        }

        public void AttachAbove(View other, float indent = 0.0f)
        {
            Y = other.Y - Height - indent;
        }

        public void AttachUnder(View other, float indent = 0.0f)
        {
            Y = other.Y + other.Height + indent;
        }

        public void AttachLeft(View other, float indent = 0.0f)
        {
            X = other.X - Width - indent;
        }

        public void AttachRight(View other, float indent = 0.0f)
        {
            X = other.X + other.Width + indent;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Sizing

        public void Resize(float width, float height)
        {
            float dx = width - this.Width;
            float dy = height - this.Height;

            if (dx != 0 || dy != 0)
            {
                this.Width = width;
                this.Height = height;

                OnResize(dx, dy);
            }
        }

        public void ResizeToFitSubviews()
        {
            float newWidth = 0;
            float newHeight = 0;

            for (int i = 0; i < m_subviews.Count; ++i)
            {
                View v = m_subviews[i];
                newWidth = Mathf.Max(newWidth, v.X + v.Width);
                newHeight = Mathf.Max(newHeight, v.Y + v.Height);
            }

            Width = newWidth;
            Height = newHeight;
        }

        public void ArrangeVert(float indent = 0.0f)
        {
            float nextY = 0;
            foreach (View view in Subviews)
            {
                view.Y = nextY;
                nextY += view.Height + indent;
            }
        }

        public void ArrangeHort(float indent = 0.0f)
        {
            float nextX = 0;
            foreach (View view in Subviews)
            {
                view.X = nextX;
                nextX += view.Width + indent;
            }
        }

        protected virtual void OnResize(float dx, float dy)
        {
            for (int i = 0; i < m_subviews.Count; ++i)
            {
                View subview = m_subviews[i];
                ViewAutoresizing mask = subview.AutoresizeMask;

                if (mask == ViewAutoresizing.None)
                {
                    continue;
                }

                float dw = 0;
                float dh = 0;

                if ((mask & ViewAutoresizing.FlexibleWidth) != 0)
                {
                    dw = dx;
                    subview.Width += dw;
                }
                else if ((mask & ViewAutoresizing.FlexibleLeftMargin) != 0)
                {
                    subview.X += dx;
                }

                if ((mask & ViewAutoresizing.FlexibleHeight) != 0)
                {
                    dh = dy;
                    subview.Height += dh;
                }
                else if ((mask & ViewAutoresizing.FlexibleTopMargin) != 0)
                {
                    subview.Y += dy;
                }

                if (dx != 0 || dy != 0)
                {
                    subview.OnResize(dw, dh);
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void Translate(float dx, float dy)
        {
            X += dx;
            Y += dy;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Inheritance

        protected virtual GUIStyle CreateGUIStyle()
        {
            return new GUIStyle();
        }

        protected virtual void DrawBackgroud()
        {
            if (m_style != null)
            {
                GUI.Box(m_bounds, GUIContent.none, m_style);
            }
        }

        protected void DrawChildren()
        {
            if (m_subviews.Count > 0)
            {
                for (int i = 0; i < m_subviews.Count; ++i)
                {
                    View subview = m_subviews[i];
                    if (!subview.IsEnabled && GUI.enabled)
                    {
                        GUI.enabled = false;
                        try
                        {
                            subview.OnGUI();
                        }
                        finally
                        {
                            GUI.enabled = true;
                        }
                    }
                    else
                    {
                        subview.OnGUI();
                    }
                }
            }
        }

        public virtual void Repaint()
        {
            View rootView = RootView;
            if (rootView != null)
            {
                rootView.Repaint();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Focus

        public virtual void FocusControl()
        {
            if (m_focusableViewId != null)
            {
                GUI.FocusControl(m_focusableViewId);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IDisposable

        public void Dispose()
        {
            NotificationCenter.UnregisterNotifications(this);
            TimerManager.CancelTimers(this);

            OnDispose();

            for (int i = 0; i < m_subviews.Count; ++i)
            {
                m_subviews[i].Dispose();
            }
        }

        protected virtual void OnDispose()
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public bool IsEnabled { get; set; }

        public View ParentView { get; private set; }

        public virtual View RootView 
        { 
            get { return ParentView != null ? ParentView.RootView : null; } 
        }

        public List<View> Subviews
        {
            get { return m_subviews; }
        }

        public View FistSubview
        {
            get { return m_subviews.Count > 0 ? m_subviews[0] : null; }
        }

        public View LastSubview
        {
            get { return m_subviews.Count > 0 ? m_subviews[m_subviews.Count - 1] : null; }
        }

        public Rect Frame 
        { 
            get { return new Rect(X, Y, Width, Height); } 
            set 
            {
                m_origin.x = value.x;
                m_origin.y = value.y;
                m_bounds.width = value.width;
                m_bounds.height = value.height;
            } 
        }

        public Rect Bounds 
        { 
            get { return m_bounds; } 
            set 
            {
                m_bounds.width = value.width;
                m_bounds.height = value.height;
            } 
        }

        public float X 
        { 
            get { return m_origin.x; } 
            set { m_origin.x = value; } 
        }

        public float Y 
        { 
            get { return m_origin.y; } 
            set { m_origin.y = value; } 
        }
        
        public float Width 
        { 
            get { return m_bounds.width; } 
            set { m_bounds.width = value; } 
        }
        
        public float Height 
        { 
            get { return m_bounds.height; } 
            set { m_bounds.height = value; } 
        }

        public float Right
        {
            get { return X + Width; }
            set { X = value - Width; }
        }

        public float Bottom
        {
            get { return Y + Height; }
            set { Y = value - Height; }
        }

        public object Tag { get; set; }

        public ViewAutoresizing AutoresizeMask { get; set; }

        public Color BackColor 
        { 
            get { return m_backColor; }
            set 
            {
                if (m_backColor != value)
                {
                    m_backColor = value;
                    Style.normal.background = UIHelper.Create1x1ColorTexture(value);
                }
            }
        }

        protected GUIStyle Style
        {
            get
            {
                if (m_style == null)
                {
                    m_style = CreateGUIStyle();
                }
                return m_style;
            }

            set
            {
                m_style = value;
            }
        }

        internal EventDelegate MouseEvent { get; set; }
        internal EventDelegate MouseMove { get; set; }
        internal EventDelegate MouseDrag { get; set; }
        internal EventDelegate MouseDown { get; set; }
        internal EventDelegate MouseDoubleClick { get; set; }
        internal EventDelegate MouseUp { get; set; }

        internal EventDelegate KeyDown { get; set; }
        internal EventDelegate KeyUp { get; set; }

        public bool IsMouseEventsEnabled { get; set; }
        public bool IsKeyEventsEnabled { get; set; }

        public bool IsFocusable
        {
            get { return m_focusableViewId != null; }
            set
            {
                if (value)
                {
                    if (m_focusableViewId == null)
                    {
                        m_focusableViewId = GetType().Name + "-" + m_nextFocusableViewId;
                        ++m_nextFocusableViewId;
                    }
                }
                else
                {
                    m_focusableViewId = null;
                }
            }
        }

        protected void SetFocusableControlName()
        {
            GUI.SetNextControlName(m_focusableViewId);
        }

        public virtual bool IsFocused
        {
            get { return IsFocusable && GUI.GetNameOfFocusedControl() == m_focusableViewId; }
        }

        #endregion
    }

    internal abstract class Event
    {
        private static Event m_instance;

        public Event()
        {
            m_instance = this;
        }

        public static Event current
        { 
            get { return m_instance != null ? m_instance.CurrentEvent : null; }
        }

        public static void Destroy()
        {
            if (m_instance != null)
            {
                m_instance.DestroyEvent();
            }
        }

        public abstract EventType type { get; set; }

        public abstract bool isMouse { get; }
        public abstract bool isKey { get; }
        public abstract void Use();

        public abstract KeyCode keyCode { get; set; }
        public abstract char character { get; set; }

        public abstract bool control { get; set; }
        public abstract bool shift { get; set; }
        public abstract bool alt { get; set; }
        public abstract bool command { get; set; }

        public abstract Vector2 mousePosition { get; set; }
        public abstract int button { get; set; }
        public abstract int clickCount { get; set; }

        protected abstract Event CurrentEvent { get; }

        protected abstract void DestroyEvent();
    }

    class SharedStyles
    {
        private static GUIContent m_content = new GUIContent();

        private static GUIStyle m_linkStyle;

        private static GUIStyle m_consoleTextStyle;
        private static GUIStyle m_consoleLinkStyle;
        private static GUIStyle m_consoleLinkInnactiveStyle;

        private static GUIStyle m_searchFieldStyle;
        private static GUIStyle m_searchCancelButton;

        private static GUIStyle m_toolbarButtonStyle;

        private static GUIStyle m_selectedCellBackStyle;
        private static GUIStyle m_cursorStyle;

        private static GUIStyle m_clientConnectedStyle;
        private static GUIStyle m_clientDisconnectedStyle;

        public static GUIStyle label
        { 
            get
            {
                return GUI.skin.label;
            }
        }

        public static GUIStyle consoleTextStyle
        { 
            get
            {
                if (m_consoleTextStyle == null)
                {
                    m_consoleTextStyle = new GUIStyle(GUI.skin.label);
                    m_consoleTextStyle.font = (Font) UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Editor/Lunar/Menlo-Regular.ttf", typeof(Font)); // FIXME: embed in configuration
                    m_consoleTextStyle.fontSize = 12;
                    m_consoleTextStyle.normal.textColor = EditorSkin.GetColor(ColorCode.Plain);
                    m_consoleTextStyle.richText = true;
                }

                return m_consoleTextStyle;
            }
        }

        public static GUIStyle consoleLinkStyle
        { 
            get
            {
                if (m_consoleLinkStyle == null)
                {
                    m_consoleLinkStyle = new GUIStyle(consoleTextStyle);
                    m_consoleLinkStyle.normal.textColor = EditorSkin.GetColor(ColorCode.Link);
                }
                
                return m_consoleLinkStyle;
            }
        }

        public static GUIStyle consoleLinkInnactiveStyle
        { 
            get
            {
                if (m_consoleLinkInnactiveStyle == null)
                {
                    m_consoleLinkInnactiveStyle = new GUIStyle(consoleTextStyle);
                    m_consoleLinkInnactiveStyle.normal.textColor = EditorSkin.GetColor(ColorCode.LinkInnactive);
                }
                
                return m_consoleLinkInnactiveStyle;
            }
        }

        public static GUIStyle linkTextStyle
        {
            get
            {
                if (m_linkStyle == null)
                {
                    m_linkStyle = new GUIStyle(GUI.skin.label);
                    m_linkStyle.normal.textColor = EditorSkin.GetColor(ColorCode.Link);
                }

                return m_linkStyle;
            }
        }

        public static GUIStyle searchField
        {
            get
            {
                m_searchFieldStyle = FindStyleIfNull(m_searchFieldStyle, "ToolbarSeachTextFieldPopup");
                return m_searchFieldStyle;
            }
        }

        public static GUIStyle toolbarSearchCancelButton
        {
            get
            {
                m_searchCancelButton = FindStyleIfNull(m_searchCancelButton, "ToolbarSeachCancelButton");
                return m_searchCancelButton;
            }
        }

        public static GUIStyle toolbar
        {
            get
            {
                return EditorStyles.toolbar;
            }
        }

        public static GUIStyle toolbarButton
        {
            get
            {
                m_toolbarButtonStyle = FindStyleIfNull(m_toolbarButtonStyle, "toolbarbutton");
                return m_toolbarButtonStyle;
            }
        }

        public static GUIStyle SelectedCellBackStyle
        {
            get
            {
                if (m_selectedCellBackStyle == null)
                {
                    m_selectedCellBackStyle = new GUIStyle("box");
                }
                return m_selectedCellBackStyle;
            }
        }

        public static GUIStyle Cursor
        {
            get
            {
                if (m_cursorStyle == null)
                {
                    m_cursorStyle = new GUIStyle();
                    m_cursorStyle.normal.background = UIHelper.Create1x1ColorTexture(Color.blue);
                }

                return m_cursorStyle;
            }
        }

        public static GUIStyle ClientConnectedStyle
        {
            get
            {
                m_clientConnectedStyle = FindStyleIfNull(m_clientConnectedStyle, "WinBtnMaxMac");
                return m_clientConnectedStyle;
            }
        }

        public static GUIStyle ClientDisconnectedStyle
        {
            get
            {
                m_clientDisconnectedStyle = FindStyleIfNull(m_clientDisconnectedStyle, "WinBtnCloseMac");
                return m_clientDisconnectedStyle;
            }
        }

        private static GUIStyle FindStyleIfNull(GUIStyle style, string name)
        {
            if (style == null)
            {
                style = GUI.skin.FindStyle(name);
                if (style == null)
                {
                    return GUIStyle.none;
                }
            }

            return style;
        }

        public static Vector2 MeasureText(string text)
        {
            return MeasureText(label, text);
        }

        public static Vector2 MeasureText(GUIStyle style, string text)
        {
            m_content.text = text;
            return style.CalcSize(m_content);
        }

        public static float MeasureWidth(GUIStyle style, string text)
        {
            return MeasureText(style, text).x;
        }

        public static float MeasureHeight(GUIStyle style, string text, float width)
        {
            m_content.text = text;
            return style.CalcHeight(m_content, width);
        }
    }

    class GUIStyleTextMeasure : ITextMeasure
    {
        private GUIStyle m_style;

        public GUIStyleTextMeasure(GUIStyle style)
        {
            if (style == null)
            {
                throw new NullReferenceException("Style is null");
            }

            m_style = style;
        }

        public Vector2 CalcSize(string text)
        {
            return SharedStyles.MeasureText(m_style, text);
        }

        public float CalcHeight(string text, float width)
        {
            return SharedStyles.MeasureHeight(m_style, text, width);
        }

        public float LineHeight
        {
            get { return m_style.lineHeight; }
        }

        public GUIStyle Style
        {
            get { return m_style; }
        }
    }

    static class SharedGUIContent
    {
        private static GUIContent m_content = new GUIContent();

        public static GUIContent Get(string text)
        {
            m_content.text = text;
            return m_content;
        }
    }
}
