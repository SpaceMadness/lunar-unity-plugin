//
//  CTimerManager.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

﻿using System;

using LunarPlugin;

namespace LunarPluginInternal
{
    class CTimerManager : ICTimerManager
    {
        public static readonly ICTimerManager Null = new CNullTimerManager();

        private static CTimerManager s_sharedInstance;

        internal double currentTime;

        private CTimer rootTimer;

        private CTimer delayedAddHeadTimer; // timers which were scheduled while iterating the list
        private CTimer delayedAddTailTimer; // track tail to append at the end of the list so timers are
                                           // fired in the same order as they scheduled

        private CTimer delayedFreeRootTimer; // timers which were cancelled while iterating the list

        private int timersCount;
        private bool updating;

        //////////////////////////////////////////////////////////////////////////////

        #region Shared instance

        static CTimerManager()
        {
            s_sharedInstance = new CTimerManager();
        }

        public static CTimer ScheduleTimer(Action callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return s_sharedInstance.Schedule(callback, delay, repeated, name);
        }

        public static CTimer ScheduleTimer(Action<CTimer> callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return s_sharedInstance.Schedule(callback, delay, repeated, name);
        }

        public static CTimer ScheduleTimerOnce(Action callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return s_sharedInstance.ScheduleOnce(callback, delay, repeated, name);
        }

        public static CTimer ScheduleTimerOnce(Action<CTimer> callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return s_sharedInstance.ScheduleOnce(callback, delay, repeated, name);
        }

        public static void CancelTimer(Action callback)
        {
            s_sharedInstance.Cancel(callback);
        }

        public static void CancelTimer(Action<CTimer> callback)
        {
            s_sharedInstance.Cancel(callback);
        }

        public static void CancelTimers(object target)
        {
            s_sharedInstance.CancelAll(target);
        }

        internal static CTimerManager SharedInstance
        {
            get { return s_sharedInstance; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public override void Update(float delta)
        {
            lock (this)
            {
                currentTime += delta;

                if (timersCount > 0)
                {
                    updating = true;
                    for (CTimer t = rootTimer; t != null;)
                    {
                        if (t.fireTime > currentTime)
                        {
                            break;
                        }

                        CTimer timer = t;
                        t = t.next;

                        if (!timer.cancelled)
                        {
                            timer.Fire();
                        }
                    }
                    updating = false;
                
                    // Put timers which were cancelled during this update back into the pool
                    if (delayedFreeRootTimer != null)
                    {
                        for (CTimer t = delayedFreeRootTimer; t != null;)
                        {
                            CTimer timer = t;
                            t = t.helpListNext;

                            CancelTimerInLoop(timer);
                        }
                        delayedFreeRootTimer = null;
                    }

                    // Add timers which were scheduled during this update
                    if (delayedAddHeadTimer != null)
                    {
                        for (CTimer t = delayedAddHeadTimer; t != null;)
                        {
                            CTimer timer = t;
                            t = t.helpListNext;

                            AddTimer(timer);
                        }
                        delayedAddHeadTimer = null;
                        delayedAddTailTimer = null;
                    }
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Schedule

        public override CTimer Schedule(Action callback, float delay, int numRepeats, string name = null)
        {
            return Schedule(callback, CTimer.DefaultTimerCallback, delay, numRepeats, name);
        }

        public override CTimer Schedule(Action<CTimer> callback, float delay, int numRepeats, string name = null)
        {
            return Schedule(null, callback, delay, numRepeats, name);
        }

        private CTimer Schedule(Action callback1, Action<CTimer> callback2, float delay, int numRepeats, string name)
        {
            float timeout = delay < 0 ? 0 : delay;

            CTimer timer = NextFreeTimer();
            timer.callback1 = callback1;
            timer.callback2 = callback2;
            timer.timeout = timeout;
            timer.numRepeats = numRepeats;
            timer.scheduleTime = currentTime;
            timer.fireTime = currentTime + timeout;
            timer.name = name;

            lock (this)
            {
                if (updating)
                {
                    AddTimerDelayed(timer);
                }
                else
                {
                    AddTimer(timer);
                }
            }

            return timer;
        }

        protected override CTimer FindTimer(Action callback)
        {
            for (CTimer timer = rootTimer; timer != null; timer = timer.next)
            {
                if (timer.callback1 == callback)
                {
                    return timer;
                }
            }

            return null;
        }

        protected override CTimer FindTimer(Action<CTimer> callback)
        {
            for (CTimer timer = rootTimer; timer != null; timer = timer.next)
            {
                if (timer.callback2 == callback)
                {
                    return timer;
                }
            }

            return null;
        }

        public override void Cancel(Action callback)
        {
            lock (this)
            {
                for (CTimer timer = rootTimer; timer != null;)
                {
                    CTimer t = timer;
                    timer = timer.next;

                    if (t.callback1 == callback)
                    {
                        t.Cancel();
                    }
                }
            }
        }

        public override void Cancel(Action<CTimer> callback)
        {
            lock (this)
            {
                for (CTimer timer = rootTimer; timer != null;)
                {
                    CTimer t = timer;
                    timer = timer.next;

                    if (t.callback2 == callback)
                    {
                        t.Cancel();
                    }
                }
            }
        }

        public override void Cancel(string name)
        {
            lock (this)
            {
                for (CTimer timer = rootTimer; timer != null;)
                {
                    CTimer t = timer;
                    timer = timer.next;

                    if (t.name == name)
                    {
                        t.Cancel();
                    }
                }
            }
        }

        public override void CancelAll(Object target)
        {
            lock (this)
            {
                for (CTimer timer = rootTimer; timer != null;)
                {
                    CTimer t = timer;
                    timer = timer.next;

                    if (t.callback1 != null && t.callback1.Target == target || t.callback2.Target == target)
                    {
                        t.Cancel();
                    }
                }
            }
        }

        public override void CancelAll()
        {
            lock (this)
            {
                for (CTimer timer = rootTimer; timer != null;)
                {
                    CTimer t = timer;
                    timer = timer.next;

                    t.Cancel();
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Destroyable

        public override void Destroy()
        {
            CancelAll();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Timer List

        private CTimer NextFreeTimer()
        {
            CTimer timer = CTimer.NextFreeTimer();
            timer.manager = this;
            return timer;
        }

        private void AddTimerDelayed(CTimer timer)
        {
            if (delayedAddHeadTimer == null)
            {
                delayedAddHeadTimer = timer; // beginning of the list
            }

            if (delayedAddTailTimer != null)
            {
                delayedAddTailTimer.helpListNext = timer;
            }
            delayedAddTailTimer = timer;
        }

        private void AddFreeTimerDelayed(CTimer timer)
        {
            timer.helpListNext = delayedFreeRootTimer;
            delayedFreeRootTimer = timer;
        }

        private void AddFreeTimer(CTimer timer)
        {
            CTimer.AddFreeTimer(timer);
        }

        private void AddTimer(CTimer timer)
        {
            CAssert.AreSame(this, timer.manager);
            ++timersCount;

            if (rootTimer != null)
            {
                // if timer has the least remaining time - it goes first
                if (timer.fireTime < rootTimer.fireTime)
                {
                    timer.next = rootTimer;
                    rootTimer.prev = timer;
                    rootTimer = timer;

                    return;
                }

                // try to insert in a sorted order
                CTimer tail = rootTimer;
                for (CTimer t = rootTimer.next; t != null; tail = t, t = t.next)
                {
                    if (timer.fireTime < t.fireTime)
                    {
                        CTimer prev = t.prev;
                        CTimer next = t;

                        timer.prev = prev;
                        timer.next = next;

                        next.prev = timer;
                        prev.next = timer;

                        return;
                    }
                }

                // add timer at the end of the list
                tail.next = timer;
                timer.prev = tail;
            }
            else
            {
                rootTimer = timer; // timer is root now
            }
        }

        private void RemoveTimer(CTimer timer)
        {
            CAssert.AreSame(this, timer.manager);
            CAssert.Greater(timersCount, 0);
            --timersCount;

            CTimer prev = timer.prev;
            CTimer next = timer.next;

            if (prev != null)
                prev.next = next;
            else
                rootTimer = next;

            if (next != null)
                next.prev = prev;
        }

        internal void CancelTimer(CTimer timer)
        {   
            lock (this)
            {
                if (updating)
                {
                    AddFreeTimerDelayed(timer);
                }
                else
                {
                    CancelTimerInLoop(timer);
                }
            }
        }

        private void CancelTimerInLoop(CTimer timer)
        {
            RemoveTimer(timer);
            AddFreeTimer(timer);
        }

        public override int Count()
        {
            lock (this)
            {
                return timersCount;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        protected CTimer RootTimer
        {
            get { return rootTimer; }
        }

        protected CTimer DelayedFreeHeadTimer
        {
            get { return delayedFreeRootTimer; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #if LUNAR_DEVELOPMENT

        public static void CancelTimers()
        {
            s_sharedInstance.CancelAll();
        }

        public static void RunUpdate(float delta)
        {
            s_sharedInstance.Update(delta);
        }

        #endif
    }

    internal class CNullTimerManager : ICTimerManager
    {
        public override CTimer Schedule(Action callback, float delay, int numRepeats, string name = null)
        {
            throw new InvalidOperationException("Can't schedule timer on a 'null' timer manager");
        }

        public override CTimer Schedule(Action<CTimer> callback, float delay, int numRepeats, string name = null)
        {
            throw new InvalidOperationException("Can't schedule timer on a 'null' timer manager");
        }

        public override void Cancel(Action callback)
        {   
        }

        public override void Cancel(Action<CTimer> callback)
        {
        }

        public override void Cancel(string name)
        {
        }

        public override void CancelAll()
        {
        }

        public override void CancelAll(object target)
        {
        }

        protected override CTimer FindTimer(Action callback)
        {
            return null;
        }

        protected override CTimer FindTimer(Action<CTimer> callback)
        {
            return null;
        }

        public override int Count()
        {
            return 0;
        }
    }
}