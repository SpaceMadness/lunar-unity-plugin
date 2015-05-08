using UnityEngine;

using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

using LunarPlugin;

namespace LunarPluginInternal
{
    static class Tags
    {
        public static readonly Tag App = new Tag("#app", Config.lunarDebugMode);
    }

    abstract class App : IDestroyable, ICCommandDelegate
    {
        protected static App s_sharedInstance;

        private UpdatableList m_updatables;
        private TimerManager m_timerManager;

        private NotificationCenter m_notificationCenter;

        private CommandProcessor m_processor;

        protected App()
        {
            m_processor = new CommandProcessor();
            m_processor.CommandDelegate = this;

            m_timerManager = CreateTimerManager();
            m_notificationCenter = CreateNotificationCenter();

            m_updatables = new UpdatableList(3);
            m_updatables.Add(m_timerManager);
            m_updatables.Add(UpdateBindings);

            CRegistery.ResolveCommands();
        }

        protected virtual void Start()
        {
            TimerManager.ScheduleTimerOnce(delegate()
            {
                App.ExecCommand("exec default.cfg");

                NotificationCenter.RegisterNotification(CCommandNotifications.CVarValueChanged, OnCVarValueChanged);
                NotificationCenter.RegisterNotification(CCommandNotifications.CBindingsChanged, OnCBindingsChanged);
                NotificationCenter.RegisterNotification(CCommandNotifications.CAliasesChanged, OnCAliasesChanged);
            });
        }

        protected virtual TimerManager CreateTimerManager()
        {
            return TimerManager.SharedInstance;
        }

        protected virtual NotificationCenter CreateNotificationCenter()
        {
            return NotificationCenter.SharedInstance;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public static void Update()
        {
            if (s_sharedInstance != null)
            {
                s_sharedInstance.UpdateInstance();
            }
        }

        private void UpdateInstance()
        {
            UpdateInstance(Time.deltaTime);
        }

        protected void UpdateInstance(float delta)
        {
            m_updatables.Update(delta);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IDestroyable

        public void Destroy()
        {
            m_notificationCenter.Destroy();
            m_timerManager.Destroy();
            m_updatables.Destroy();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
        
        #region Life cycle

        private static App CreateInstance()
        {
            if (Runtime.IsEditor)
            {
                Type editorAppType = ClassUtils.TypeForName("LunarPlugin.EditorApp");
                PropertyInfo instanceProperty = editorAppType.GetProperty("SharedInstance", BindingFlags.Static | BindingFlags.NonPublic);
                return (App)instanceProperty.GetValue(null, null);
            }

            throw new NotImplementedException("Platform not supported");
        }

        public static void Shutdown()
        {
            if (s_sharedInstance != null)
            {
                s_sharedInstance.Destroy();
                s_sharedInstance = null;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Console commands

        public static bool ExecCommand(string commandLine, bool manualMode = false)
        {
            return s_sharedInstance.m_processor.TryExecute(commandLine, manualMode);
        }

        protected abstract void LogTerminalImpl(string line);

        protected abstract void LogTerminalImpl(string[] table);

        protected abstract void ClearTerminalImpl();

        #endregion

        #region ICommandDelegate

        public void LogTerminal(string line)
        {
            LogTerminalImpl(line);
        }

        public void LogTerminal(string[] table)
        {
            LogTerminalImpl(table);
        }

        public void ClearTerminal()
        {
            ClearTerminalImpl();
        }

        public bool ExecuteCommandLine(string commandLine, bool manual = false)
        {
            return App.ExecCommand(commandLine, manual);
        }

        public void PostNotification(CCommand cmd, string name, params object[] data)
        {
            object sender = (object)cmd;
            NotificationCenter.PostNotification(sender, name, data);
        }

        public bool IsPromptEnabled
        {
            get { return true; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private void OnCVarValueChanged(Notification n)
        {
            bool manual = n.Get<bool>(CCommandNotifications.KeyManualMode);
            if (manual)
            {
                CVar cvar = n.Get<CVar>(CCommandNotifications.CVarValueChangedKeyVar);
                Assert.IsNotNull(cvar);

                if (cvar != null)
                {
                    ScheduleSaveConfig();
                }
            }
        }

        private void OnCBindingsChanged(Notification n)
        {
            bool manual = n.Get<bool>(CCommandNotifications.KeyManualMode);
            if (manual)
            {
                ScheduleSaveConfig();
            }
        }

        private void OnCAliasesChanged(Notification n)
        {
            bool manual = n.Get<bool>(CCommandNotifications.KeyManualMode);
            if (manual)
            {
                ScheduleSaveConfig();
            }
        }

        private void ScheduleSaveConfig()
        {
            m_timerManager.ScheduleOnce(SaveConfig);
        }

        private void SaveConfig()
        {
            App.ExecCommand("writeconfig default.cfg");
        }

        //////////////////////////////////////////////////////////////////////////////

        #region

        private void UpdateBindings(float delta)
        {
            IList<CBinding> bindings = CBindings.BindingsList;
            for (int i = 0; i < bindings.Count; ++i)
            {
                KeyCode key = bindings[i].key;
                if (Input.GetKeyDown(key))
                {
                    string commandLine = bindings[i].cmd;
                    ExecCommand(commandLine, true);
                }
            }
        }

        #endregion
    }
}