using NUnit.Framework;
using System;
using System.Collections.Generic;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

namespace LunarPlugin.Test
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture()]
    public class IteratorTest
    {
        [Test()]
        public void TestForwardIteration()
        {
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");

            int index = 0;

            CIterator<string> iter = new CIterator<string>(list);
            while (iter.HasNext())
            {
                Assert.AreEqual(list[index++], iter.Next());
            }

            Assert.AreEqual(list.Count, index);
        }

        [Test()]
        public void TestSkip()
        {
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");

            int iterations = 0;

            CIterator<string> iter = new CIterator<string>(list);
            while (iter.HasNext())
            {
                iter.Skip();
                ++iterations;
            }

            Assert.AreEqual(list.Count, iterations);
        }

        [Test()]
        public void TestSkipSomeElements()
        {
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");

            CIterator<string> iter = new CIterator<string>(list);

            Assert.IsTrue(iter.HasNext());
            iter.Skip();

            Assert.IsTrue(iter.HasNext());
            Assert.AreEqual(list [1], iter.Next());

            Assert.IsTrue(iter.HasNext());
            iter.Skip();

            Assert.IsFalse(iter.HasNext());
        }

        [Test()]
        public void TestHasNext()
        {
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");

            CIterator<string> iter = new CIterator<string>(list);

            Assert.IsTrue(iter.HasNext(1));
            Assert.IsTrue(iter.HasNext(2));
            Assert.IsTrue(iter.HasNext(3));
            Assert.IsFalse(iter.HasNext(4));
        }

        [Test()]
        public void TestSkipMultiple()
        {
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");

            CIterator<string> iter = new CIterator<string>(list);

            iter.Skip(2);
            Assert.IsTrue(iter.HasNext());
            Assert.AreEqual(list[2], iter.Next());
        }
    }
}

