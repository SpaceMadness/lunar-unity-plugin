//
//  ITimerManager.cs
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

namespace LunarPluginInternal
{
    abstract class ITimerManager : IUpdatable, IDestroyable // not a good idea to name abstract class as interface, but what you gonna do
    {
        public virtual void Update(float delta)
        {
        }

        public virtual void Destroy()
        {
        }

        public Timer Schedule(Action callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return Schedule(callback, delay, repeated ? 0 : 1, name);
        }

        public Timer Schedule(Action<Timer> callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return Schedule(callback, delay, repeated ? 0 : 1, name);
        }

        public Timer ScheduleOnce(Action callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return ScheduleOnce(callback, delay, repeated ? 0 : 1, name);
        }

        public Timer ScheduleOnce(Action<Timer> callback, float delay = 0.0f, bool repeated = false, string name = null)
        {
            return ScheduleOnce(callback, delay, repeated ? 0 : 1, name);
        }

        public Timer ScheduleOnce(Action callback, float delay, int numRepeats, string name = null)
        {
            lock (this)
            {
                Timer timer = FindTimer(callback);
                if (timer != null)
                {
                    return timer;
                }

                return Schedule(callback, delay, numRepeats, name);
            }
        }

        public Timer ScheduleOnce(Action<Timer> callback, float delay, int numRepeats, string name = null)
        {
            lock (this)
            {
                Timer timer = FindTimer(callback);
                if (timer != null)
                {
                    return timer;
                }

                return Schedule(callback, delay, numRepeats, name);
            }
        }

        public abstract Timer Schedule(Action callback, float delay, int numRepeats, string name = null);
        public abstract Timer Schedule(Action<Timer> callback, float delay, int numRepeats, string name = null);

        public abstract void Cancel(Action callback);
        public abstract void Cancel(Action<Timer> callback);

        public abstract void Cancel(string name);
        public abstract void CancelAll();
        public abstract void CancelAll(object target);

        protected abstract Timer FindTimer(Action callback);
        protected abstract Timer FindTimer(Action<Timer> callback);

        public abstract int Count();
    }
}
