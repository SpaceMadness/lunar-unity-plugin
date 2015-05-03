using System;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

namespace LunarPlugin.Test.Timers
{
    class TestTimer : Timer
    {
        public static void Reset()
        {
            Timer.FreeRoot = null;
        }

        public static new Timer FreeRoot
        {
            get { return Timer.FreeRoot; }
            set { Timer.FreeRoot = value; }
        }

        public new static Timer NextFreeTimer()
        {
            return Timer.NextFreeTimer();
        }

        public new static void AddFreeTimer(Timer timer)
        {
            Timer.AddFreeTimer(timer);
        }

        public static new Timer NextTimer(Timer t)
        {
            return Timer.NextTimer(t);
        }

        public static new Timer NextHelperListTimer(Timer t)
        {
            return Timer.NextHelperListTimer(t);
        }

        public static int PoolSize
        {
            get
            {
                int count = 0;
                for (Timer t = FreeRoot; t != null; t = Timer.NextTimer(t))
                {
                    ++count;
                }
                return count;
            }
        }
    }
}

