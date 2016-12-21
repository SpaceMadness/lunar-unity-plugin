//
//  CEditorApp.cs
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
using System.Collections.Generic;
using System.Text;

using UnityEditor;
using UnityEngine;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    delegate bool CURLHandler(string urlString);

    class CEditorApp : CApp
    {
        private static double m_lastUpdateTime;
        private static CEditorApp s_editorInstance; // keep another reference to avoid casts
        private static readonly IDictionary<string, CURLHandler> s_urlHandlers;

        static CEditorApp()
        {
            CEditorApp editorApp = new CEditorApp();
            s_sharedInstance = editorApp;
            s_editorInstance = editorApp;
            editorApp.Start();

            s_urlHandlers = CreateURLHandlersLookup();
        }

        private CEditorApp()
        {
            m_lastUpdateTime = EditorApplication.timeSinceStartup;

            CTimerManager.ScheduleTimer(() =>
            {
                CThreadUtils.InitOnMainThread(); // we need to make sure this call is done on the main thread
                CLog.Initialize(); // it's safe to initialize logging

                CEditorSceneKeyHandler.keyDownHandler += SceneKeyDownHandler;
                CEditorSceneKeyHandler.keyUpHandler += SceneUpDownHandler;
            });
        }

        public static void Update()
        {
            float delta = (float)(EditorApplication.timeSinceStartup - m_lastUpdateTime);
            s_editorInstance.Update(delta);
            m_lastUpdateTime = EditorApplication.timeSinceStartup;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region AppImp

        protected override CAppImp CreateAppImp()
        {
            return new CEditorAppImp();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Scene key handling

        private bool SceneKeyDownHandler(KeyCode key, CModifiers modifiers)
        {
            CBinding binding;
            CShortCut shortCut = new CShortCut(key, modifiers);
            if (CBindings.FindBinding(shortCut, out binding))
            {
                return ExecCommand(binding.cmdKeyDown);
            }

            return false;
        }

        private bool SceneUpDownHandler(KeyCode key, CModifiers modifiers)
        {
            CBinding binding;
            CShortCut shortCut = new CShortCut(key, modifiers);
            if (CBindings.FindBinding(shortCut, out binding) && binding.cmdKeyUp != null)
            {
                return ExecCommand(binding.cmdKeyUp);
            }

            return false;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region URL handling

        internal static bool HandleURL(string urlString)
        {
            CAssert.IsNotEmpty(urlString);

            try
            {
                Uri uri = new Uri(urlString);
                string scheme = uri.Scheme;

                CURLHandler handler = FindURLHandler(scheme);
                if (handler == null)
                {
                    CLog.e("Can't find URL handler for scheme: '{0}'", scheme);
                    return false;
                }

                return handler(urlString);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return false;
        }

        internal static CURLHandler RegisterURLHanlder(string scheme, CURLHandler handler)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException("scheme");
            }

            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            CURLHandler oldHandler = FindURLHandler(scheme);
            s_urlHandlers[scheme] = handler;
            return oldHandler;
        }

        private static CURLHandler FindURLHandler(string scheme)
        {
            CURLHandler handler;
            if (!string.IsNullOrEmpty(scheme) && s_urlHandlers.TryGetValue(scheme, out handler))
            {
                return handler;
            }

            return null;
        }

        private static IDictionary<string, CURLHandler> CreateURLHandlersLookup()
        {
            IDictionary<string, CURLHandler> handlers = new Dictionary<string, CURLHandler>();

            CURLHandler webPageURLHandler = delegate(string urlString)
            {
                Application.OpenURL(urlString);
                return true;
            };
            handlers["http"] = webPageURLHandler;
            handlers["https"] = webPageURLHandler;

            return handlers;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Playmode

        internal static void OnPlayModeChanged(bool isPlaying)
        {
            CEditor.OnPlayModeChanged(isPlaying);

            if (isPlaying)
            {
                // create a runtime object to properly handle key bindings
                GameObject runtimeObj = new GameObject("Lunar Runtime Behaviour");
                runtimeObj.AddComponent<LunarRuntimeBehaviour>();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        internal static CEditorApp EditorInstance
        {
            get { return s_editorInstance; }
        }

        internal static CTerminal Terminal
        {
            get { return Imp.Terminal; }
        }

        protected new static CEditorAppImp Imp
        {
            get { return (CEditorAppImp)CApp.Imp; }
        }

        #endregion
    }
}

