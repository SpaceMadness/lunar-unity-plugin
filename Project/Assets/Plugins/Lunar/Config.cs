using UnityEngine;

using System;
using System.Collections;

namespace LunarPluginInternal
{
    static class Config
    {
        #if LUNAR_DEVELOPMENT
        public static readonly bool lunarDebugMode = true;
        #else
        public static readonly bool lunarDebugMode = false;
        #endif

        public static readonly bool isUnityFree;
        public static readonly bool isUnityPro;

        public static readonly bool isUnityBuild;
        public static bool isDebugBuild;

        static Config()
        {
            try
            {
                isDebugBuild = Debug.isDebugBuild;
                isUnityPro = Application.HasProLicense();
                isUnityFree = !isUnityPro;
                isUnityBuild = true;
            }
            catch (Exception)
            {
                isUnityFree = isUnityPro = false;
                isDebugBuild = true;
                isUnityBuild = false;
            }
        }

        /* For unit test */
        #if LUNAR_DEVELOPMENT
        public static void OverrideIsDebugBuild(bool value)
        {
            isDebugBuild = value;
        }
        #endif
    }
}
