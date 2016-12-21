using System;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

namespace LunarPlugin.Test.Timers
{
    class TestTimer : CTimer
    {
        public static void Reset()
        {
            CTimer.FreeRoot = null;
        }

        public static new CTimer FreeRoot
        {
            get { return CTimer.FreeRoot; }
            set { CTimer.FreeRoot = value; }
        }

        public new static CTimer NextFreeTimer()
        {
            return CTimer.NextFreeTimer();
        }

        public new static void AddFreeTimer(CTimer timer)
        {
            CTimer.AddFreeTimer(timer);
        }

        public static new CTimer NextTimer(CTimer t)
        {
            return CTimer.NextTimer(t);
        }

        public static new CTimer NextHelperListTimer(CTimer t)
        {
            return CTimer.NextHelperListTimer(t);
        }

        public static int PoolSize
        {
            get
            {
                int count = 0;
                for (CTimer t = FreeRoot; t != null; t = CTimer.NextTimer(t))
                {
                    ++count;
                }
                return count;
            }
        }
    }
}

