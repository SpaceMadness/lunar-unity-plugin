using UnityEngine;

namespace LunarPluginInternal
{
    class MobileApp : App
    {
        internal static void Initialize()
        {
            MobileApp mobileApp = new MobileApp();
            s_sharedInstance = mobileApp;
            mobileApp.Start();
        }

        protected override AppImp CreateAppImp()
        {
            return new MobileAppImp();
        }

        internal static void Update()
        {
            s_sharedInstance.Update(Time.deltaTime);
        }
    }
}