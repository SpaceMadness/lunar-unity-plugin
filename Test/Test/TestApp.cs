using System;

using LunarPluginInternal;

namespace LunarPlugin.Test
{
    class TestApp : App
    {
        private readonly ITestAppImpDelegate m_delegate;
        private readonly TestAppConfig m_config;

        public TestApp(ITestAppImpDelegate del, TestAppConfig config)
        {
            s_sharedInstance = this;
            m_delegate = del;
            m_config = config;
        }

        public new void Start()
        {
            base.Start();
        }

        public new void Stop()
        {
            base.Stop();
        }

        public static void RunUpdate(float delta)
        {
            s_sharedInstance.Update(delta);
        }

        protected override AppImp CreateAppImp()
        {
            return new TestAppImp(this);
        }

        public ITestAppImpDelegate Delegate
        {
            get
            {
                return m_delegate;
            }
        }

        public TestAppConfig Config
        {
            get
            {
                return m_config;
            }
        }
    }
}

