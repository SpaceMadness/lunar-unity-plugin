using System;

using UnityEngine;

using LunarPluginInternal;

namespace LunarPlugin.Test
{
    class TestAppImp : DefaultAppImp
    {
        private TestApp m_app;

        public TestAppImp(TestApp app)
        {
            m_app = app;
            AddUpdatable(UpdateBindings);
        }

        protected override void ResolveCommands()
        {
            if (Config.shouldResolveCommands)
            {
                base.ResolveCommands();
            }
        }

        protected override void ExecStartupConfigs()
        {
            if (Config.shouldExecDefaultConfig)
            {
                base.ExecStartupConfigs();
            }
        }

        protected override void RegisterCommandNotifications()
        {
            if (Config.shouldRegisterCommandNotifications)
            {
                base.RegisterCommandNotifications();
            }
        }

        public override void LogTerminal(string message)
        {
            if (Config.shouldLogTerminal)
            {
                Delegate.LogTerminal(message);
            }
        }

        public override void LogTerminal(string[] table)
        {
            if (Config.shouldLogTerminal)
            {
                Delegate.LogTerminal(table);
            }
        }

        public override void LogTerminal(Exception e, string message)
        {
            if (Config.shouldLogTerminal)
            {
                Delegate.LogTerminal(e, message);
            }
        }

        public override void ClearTerminal()
        {
            if (Config.shouldLogTerminal)
            {
                Delegate.ClearTerminal();
            }
        }

        void UpdateBindings(float dt)
        {
            UpdateKeyBindings();
        }

        protected override bool GetKeyDown(KeyCode key)
        {
            return m_app.GetKeyDown(key);
        }

        protected override bool GetKeyUp(KeyCode key)
        {
            return m_app.GetKeyUp(key);
        }

        protected override bool GetKey(KeyCode key)
        {
            return m_app.GetKey(key);
        }

        TestAppConfig Config
        {
            get { return m_app.Config; }
        }

        ITestAppImpDelegate Delegate
        {
            get { return m_app.Delegate; }
        }
    }

    interface ITestAppImpDelegate
    {
        void LogTerminal(string message);
        void LogTerminal(string[] table);
        void LogTerminal(Exception e, string message);
        void ClearTerminal();
    }
}

