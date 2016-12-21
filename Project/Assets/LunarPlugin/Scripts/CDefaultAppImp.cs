//
//  CDefaultAppImp.cs
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

﻿using System.Collections.Generic;

using UnityEngine;

using LunarPlugin;

namespace LunarPluginInternal
{
    class CDefaultAppImp : CAppImp, ICUpdatable, ICDestroyable, ICCommandDelegate
    {
        private readonly CCommandProcessor m_processor;
        private readonly CTimerManager m_timerManager;
        private readonly CNotificationCenter m_notificationCenter;
        private readonly CUpdatableList m_updatables;

        public CDefaultAppImp()
        {
            m_timerManager = CreateTimerManager();
            m_notificationCenter = CreateNotificationCenter();
            m_processor = CreateCommandProcessor();

            m_updatables = new CUpdatableList(2);
            m_updatables.Add(m_timerManager);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public virtual void Start()
        {
            ResolveCommands();

            CTimerManager.ScheduleTimerOnce(delegate()
            {
                ExecStartupConfigs();
                RegisterCommandNotifications();
            });
        }

        public void Stop()
        {
            // TODO: cancel all and release resources
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Inheritance

        protected virtual void ResolveCommands()
        {
            CRegistery.ResolveElements();
        }

        protected virtual void ExecStartupConfigs()
        {
            // TODO: unit tests
            CApp.ExecCommand("exec " + CConstants.ConfigDefault);
            CApp.ExecCommand("exec " + CConstants.ConfigAutoExec);
            if (CRuntime.IsPlaying)
            {
                CApp.ExecCommand("exec " + CConstants.ConfigPlayMode);
            }
        }

        protected virtual void RegisterCommandNotifications()
        {
            // cvar value changed
            m_notificationCenter.Register(CCommandNotifications.CVarValueChanged, delegate(CNotification n)
            {
                bool manual = n.Get<bool>(CCommandNotifications.KeyManualMode);

                CVar cvar = n.Get<CVar>(CCommandNotifications.CVarValueChangedKeyVar);
                CAssert.IsNotNull(cvar);

                OnCVarValueChanged(cvar, manual);
            });

            // binding changed
            m_notificationCenter.Register(CCommandNotifications.CBindingsChanged, delegate(CNotification n)
            {
                bool manual = n.Get<bool>(CCommandNotifications.KeyManualMode);
                OnCBindingsChanged(manual);
            });

            // alias changed
            m_notificationCenter.Register(CCommandNotifications.CAliasesChanged, delegate(CNotification n)
            {
                bool manual = n.Get<bool>(CCommandNotifications.KeyManualMode);
                OnCAliasesChanged(manual);
            });
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IUpdatable

        public void Update(float dt)
        {
            m_updatables.Update(dt);
        }

        protected void AddUpdatable(ICUpdatable updatable)
        {
            m_updatables.Add(updatable);
        }

        protected void AddUpdatable(CUpdatableDelegate updatable)
        {
            m_updatables.Add(updatable);
        }

        protected void RemoveUpdatable(ICUpdatable updatable)
        {
            m_updatables.Remove(updatable);
        }

        protected void RemoveUpdatable(CUpdatableDelegate updatable)
        {
            m_updatables.Remove(updatable);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IDestroyable

        public void Destroy()
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Commands

        public bool ExecCommand(string commandLine, bool manual)
        {
            return m_processor.TryExecute(commandLine, manual);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region ICCommandDelegate implementation

        public virtual void LogTerminal(string message)
        {
        }

        public virtual void LogTerminal(string[] table)
        {
        }

        public virtual void LogTerminal(System.Exception e, string message)
        {
        }

        public virtual void ClearTerminal()
        {
        }

        public virtual bool ExecuteCommandLine(string commandLine, bool manual = false)
        {
            return ExecCommand(commandLine, manual);
        }

        public virtual void PostNotification(CCommand cmd, string name, params object[] data)
        {
            m_notificationCenter.Post(cmd, name, data);
        }

        public virtual bool IsPromptEnabled
        {
            get { return false; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Factory methods

        protected virtual CTimerManager CreateTimerManager()
        {
            return CTimerManager.SharedInstance;
        }

        protected virtual CNotificationCenter CreateNotificationCenter()
        {
            return CNotificationCenter.SharedInstance;
        }

        protected virtual CCommandProcessor CreateCommandProcessor()
        {
            CCommandProcessor processor = new CCommandProcessor();
            processor.CommandDelegate = this;
            return processor;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Config

        protected virtual void OnCVarValueChanged(CVar cvar, bool manual)
        {
            if (manual)
            {
                ScheduleSaveConfig();
            }
        }

        protected virtual void OnCBindingsChanged(bool manual)
        {
            if (manual)
            {
                ScheduleSaveConfig();
            }
        }

        protected virtual void OnCAliasesChanged(bool manual)
        {
            if (manual)
            {
                ScheduleSaveConfig();
            }
        }

        protected virtual void ScheduleSaveConfig()
        {
            m_timerManager.ScheduleOnce(SaveConfig);
        }

        protected virtual void SaveConfig()
        {
            CApp.ExecCommand("writeconfig " + CConstants.ConfigDefault);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Bindings

        public void UpdateKeyBindings()
        {
            IList<CBinding> bindings = CBindings.BindingsList;
            for (int i = 0; i < bindings.Count; ++i)
            {
                KeyCode key = bindings[i].key;
                if (GetKeyDown(key))
                {
                    if (IsValidModifiers(bindings[i].shortCut))
                    {
                        string commandLine = bindings[i].cmdKeyDown;
                        ExecCommand(commandLine, false);
                    }
                }
                else if (GetKeyUp(key))
                {
                    if (IsValidModifiers(bindings[i].shortCut))
                    {
                        string commandLine = bindings[i].cmdKeyUp;
                        if (commandLine != null)
                        {
                            ExecCommand(commandLine, false);
                        }
                    }
                }
            }
        }

        private bool IsValidModifiers(CShortCut shortCut)
        {
            if (shortCut.IsShift ^ (GetKey(KeyCode.LeftShift) || GetKey(KeyCode.RightShift))) return false;
            if (shortCut.IsControl ^ (GetKey(KeyCode.LeftControl) || GetKey(KeyCode.RightControl))) return false;
            if (shortCut.IsAlt ^ (GetKey(KeyCode.LeftAlt) || GetKey(KeyCode.RightAlt))) return false;
            if (shortCut.IsCommand ^ (GetKey(KeyCode.LeftCommand) || GetKey(KeyCode.RightCommand))) return false;

            return true;
        }

        protected virtual bool GetKeyDown(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        protected virtual bool GetKeyUp(KeyCode key)
        {
            return Input.GetKeyUp(key);
        }

        protected virtual bool GetKey(KeyCode key)
        {
            return Input.GetKey(key);
        }

        #endregion
    }
}

