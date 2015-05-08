using System;
using System.Collections.Generic;
using System.Text;

using UnityEditor;
using UnityEngine;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    delegate bool URLHandler(string urlString);

    class EditorApp : App
    {
        private static double m_lastUpdateTime;

        private static EditorApp s_editorInstance; // keep another reference to avoid casts

        private static readonly IDictionary<string, URLHandler> s_urlHandlers;

        private readonly Terminal m_terminal;

        private readonly Preferences m_preferences;

        static EditorApp()
        {
            EditorApp editorApp = new EditorApp();
            s_sharedInstance = editorApp;
            s_editorInstance = editorApp;
            editorApp.Start();

            s_urlHandlers = CreateURLHandlersLookup();
        }

        private EditorApp()
        {
            m_preferences = CreatePreferences();
            m_terminal = CreateTerminal(CVars.c_historySize.IntValue);
            m_lastUpdateTime = EditorApplication.timeSinceStartup;

            TimerManager.ScheduleTimer(() =>
            {
                ThreadUtils.InitOnMainThread(); // we need to make sure this call is done on the main thread
                Log.Initialize(); // it's safe to initialize logging

                EditorSceneKeyHandler.keyDownHandler += SceneKeyDownHandler;
            });
        }

        public new static void Update()
        {
            float delta = (float)(EditorApplication.timeSinceStartup - m_lastUpdateTime);
            s_editorInstance.UpdateInstance(delta);
            m_lastUpdateTime = EditorApplication.timeSinceStartup;
        }

        protected override void LogTerminalImpl(string line)
        {
            m_terminal.Add(line);
        }

        protected override void LogTerminalImpl(string[] table)
        {
            m_terminal.Add(table);
        }

        protected override void LogTerminalImpl(Exception e, string message)
        {
            m_terminal.Add(e, message);
        }

        protected override void ClearTerminalImpl()
        {
            m_terminal.Clear();
        }

        private Terminal CreateTerminal(int capacity)
        {
            return new FormattedTerminal(capacity);
        }

        #region Scene key handling

        private void SceneKeyDownHandler(KeyCode key)
        {
            string cmd = CBindings.FindCommand(key);
            if (cmd != null)
            {
                ExecCommand(cmd);
            }
        }

        #endregion

        #region URL handling

        internal static bool HandleURL(string urlString)
        {
            Assert.IsNotEmpty(urlString);

            try
            {
                Uri uri = new Uri(urlString);
                string scheme = uri.Scheme;

                URLHandler handler = FindURLHandler(scheme);
                if (handler == null)
                {
                    Log.e("Can't find URL handler for scheme: '{0}'", scheme);
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

        internal static URLHandler RegisterURLHanlder(string scheme, URLHandler handler)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException("Scheme is null");
            }

            if (handler == null)
            {
                throw new ArgumentNullException("Handler is null");
            }

            URLHandler oldHandler = FindURLHandler(scheme);
            s_urlHandlers[scheme] = handler;
            return oldHandler;
        }

        private static URLHandler FindURLHandler(string scheme)
        {
            URLHandler handler;
            if (!string.IsNullOrEmpty(scheme) && s_urlHandlers.TryGetValue(scheme, out handler))
            {
                return handler;
            }

            return null;
        }

        private static IDictionary<string, URLHandler> CreateURLHandlersLookup()
        {
            IDictionary<string, URLHandler> handlers = new Dictionary<string, URLHandler>();

            URLHandler webPageURLHandler = delegate(string urlString)
            {
                Application.OpenURL(urlString);
                return true;
            };
            handlers["http"] = webPageURLHandler;
            handlers["https"] = webPageURLHandler;

            return handlers;
        }

        #endregion

        #region Properties

        internal static EditorApp EditorInstance
        {
            get { return s_editorInstance; }
        }

        internal static Preferences Prefs
        {
            get { return s_editorInstance.m_preferences; }
        }

        internal static Terminal Terminal
        {
            get { return s_editorInstance.m_terminal; }
        }

        #endregion

        #region Preferences

        private static Preferences CreatePreferences()
        {
            if (Runtime.IsOSXEditor)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".lunar.plist";
                return new Preferences(path);
            }

            Debug.LogError("Unable to create preferences: platform is not supported");
            return new Preferences();
        }

        #endregion
    }
}

