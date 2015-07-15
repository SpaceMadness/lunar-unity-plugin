using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

using LunarPlugin;

namespace LunarPluginInternal
{
    abstract class App : IUpdatable, IDestroyable
    {
        protected static App s_sharedInstance;

        private readonly AppImp m_appImp;

        protected App()
        {
            m_appImp = CreateAppImp();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region static Access

        public static bool ExecCommand(string commandLine, bool manual = false)
        {
            if (commandLine == null)
            {
                throw new ArgumentNullException("Command line is null");
            }

            return Imp.ExecCommand(commandLine, manual);
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

        protected abstract AppImp CreateAppImp();

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

        protected static AppImp Imp
        {
            get { return s_sharedInstance.m_appImp; }
        }

        #endregion
    }
}