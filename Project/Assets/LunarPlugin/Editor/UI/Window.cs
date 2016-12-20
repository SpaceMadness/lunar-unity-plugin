//
//  Window.cs
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
using System.Collections.Generic;
using System.IO;

using LunarPlugin;
using LunarPluginInternal;

using Event = LunarEditor.Event;

namespace LunarEditor
{
    static class WindowNotifications
    {
        public static readonly string WindowOpen = "WindowOpen";   // window : Window
        public static readonly string WindowClose = "WindowClose"; // window : Window
    }

    class Window : EditorWindow
    {
        private Vector2 m_oldSize;
        private View m_rootView;

        private string m_tag;
        private bool m_dirty;

        private static List<Window> m_windows = new List<Window>();

        static Window()
        {
            new UnityEvent(); // initialize instance
        }

        public Window(string title)
        {
            #pragma warning disable 0618
            this.title = title;
            #pragma warning restore 0618
            TimerManager.ScheduleTimer(Repaint, 1.0f); // hack: after script reloading windows are not repainted
        }

        private void RunStart()
        {
            AddWindow(this);
            OnStart();

            NotificationCenter.PostNotification(this, WindowNotifications.WindowOpen, "window", this);
        }

        private void RunStop()
        {
            RemoveWindow(this);
            OnStop();

            NotificationCenter.PostNotification(this, WindowNotifications.WindowClose, "window", this);
        }

        void OnDestroy()
        {
            RunStop();

            NotificationCenter.UnregisterNotifications(this);
        }

        void OnGUI()
        {
            if (m_rootView == null)
            {
                m_oldSize = new Vector2(this.Width, this.Height);

                m_rootView = CreateRootView();
                if (m_rootView == null)
                {
                    throw new NullReferenceException("Root view should not be null");
                }

                CreateUI();
                RunStart();
            }

            if (m_oldSize.x != this.Width || 
                m_oldSize.y != this.Height)
            {
                m_oldSize = new Vector2(this.Width, this.Height);
                OnSizeChanged();
            }

            m_rootView.OnGUI();
            m_dirty = true;

            Event.Destroy();
        }

        void OnFocus()
        {
            OnFocusGain();
        }

        void OnLostFocus()
        {
            OnFocusLost();
        }

        public new void Repaint()
        {
            if (m_dirty)
            {
                m_dirty = false;
                base.Repaint();
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        protected virtual void OnStart()
        {
        }

        protected virtual void OnStop()
        {
        }

        protected virtual void OnFocusGain()
        {
        }

        protected virtual void OnFocusLost()
        {
        }

        public bool IsFocused
        {
            get { return EditorWindow.focusedWindow == this; }
        }

        public static Window Find(string tag)
        {
            foreach (Window window in m_windows)
            {
                if (window.Tag == tag)
                {
                    return window;
                }
            }

            return null;
        }

        public static Window Find<T>()
        {
            return Find(typeof(T));
        }

        public static Window Find(Type type)
        {
            foreach (Window window in m_windows)
            {
                if (window.GetType() == type)
                {
                    return window;
                }
            }

            return null;
        }

        internal static void CloseAll()
        {
            for (int i = m_windows.Count - 1; i >= 0; --i)
            {
                m_windows[i].Close();
            }
            m_windows.Clear();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Resizing

        private void OnSizeChanged()
        {
            if (m_rootView != null)
            {
                m_rootView.Resize(this.Width, this.Height);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private static void AddWindow(Window window)
        {
            // there're might be duplicates from the serialization
            for (int i = m_windows.Count - 1; i >= 0; --i)
            {
                Window current = m_windows[i];
                if (current.Tag != null && 
                    current.Tag.Length > 0 && 
                    current.Tag == window.Tag)
                {
                    m_windows.RemoveAt(i);
                }
            }

            m_windows.Add(window);
        }

        private static void RemoveWindow(Window window)
        {
            m_windows.Remove(window);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Inheritance

        private View CreateRootView()
        {
            return new WindowRootView(this, Width, Height);
        }

        protected void DestroyRootView()
        {
            m_rootView = null;
        }

        protected virtual void CreateUI()
        {
        }

        public void AddSubview(View view)
        {
            m_rootView.AddSubview(view);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public View RootView { get { return m_rootView; } }

        public float Width
        {
            get { return position.width; }
            set
            { 
                Rect temp = this.position;
                temp.width = value;
                this.position = temp;
            }
        }

        public float Height
        {
            get { return position.height; }
            set
            { 
                Rect temp = this.position;
                temp.height = value; 
                this.position = temp;
            }
        }

        public string Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        #endregion
    }

    class WindowRootView : View
    {
        private Window m_window;
        
        public WindowRootView(Window window, float width, float height)
            : base(width, height)
        {
            if (window == null)
            {
                throw new NullReferenceException("Parent is null");
            }

            m_window = window;
        }

        public override bool IsFocused
        {
            get { return m_window.IsFocused; }
        }

        public override View RootView
        {
            get { return this; }
        }

        public override void Repaint()
        {
            m_window.Repaint();
        }
    }
}
