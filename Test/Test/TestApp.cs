using System;

using LunarPluginInternal;
using UnityEngine;

namespace LunarPlugin.Test
{
    class TestApp : App
    {
        private readonly ITestAppImpDelegate m_delegate;
        private readonly TestAppConfig m_config;
        private readonly TestInput m_input;

        public TestApp(ITestAppImpDelegate del, TestAppConfig config)
        {
            s_sharedInstance = this;
            m_delegate = del;
            m_config = config;
            m_input = CreateTestInput();
        }

        #region Lifecycle

        public new void Start()
        {
            base.Start();
        }

        public new void Stop()
        {
            base.Stop();
        }

        #endregion

        #region Updatable

        public static void RunUpdate(float delta)
        {
            ((TestApp) s_sharedInstance).Update(delta);
        }

        public new void Update(float delta)
        {
            base.Update(delta);

            m_input.Update(delta);
        }

        #endregion

        #region Factory methods

        protected override AppImp CreateAppImp()
        {
            return new TestAppImp(this);
        }

        protected virtual TestInput CreateTestInput()
        {
            return new TestInput();
        }

        #endregion

        #region Input

        public void PressKey(KeyCode key, bool hold = false)
        {
            m_input.PressKey(key, hold);
        }

        public void ReleaseKey(KeyCode key)
        {
            m_input.ReleaseKey(key);
        }

        public bool GetKeyDown(KeyCode key)
        {
            return m_input.GetKeyDown(key);
        }

        public bool GetKeyUp(KeyCode key)
        {
            return m_input.GetKeyUp(key);
        }

        public bool GetKey(KeyCode key)
        {
            return m_input.GetKey(key);
        }

        #endregion

        #region Properties

        public ITestAppImpDelegate Delegate
        {
            get { return m_delegate; }
        }

        public TestAppConfig Config
        {
            get { return m_config; }
        }

        #endregion
    }
}

