//
//  CApp.cs
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
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

using LunarPlugin;

[assembly: InternalsVisibleTo("Assembly-CSharp")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

[assembly: InternalsVisibleTo("Test")]
[assembly: InternalsVisibleTo("LunarEditor")]

namespace LunarPluginInternal
{
    abstract class CApp : ICUpdatable, ICDestroyable
    {
        #pragma warning disable 0649
        protected static CApp s_sharedInstance;
        #pragma warning restore 0649

        private readonly CAppImp m_appImp;

        protected CApp()
        {
            m_appImp = CreateAppImp();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region static Access

        public static bool ExecCommand(string commandLine, bool manual = false)
        {
            if (commandLine == null)
            {
                throw new ArgumentNullException("commandLine");
            }

            if (s_sharedInstance == null)
            {
                CLog.e("Can't execute command: app is not initialized");
                return false;
            }

            return Imp.ExecCommand(commandLine, manual);
        }

        internal static void UpdateKeyBindings()
        {
            if (s_sharedInstance != null)
            {
                Imp.UpdateKeyBindings();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        protected void Start()
        {
            m_appImp.Start();
        }

        protected void Stop()
        {
            m_appImp.Stop();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Factory methods

        protected abstract CAppImp CreateAppImp();

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public void Update(float dt)
        {
            m_appImp.Update(dt);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IDestroyable

        public void Destroy()
        {
            m_appImp.Destroy();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        protected static CAppImp Imp
        {
            get { return s_sharedInstance.m_appImp; }
        }

        #endregion
    }
}