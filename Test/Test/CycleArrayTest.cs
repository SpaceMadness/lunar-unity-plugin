using System;

using NUnit.Framework;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

namespace LunarPlugin.Test
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class CycleArrayTest
    {
        [Test]
        public void TestAddElements()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);

            Assert.AreEqual(3, array.Capacity);
            Assert.AreEqual(1, array.Length);
            Assert.AreEqual(1, array.RealLength);
            AssertArray(array, 1);
        }

        [Test]
        public void TestAddElements2()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);

            Assert.AreEqual(3, array.Capacity);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(2, array.RealLength);
            AssertArray(array, 1, 2);
        }

        [Test]
        public void TestAddElements3()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);

            Assert.AreEqual(3, array.Capacity);
            Assert.AreEqual(3, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 1, 2, 3);
        }

        [Test]
        public void TestAddElements4()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);

            Assert.AreEqual(3, array.Capacity);
            Assert.AreEqual(4, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 2, 3, 4);
        }

        [Test]
        public void TestAddElements5()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);

            Assert.AreEqual(3, array.Capacity);
            Assert.AreEqual(5, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 3, 4, 5);
        }

        [Test]
        public void TestAddElements6()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);
            array.Add(6);

            Assert.AreEqual(3, array.Capacity);
            Assert.AreEqual(6, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 4, 5, 6);
        }

        [Test]
        public void TestAddElements7()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);
            array.Add(6);
            array.Add(7);

            Assert.AreEqual(3, array.Capacity);
            Assert.AreEqual(7, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 5, 6, 7);
        }

        [Test]
        public void TestAddElements8()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);
            array.Add(6);
            array.Add(7);
            array.Add(8);

            Assert.AreEqual(3, array.Capacity);
            Assert.AreEqual(8, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 6, 7, 8);
        }

        [Test]
        public void TestAddElements9()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);
            array.Add(6);
            array.Add(7);
            array.Add(8);
            array.Add(9);

            Assert.AreEqual(3, array.Capacity);
            Assert.AreEqual(9, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 7, 8, 9);
        }

        [Test]
        public void TestGrowCapacity()
        {
            CycleArray<int> array = new CycleArray<int>(5);
            array.Add(1);
            array.Add(2);
            array.Add(3);

            array.Capacity = 10;

            Assert.AreEqual(10, array.Capacity);
            Assert.AreEqual(3, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 1, 2, 3);
        }

        [Test]
        public void TestGrowCapacityForAFullArray()
        {
            CycleArray<int> array = new CycleArray<int>(5);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);

            array.Capacity = 10;

            Assert.AreEqual(10, array.Capacity);
            Assert.AreEqual(5, array.Length);
            Assert.AreEqual(5, array.RealLength);
            AssertArray(array, 1, 2, 3, 4, 5);
        }

        [Test]
        public void TestGrowCapacityForOverflowedArrayWithOneExtraElement()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);

            array.Capacity = 10;

            Assert.AreEqual(10, array.Capacity);
            Assert.AreEqual(4, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 2, 3, 4);
        }

        [Test]
        public void TestGrowCapacityForOverflowedArrayWithTwoExtraElements()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);

            array.Capacity = 10;

            Assert.AreEqual(10, array.Capacity);
            Assert.AreEqual(5, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 3, 4, 5);
        }

        [Test]
        public void TestGrowCapacityForOverflowedArrayWithThreeExtraElements()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);
            array.Add(6);

            array.Capacity = 10;

            Assert.AreEqual(10, array.Capacity);
            Assert.AreEqual(6, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 4, 5, 6);
        }

        [Test]
        public void TestGrowCapacityForOverflowedArrayWithFourExtraElements()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);
            array.Add(6);
            array.Add(7);

            array.Capacity = 10;

            Assert.AreEqual(10, array.Capacity);
            Assert.AreEqual(7, array.Length);
            Assert.AreEqual(3, array.RealLength);
            AssertArray(array, 5, 6, 7);
        }

        [Test]
        public void TestGrowCapacityAndAddMoreElements()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);
            array.Add(6);
            array.Add(7);

            array.Capacity = 5;
            AssertArray(array, 5, 6, 7);
            Assert.AreEqual(5, array.Capacity);
            Assert.AreEqual(7, array.Length);
            Assert.AreEqual(3, array.RealLength);

            array.Add(8);
            array.Add(9);

            Assert.AreEqual(5, array.Capacity);
            Assert.AreEqual(9, array.Length);
            Assert.AreEqual(5, array.RealLength);
            AssertArray(array, 5, 6, 7, 8, 9);
        }

        [Test]
        public void TestGrowCapacityBiggerArray()
        {
            CycleArray<int> array = new CycleArray<int>(7);
            for (int i = 1; i <= 7; ++i)
            {
                array.Add(i);
            }

            array.Capacity = 9;
            AssertArray(array, 1, 2,3, 4, 5, 6, 7);

            array.Add(8);
            array.Add(9);

            Assert.AreEqual(9, array.Capacity);
            Assert.AreEqual(9, array.Length);
            Assert.AreEqual(9, array.RealLength);
            AssertArray(array, 1, 2,3, 4, 5, 6, 7, 8, 9);
        }

        [Test]
        public void TestGrowCapacityAndOverflowMultipleTimes()
        {
            CycleArray<int> array = new CycleArray<int>(3);
            for (int i = 0; i < 10; ++i)
            {
                array.Add(i + 1);
            }

            array.Capacity = 5;
            AssertArray(array, 8, 9, 10);

            array.Add(11);
            array.Add(12);

            Assert.AreEqual(5, array.Capacity);
            Assert.AreEqual(12, array.Length);
            Assert.AreEqual(5, array.RealLength);
            AssertArray(array, 8, 9, 10, 11, 12);
        }

        [Test]
        public void TestTrimLength()
        {
            CycleArray<int> array = new CycleArray<int>(5);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);

            array.TrimToLength(3);

            AssertArray(array, 1, 2, 3);
            Assert.AreEqual(0, array.HeadIndex);
            Assert.AreEqual(3, array.Length);
            Assert.AreEqual(3, array.RealLength);

            array.Add(6);
            array.Add(7);

            AssertArray(array, 1, 2, 3, 6, 7);
            Assert.AreEqual(0, array.HeadIndex);
            Assert.AreEqual(5, array.Length);
            Assert.AreEqual(5, array.RealLength);

            array.Add(8);
            array.Add(9);

            AssertArray(array, 3, 6, 7, 8, 9);
            Assert.AreEqual(2, array.HeadIndex);
            Assert.AreEqual(7, array.Length);
            Assert.AreEqual(5, array.RealLength);

            array.TrimToLength(4);

            AssertArray(array, 3, 6);
            Assert.AreEqual(2, array.HeadIndex);
            Assert.AreEqual(4, array.Length);
            Assert.AreEqual(2, array.RealLength);

            array.Add(10);
            array.Add(11);

            AssertArray(array, 3, 6, 10, 11);
            Assert.AreEqual(2, array.HeadIndex);
            Assert.AreEqual(6, array.Length);
            Assert.AreEqual(4, array.RealLength);

            array.TrimToLength(2);

            AssertArray(array);
            Assert.AreEqual(2, array.HeadIndex);
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(0, array.RealLength);

            array.Add(12);
            array.Add(13);
            array.Add(14);
            array.Add(15);
            array.Add(16);
            array.Add(17);
            array.Add(18);

            AssertArray(array, 14, 15, 16, 17, 18);
            Assert.AreEqual(4, array.HeadIndex);
            Assert.AreEqual(9, array.Length);
            Assert.AreEqual(5, array.RealLength);
        }

        [Test]
        public void TestTrimHeadIndex()
        {
            CycleArray<int> array = new CycleArray<int>(5);
            array.Add(1);
            array.Add(2);
            array.Add(3);
            array.Add(4);
            array.Add(5);

            array.TrimToHeadIndex(2);

            AssertArray(array, 3, 4, 5);
            Assert.AreEqual(2, array.HeadIndex);
            Assert.AreEqual(5, array.Length);
            Assert.AreEqual(3, array.RealLength);

            array.Add(6);
            array.Add(7);

            AssertArray(array, 3, 4, 5, 6, 7);
            Assert.AreEqual(2, array.HeadIndex);
            Assert.AreEqual(7, array.Length);
            Assert.AreEqual(5, array.RealLength);

            array.Add(8);
            array.Add(9);

            AssertArray(array, 5, 6, 7, 8, 9);
            Assert.AreEqual(4, array.HeadIndex);
            Assert.AreEqual(9, array.Length);
            Assert.AreEqual(5, array.RealLength);

            array.TrimToHeadIndex(7);

            AssertArray(array, 8, 9);
            Assert.AreEqual(7, array.HeadIndex);
            Assert.AreEqual(9, array.Length);
            Assert.AreEqual(2, array.RealLength);

            array.Add(10);
            array.Add(11);

            AssertArray(array, 8, 9, 10, 11);
            Assert.AreEqual(7, array.HeadIndex);
            Assert.AreEqual(11, array.Length);
            Assert.AreEqual(4, array.RealLength);

            array.TrimToHeadIndex(11);

            AssertArray(array);
            Assert.AreEqual(11, array.HeadIndex);
            Assert.AreEqual(11, array.Length);
            Assert.AreEqual(0, array.RealLength);

            array.Add(12);
            array.Add(13);
            array.Add(14);
            array.Add(15);
            array.Add(16);
            array.Add(17);
            array.Add(18);

            AssertArray(array, 14, 15, 16, 17, 18);
            Assert.AreEqual(13, array.HeadIndex);
            Assert.AreEqual(18, array.Length);
            Assert.AreEqual(5, array.RealLength);
        }

        private void AssertArray<T>(CycleArray<T> actual, params T[] expected)
        {
            Assert.AreEqual(expected.Length, actual.RealLength);
            for (int i = 0, j = actual.HeadIndex; i < expected.Length; ++i, ++j)
            {
                Assert.AreEqual(expected[i], actual[j]);
            }
        }
    }
}

