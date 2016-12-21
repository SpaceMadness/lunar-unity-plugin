using System;

using LunarPlugin;
using LunarPluginInternal;
using UnityEngine;

namespace LunarPlugin.Test
{
    public class TestAppConfig
    {
        public bool shouldResolveCommands;
        public bool shouldLogTerminal;
        public bool shouldExecDefaultConfig;
        public bool shouldRegisterCommandNotifications;

        public TestAppConfig()
        {
            shouldLogTerminal = true;
        }
    }

    public class AppTestFixtureBase : TestFixtureBase, ITestAppImpDelegate
    {
        private TestApp m_app;

        protected override void RunSetUp()
        {
            base.RunSetUp();

            m_app = new TestApp(this, CreateTestConfig());
            m_app.Start();

            RunUpdate(1.0f); // make sure all 'start up' timers are done
        }

        protected virtual TestAppConfig CreateTestConfig()
        {
            return new TestAppConfig();
        }

        protected override void RunTearDown()
        {
            m_app.Stop();
            m_app.Destroy();

            base.RunTearDown();
        }

        protected void RunUpdate(float delta = 0.016f)
        {
            TestApp.RunUpdate(delta);
        }

        #region ITestAppImpDelegate

        public void LogTerminal(string message)
        {
            if (IsTrackTerminalLog)
            {
                AddResult(message);
            }
        }

        public void LogTerminal(string[] table)
        {
            if (IsTrackTerminalLog)
            {
                AddResult(CStringUtils.Join(table));
            }
        }

        public void LogTerminal(Exception e, string message)
        {
            throw new Exception(message, e);
        }

        public void ClearTerminal()
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Input

        protected void TapKeys(params KeyCode[] keys)
        {
            foreach (KeyCode key in keys)
            {
                m_app.PressKey(key);
            }
            RunUpdate();
        }

        protected void PressKeys(params KeyCode[] keys)
        {
            foreach (KeyCode key in keys)
            {
                m_app.PressKey(key, true);
            }
            RunUpdate();
        }

        protected void ReleaseKeys(params KeyCode[] keys)
        {
            foreach (KeyCode key in keys)
            {
                m_app.ReleaseKey(key);
            }
            RunUpdate();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        protected void AddResult(string format, params object[] args)
        {
            this.Result.Add(CStringUtils.RemoveRichTextTags(string.Format(format, args)));
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        protected bool IsTrackConsoleLog { get; set; }
        protected bool IsTrackTerminalLog { get; set; }

        #endregion
    }
}

