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
    public class TimerTest
    {
        [SetUp]
        public void Foo()
        {
            TestTimer.Reset();
        }

        [Test]
        public void TestSingleTimerReuse()
        {
            Timer instance = NextTimer();
            Assert.AreEqual(0, TestTimer.PoolSize);

            Recycle(instance);
            Assert.AreEqual(1, TestTimer.PoolSize);

            Assert.AreSame(instance, NextTimer());
            Assert.AreEqual(0, TestTimer.PoolSize);
        }

        [Test]
        public void TestMultipleTimerReuse()
        {
            Timer instance1 = NextTimer();
            Assert.AreEqual(0, TestTimer.PoolSize);

            Timer instance2 = NextTimer();
            Assert.AreEqual(0, TestTimer.PoolSize);

            Recycle(instance1);
            Assert.AreEqual(1, TestTimer.PoolSize);

            Recycle(instance2);
            Assert.AreEqual(2, TestTimer.PoolSize);

            Assert.AreSame(instance2, NextTimer());
            Assert.AreEqual(1, TestTimer.PoolSize);

            Assert.AreSame(instance1, NextTimer());
            Assert.AreEqual(0, TestTimer.PoolSize);
        }

        [Test]
        public void TestMultipleTimerMultipleReuse()
        {
            Timer instance1 = NextTimer();
            Assert.AreEqual(0, TestTimer.PoolSize);

            Timer instance2 = NextTimer();
            Assert.AreEqual(0, TestTimer.PoolSize);

            Recycle(instance1);
            Assert.AreEqual(1, TestTimer.PoolSize);

            Timer instance3 = NextTimer();
            Assert.AreEqual(0, TestTimer.PoolSize);

            Recycle(instance2);
            Assert.AreEqual(1, TestTimer.PoolSize);

            Assert.AreSame(instance2, NextTimer());
            Assert.AreEqual(0, TestTimer.PoolSize);

            Recycle(instance3);
            Assert.AreEqual(1, TestTimer.PoolSize);

            Assert.AreSame(instance3, NextTimer());
            Assert.AreEqual(0, TestTimer.PoolSize);
        }

        private static Timer NextTimer()
        {
            return TestTimer.NextFreeTimer();
        }

        private static void Recycle(Timer instance)
        {
            TestTimer.AddFreeTimer(instance);
        }
    }
}

