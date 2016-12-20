//
//  Timer.cs
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
    public class CTimer
    {
        internal static readonly object mutex = new object();

        internal static CTimer freeRoot;

        internal bool cancelled;

        internal Action callback1;
        internal Action<CTimer> callback2;

        internal CTimer next;
        internal CTimer prev;

        internal CTimer helpListNext;

        internal CTimerManager manager;

        internal int numRepeats;
        internal int numRepeated;

        internal float timeout;
        internal double fireTime;
        internal double scheduleTime;

        public string name;
        public Object userData;

        public void Cancel()
        {   
            lock (this)
            {
                if (!cancelled)
                {
                    cancelled = true;
                    manager.CancelTimer(this);
                }
            }
        }

        internal void Fire()
        {
            lock (this)
            {
                try
                {
                    callback2(this);

                    if (!cancelled)
                    {
                        ++numRepeated;
                        if (numRepeated == numRepeats)
                        {
                            Cancel();
                        }
                        else
                        {
                            fireTime = manager.currentTime + timeout;
                        }
                    }
                }
                catch (Exception e)
                {
                    CLog.error(e, "Exception while firing timer");
                    Cancel();
                }
            }
        }

        internal static void DefaultTimerCallback(CTimer timer)
        {
            timer.callback1();
        }

        public T UserData<T>() where T : class
        {
            return ClassUtils.Cast<T>(userData);
        }

        public bool IsRepeated
        {
            get { return numRepeats != 1; }
        }

        public float Timeout
        {
            get { return timeout; }
        }

        public float Elapsed
        {
            get { return (float) (manager.currentTime - scheduleTime); }
        }

        protected static CTimer FreeRoot
        {
            get { return freeRoot; }
            set { freeRoot = value; }
        }

        public override string ToString()
        {
            Delegate callback = callback2 != DefaultTimerCallback ? (Delegate)callback2 : (Delegate)callback1;
            return string.Format("[Target={0}, Method={1}, IsRepeated={2}, Timeout={3}, Elapsed={4}]",
                callback != null ? callback.Target : null,
                callback != null ? callback.Method : null,
                this.IsRepeated, 
                this.Timeout, 
                this.manager != null ? this.Elapsed : 0);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Objects pool

        internal protected static CTimer NextFreeTimer()
        {
            lock (mutex)
            {
                CTimer timer;
                if (freeRoot != null)
                {
                    timer = freeRoot;
                    freeRoot = timer.next;
                    timer.prev = timer.next = null;
                }
                else
                {
                    timer = new CTimer();
                }
            
                return timer;
            }
        }

        internal protected static void AddFreeTimer(CTimer timer)
        {
            lock (mutex)
            {
                timer.Reset();

                if (freeRoot != null)
                {
                    timer.next = freeRoot;
                }

                freeRoot = timer;
            }
        }

        #if LUNAR_DEVELOPMENT

        public static CTimer NextTimer(CTimer timer)
        {
            return timer.next;
        }

        public static CTimer PrevTimer(CTimer timer)
        {
            return timer.prev;
        }

        public static CTimer NextHelperListTimer(CTimer timer)
        {
            return timer.helpListNext;
        }

        #endif // LUNAR_DEVELOPMENT

        private void Reset()
        {
            next = prev = null;
            helpListNext = null;
            manager = null;
            callback1 = null;
            callback2 = null;
            numRepeats = numRepeated = 0;
            timeout = 0;
            fireTime = 0;
            scheduleTime = 0;
            cancelled = false;
            name = null;
            userData = null;
        }

        #endregion
    }
}