using System;

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
