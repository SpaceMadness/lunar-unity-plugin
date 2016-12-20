//
//  TimerManager.cs
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
    class TimerManager : ITimerManager
    {
        public static readonly ITimerManager Null = new NullTimerManager();

        private static TimerManager s_sharedInstance;

        internal double currentTime;

        private Timer rootTimer;

        private Timer delayedAddHeadTimer; // timers which were scheduled while iterating the list
        private Timer delayedAddTailTimer; // track tail to append at the end of the list so timers are
                                           // fired in the same order as they scheduled

        private Timer delayedFreeRootTimer; // timers which were cancelled while iterating the list

        private int timersCount;
        private bool updating;

        //////////////////////////////////////////////////////////////////////////////

        #region Shared instance

        static TimerManager()
        {
            s_sharedInstance = new TimerManager();
        }

        public static Timer ScheduleTimer(Action callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return s_sharedInstance.Schedule(callback, delay, repeated, name);
        }

        public static Timer ScheduleTimer(Action<Timer> callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return s_sharedInstance.Schedule(callback, delay, repeated, name);
        }

        public static Timer ScheduleTimerOnce(Action callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return s_sharedInstance.ScheduleOnce(callback, delay, repeated, name);
        }

        public static Timer ScheduleTimerOnce(Action<Timer> callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return s_sharedInstance.ScheduleOnce(callback, delay, repeated, name);
        }

        public static void CancelTimer(Action callback)
        {
            s_sharedInstance.Cancel(callback);
        }

        public static void CancelTimer(Action<Timer> callback)
        {
            s_sharedInstance.Cancel(callback);
        }

        public static void CancelTimers(object target)
        {
            s_sharedInstance.CancelAll(target);
        }

        internal static TimerManager SharedInstance
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
                    for (Timer t = rootTimer; t != null;)
                    {
                        if (t.fireTime > currentTime)
                        {
                            break;
                        }

                        Timer timer = t;
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
                        for (Timer t = delayedFreeRootTimer; t != null;)
                        {
                            Timer timer = t;
                            t = t.helpListNext;

                            CancelTimerInLoop(timer);
                        }
                        delayedFreeRootTimer = null;
                    }

                    // Add timers which were scheduled during this update
                    if (delayedAddHeadTimer != null)
                    {
                        for (Timer t = delayedAddHeadTimer; t != null;)
                        {
                            Timer timer = t;
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

        public override Timer Schedule(Action callback, float delay, int numRepeats, string name = null)
        {
            return Schedule(callback, Timer.DefaultTimerCallback, delay, numRepeats, name);
        }

        public override Timer Schedule(Action<Timer> callback, float delay, int numRepeats, string name = null)
        {
            return Schedule(null, callback, delay, numRepeats, name);
        }

        private Timer Schedule(Action callback1, Action<Timer> callback2, float delay, int numRepeats, string name)
        {
            float timeout = delay < 0 ? 0 : delay;

            Timer timer = NextFreeTimer();
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

        protected override Timer FindTimer(Action callback)
        {
            for (Timer timer = rootTimer; timer != null; timer = timer.next)
            {
                if (timer.callback1 == callback)
                {
                    return timer;
                }
            }

            return null;
        }

        protected override Timer FindTimer(Action<Timer> callback)
        {
            for (Timer timer = rootTimer; timer != null; timer = timer.next)
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
                for (Timer timer = rootTimer; timer != null;)
                {
                    Timer t = timer;
                    timer = timer.next;

                    if (t.callback1 == callback)
                    {
                        t.Cancel();
                    }
                }
            }
        }

        public override void Cancel(Action<Timer> callback)
        {
            lock (this)
            {
                for (Timer timer = rootTimer; timer != null;)
                {
                    Timer t = timer;
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
                for (Timer timer = rootTimer; timer != null;)
                {
                    Timer t = timer;
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
                for (Timer timer = rootTimer; timer != null;)
                {
                    Timer t = timer;
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
                for (Timer timer = rootTimer; timer != null;)
                {
                    Timer t = timer;
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

        private Timer NextFreeTimer()
        {
            Timer timer = Timer.NextFreeTimer();
            timer.manager = this;
            return timer;
        }

        private void AddTimerDelayed(Timer timer)
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

        private void AddFreeTimerDelayed(Timer timer)
        {
            timer.helpListNext = delayedFreeRootTimer;
            delayedFreeRootTimer = timer;
        }

        private void AddFreeTimer(Timer timer)
        {
            Timer.AddFreeTimer(timer);
        }

        private void AddTimer(Timer timer)
        {
            Assert.AreSame(this, timer.manager);
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
                Timer tail = rootTimer;
                for (Timer t = rootTimer.next; t != null; tail = t, t = t.next)
                {
                    if (timer.fireTime < t.fireTime)
                    {
                        Timer prev = t.prev;
                        Timer next = t;

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

        private void RemoveTimer(Timer timer)
        {
            Assert.AreSame(this, timer.manager);
            Assert.Greater(timersCount, 0);
            --timersCount;

            Timer prev = timer.prev;
            Timer next = timer.next;

            if (prev != null)
                prev.next = next;
            else
                rootTimer = next;

            if (next != null)
                next.prev = prev;
        }

        internal void CancelTimer(Timer timer)
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

        private void CancelTimerInLoop(Timer timer)
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

        protected Timer RootTimer
        {
            get { return rootTimer; }
        }

        protected Timer DelayedFreeHeadTimer
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

    internal class NullTimerManager : ITimerManager
    {
        public override Timer Schedule(Action callback, float delay, int numRepeats, string name = null)
        {
            throw new InvalidOperationException("Can't schedule timer on a 'null' timer manager");
        }

        public override Timer Schedule(Action<Timer> callback, float delay, int numRepeats, string name = null)
        {
            throw new InvalidOperationException("Can't schedule timer on a 'null' timer manager");
        }

        public override void Cancel(Action callback)
        {   
        }

        public override void Cancel(Action<Timer> callback)
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

        protected override Timer FindTimer(Action callback)
        {
            return null;
        }

        protected override Timer FindTimer(Action<Timer> callback)
        {
            return null;
        }

        public override int Count()
        {
            return 0;
        }
    }
}