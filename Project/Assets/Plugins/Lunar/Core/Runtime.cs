using UnityEngine;

using System;
using System.Collections;

namespace LunarPluginInternal
{
    static class Runtime
    {
        #if LUNAR_DEVELOPMENT
        private static RuntimePlatform s_overridenPlatform = RuntimePlatform.OSXEditor;
        private static bool s_overridenIsPlaying = false;
        #endif

        public static bool IsAndroid { get { return Platform == RuntimePlatform.Android; } }
        public static bool IsIOS { get { return Platform == RuntimePlatform.IPhonePlayer; } }
        public static bool IsMobile { get { return Application.isMobilePlatform; } }
        
        public static bool IsEditor { get { return Application.isEditor; } }
        public static bool IsOSXEditor { get { return Platform == RuntimePlatform.OSXEditor; } }
        public static bool IsWindowsEditor { get { return Platform == RuntimePlatform.WindowsEditor; } }

        public static bool IsStandAlone
        {
            get
            {
                return Platform == RuntimePlatform.OSXPlayer || 
                       Platform == RuntimePlatform.WindowsPlayer ||
                       Platform == RuntimePlatform.LinuxPlayer;
            }
        }

        public static bool IsOSX { get { return Platform == RuntimePlatform.OSXPlayer; } }
        public static bool IsWindows { get { return Platform == RuntimePlatform.WindowsPlayer; } }
        public static bool IsLinux { get { return Platform == RuntimePlatform.LinuxPlayer; } }

        #if LUNAR_DEVELOPMENT
        public static bool IsPlaying
        {
            get
            {
                try
                {
                    return Application.isPlaying;
                }
                catch (Exception)
                {
                    return s_overridenIsPlaying;
                }
            }

            set
            {
                s_overridenIsPlaying = value;
            }
        }
        #else
        public static bool IsPlaying
        {
            get { return Application.isPlaying; }
        }
        #endif // LUNAR_DEVELOPMENT

        #if LUNAR_DEVELOPMENT
        public static RuntimePlatform Platform
        {
            get
            {
                try
                {
                    return UnityEngine.Application.platform;
                }
                catch (Exception)
                {
                    return s_overridenPlatform;
                }
            }

            set
            {
                s_overridenPlatform = value;
            }
        }
        #else
        public static RuntimePlatform Platform
        {
            get { return UnityEngine.Application.platform; }
        }
        #endif // LUNAR_DEVELOPMENT
    }
}