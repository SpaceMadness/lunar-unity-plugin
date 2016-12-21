using System;

using NUnit.Framework;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

namespace LunarPlugin.Test
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class CollectionTest : TestFixtureBase
    {
        [Test]
        public void TestEach()
        {
            string[] array = { "1", "2", "3" };
            CCollection.Each(array, delegate(String element)
            {
                AddResult(element);
            });

            AssertResult("1", "2", "3");
        }

        [Test]
        public void TestEachIndex()
        {
            string[] array = { "1", "2", "3" };
            CCollection.Each(array, delegate(String element, int index)
            {
                AddResult(index + ":" + element);
            });

            AssertResult("0:1", "1:2", "2:3");
        }

        [Test]
        public void TestMap()
        {
            string[] array = { "a", "b", "c" };

            char[] expected = { 'A', 'B', 'C' };
            char[] actual = CCollection.Map(array, delegate(String element)
            {
                return char.ToUpper(element[0]);
            });

            AssertArray(actual, expected);
        }

        [Test]
        public void TestMapIndex()
        {
            string[] array = { "a", "b", "c" };

            string[] expected = { "a0", "b1", "c2" };
            string[] actual = CCollection.Map(array, delegate(String element, int index)
            {
                return element + index;
            });

            AssertArray(actual, expected);
        }

        [SetUp]
        protected override void RunSetUp()
        {
            base.RunSetUp();
        }
    }
}

