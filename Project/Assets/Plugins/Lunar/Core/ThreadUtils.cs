using System;
using System.Threading;
using UnityEngine;

using LunarPlugin;

namespace LunarPluginInternal
{
    static class ThreadUtils
    {
        private static readonly object s_mutex = new object();
        private static Thread s_mainThread;
        private static bool s_waitFlag;

        public static void Wait()
        {
            lock (s_mutex)
            {
                s_waitFlag = true;
                while (s_waitFlag)
                {
                    Monitor.Wait(s_mutex);
                }
            }
        }

        public static void Notify()
        {
            lock (s_mutex)
            {
                s_waitFlag = false;
                Monitor.Pulse(s_mutex);
            }
        }

        public static void InitOnMainThread()
        {
            lock (s_mutex)
            {
                if (s_mainThread != null)
                {
                    return;
                }

                if (Runtime.IsEditor)
                {
                    try
                    {
                        GUIStyle style = new GUIStyle();
                        style.CalcHeight(GUIContent.none, 0);
                    }
                    catch (ArgumentException)
                    {
                        #if LUNAR_DEBUG
                        UnityEngine.Debug.Log("ThreadUtils.Init() is not called on the main thread");
                        #endif

                        return;
                    }
                }

                s_mainThread = Thread.CurrentThread;
            }
        }

        public static bool IsUnityThread()
        {
            return Thread.CurrentThread == s_mainThread;
        }

        #if LUNAR_DEVELOPMENT
        public static void SetMainThread()
        {
            s_mainThread = Thread.CurrentThread;
        }
        #endif
    }
}

