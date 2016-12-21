using NUnit.Framework;

using System;
using System.Collections.Generic;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

namespace LunarPlugin.Test.Timers
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class TimerManagerTest
    {
        private List<Action> callbacks = new List<Action>();

        [SetUp]
        public void SetUp()
        {
            callbacks.Clear();
            TestTimer.FreeRoot = null;
        }

        [Test]
        public void TestSortingTimers1()
        {
            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.0f,  "timer1");
            manager.Schedule(TimerCallback1, 0.25f, "timer2");
            manager.Schedule(TimerCallback1, 0.25f, "timer3");
            manager.Schedule(TimerCallback1, 0.5f,  "timer4");
            manager.Schedule(TimerCallback1, 0.75f, "timer5");

            CTimer timer = manager.RootTimer;
            Assert.AreEqual(timer.name, "timer1"); timer = NextTimer(timer);
            Assert.AreEqual(timer.name, "timer2"); timer = NextTimer(timer);
            Assert.AreEqual(timer.name, "timer3"); timer = NextTimer(timer);
            Assert.AreEqual(timer.name, "timer4"); timer = NextTimer(timer);
            Assert.AreEqual(timer.name, "timer5"); timer = NextTimer(timer);
            Assert.IsNull(timer);
        }

        [Test]
        public void TestSortingTimers2()
        {
            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.75f, "timer1");
            manager.Schedule(TimerCallback1, 0.5f,  "timer2");
            manager.Schedule(TimerCallback1, 0.25f, "timer3");
            manager.Schedule(TimerCallback1, 0.25f, "timer4");
            manager.Schedule(TimerCallback1, 0.0f,  "timer5");

            CTimer timer = manager.RootTimer;
            Assert.AreEqual(timer.name, "timer5"); timer = NextTimer(timer);
            Assert.AreEqual(timer.name, "timer3"); timer = NextTimer(timer);
            Assert.AreEqual(timer.name, "timer4"); timer = NextTimer(timer);
            Assert.AreEqual(timer.name, "timer2"); timer = NextTimer(timer);
            Assert.AreEqual(timer.name, "timer1"); timer = NextTimer(timer);
            Assert.IsNull(timer);
        }

        [Test]
        public void TestScheduleTimer()
        {
            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.5f);

            manager.Update(0.25f);
            AssertCallbacks();
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback1);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback1);
            Assert.AreEqual(0, manager.Count());
        }

        [Test]
        public void TestScheduleTimers()
        {
            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);

            manager.Update(0.25f);
            AssertCallbacks();
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(0, manager.Count());
        }

        [Test]
        public void TestScheduleMoreTimers()
        {
            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);
            manager.Schedule(TimerCallback3, 0.5f);
            manager.Schedule(TimerCallback4, 1.0f);

            manager.Update(0.25f);
            AssertCallbacks();
            Assert.AreEqual(4, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback3);
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback3, TimerCallback1);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback2, TimerCallback3, TimerCallback1, TimerCallback4);
            Assert.AreEqual(0, manager.Count());
        }

        [Test]
        public void TestScheduleMoreTimersLater()
        {
            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);

            manager.Update(0.25f); // 0.25
            AssertCallbacks();
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 0.5
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallback3, 1.0f); // fires at 1.5
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 0.75
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f); // 1.0
            AssertCallbacks(TimerCallback2, TimerCallback1);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallback4, 0.15f); // fires at 1.15
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 1.25
            AssertCallbacks(TimerCallback2, TimerCallback1, TimerCallback4);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f); // 1.5
            AssertCallbacks(TimerCallback2, TimerCallback1, TimerCallback4, TimerCallback3);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f); // 1.75
            AssertCallbacks(TimerCallback2, TimerCallback1, TimerCallback4, TimerCallback3);
            Assert.AreEqual(0, manager.Count());
        }

        [Test]
        public void TestCancelTimer()
        {
            TestTimerManager manager = new TestTimerManager();
            manager.Schedule(TimerCallback1, 0.75f);
            manager.Schedule(TimerCallback2, 0.5f);

            manager.Update(0.25f); // 0.25
            AssertCallbacks();
            Assert.AreEqual(2, manager.Count());

            manager.Update(0.25f); // 0.5
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallback3, 1.0f); // fires at 1.5
            Assert.AreEqual(2, manager.Count());

            manager.Cancel(TimerCallback1);

            manager.Update(0.25f); // 0.75
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f); // 1.0
            AssertCallbacks(TimerCallback2);
            Assert.AreEqual(1, manager.Count());

            manager.Schedule(TimerCallback4, 0.15f); // fires at 1.15
            Assert.AreEqual(2, manager.Count());

            manager.Cancel(TimerCallback3);

            manager.Update(0.25f); // 1.25
            AssertCallbacks(TimerCallback2, TimerCallback4);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f); // 1.5
            AssertCallbacks(TimerCallback2, TimerCallback4);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f); // 1.75
            AssertCallbacks(TimerCallback2, TimerCallback4);
            Assert.AreEqual(0, manager.Count());
        }

        [Test]
        public void TestCancelTimerInLoop()
        {
            TestTimerManager manager = new TestTimerManager();

            Action dummyCallback = () => {};
            Action callback = () =>
            {
                callbacks.Add(dummyCallback);
                Assert.AreEqual(0, manager.FreeDelayedCount);

                manager.Cancel(TimerCallback1);
                Assert.AreEqual(1, manager.FreeDelayedCount);

                manager.Cancel(TimerCallback2);
                Assert.AreEqual(2, manager.FreeDelayedCount);
            };

            manager.Schedule(callback, 0.25f);
            manager.Schedule(TimerCallback1, 0.25f);
            manager.Schedule(TimerCallback2, 0.25f);
            manager.Schedule(TimerCallback3, 0.25f);

            manager.Update(0.25f);
            Assert.AreEqual(0, manager.FreeDelayedCount);

            AssertCallbacks(dummyCallback, TimerCallback3);
            Assert.AreEqual(0, manager.Count());
        }

        [Test]
        public void TestAddTimerOnce()
        {
            TestTimerManager manager = new TestTimerManager();
            manager.ScheduleOnce(TimerCallback1, 0.25f);
            manager.ScheduleOnce(TimerCallback2, 0.25f);
            manager.ScheduleOnce(TimerCallback3, 0.25f);
            manager.ScheduleOnce(TimerCallback1, 0.25f);
            manager.ScheduleOnce(TimerCallback2, 0.25f);

            Assert.AreEqual(3, manager.Count());

            manager.Update(0.25f);
            AssertCallbacks(TimerCallback1, TimerCallback2, TimerCallback3);
            Assert.AreEqual(0, manager.Count());
        }

        [Test]
        public void TestAddDelayedTimer()
        {
            TestTimerManager manager = new TestTimerManager();

            Action dummyCallback = () => {};
            Action callback = () =>
            {
                callbacks.Add(dummyCallback);
                manager.Schedule(TimerCallback1, 0.25f);
                manager.Schedule(TimerCallback2, 0.25f);
                manager.Schedule(TimerCallback3, 0.25f);
            };

            manager.Schedule(callback, 0.25f);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f); // 0.25
            AssertCallbacks(dummyCallback);
            Assert.AreEqual(3, manager.Count());

            manager.Update(0.25f); // 0.5
            AssertCallbacks(dummyCallback, TimerCallback1, TimerCallback2, TimerCallback3);
            Assert.AreEqual(0, manager.Count());
        }

        [Test]
        public void TestWithException()
        {
            TestTimerManager manager = new TestTimerManager();

            Action dummyCallback = () => {};
            Action callback = () =>
            {
                callbacks.Add(dummyCallback);
                throw new Exception();
            };

            manager.Schedule(callback, 0.25f);
            Assert.AreEqual(1, manager.Count());

            manager.Update(0.25f); // 0.25

            AssertCallbacks(dummyCallback);
            Assert.AreEqual(0, manager.Count());

            manager.Update(0.25f); // 0.25
            AssertCallbacks(dummyCallback);
            Assert.AreEqual(0, manager.Count());
        }

        private void TimerCallback1()
        {
            callbacks.Add(TimerCallback1);
        }

        private void TimerCallback2()
        {
            callbacks.Add(TimerCallback2);
        }

        private void TimerCallback3()
        {
            callbacks.Add(TimerCallback3);
        }

        private void TimerCallback4()
        {
            callbacks.Add(TimerCallback4);
        }

        private void AssertCallbacks(params Action[] expected)
        {
            Action[] actual = GetCallbacks();
            Assert.AreEqual(expected.Length, actual.Length);

            String message = "";
            for (int i = 0; i < expected.Length; ++i)
            {
                if (expected[i] != actual[i])
                {
                    message += expected[i] + "!=" + actual[i];
                }
            }

            Assert.IsTrue(message.Length == 0, message);
        }

        private Action[] GetCallbacks()
        {
            Action[] array = new Action[callbacks.Count];
            callbacks.CopyTo(array);
            return array;
        }

        private static CTimer NextTimer(CTimer timer)
        {
            return CClassUtils.GetObjectField<CTimer>(timer, "next");
        }
    }

    class TestTimerManager : CTimerManager
    {
        public CTimer Schedule(Action callback, float delay, string name)
        {
            return Schedule(callback, delay, false, name);
        }

        public new CTimer RootTimer
        {
            get { return base.RootTimer; }
        }

        public int FreeDelayedCount
        {
            get
            {
                int count = 0;
                for (CTimer t = DelayedFreeHeadTimer; t != null; t = TestTimer.NextHelperListTimer(t))
                {
                    ++count;
                }

                return count;
            }
        }
    }
}

