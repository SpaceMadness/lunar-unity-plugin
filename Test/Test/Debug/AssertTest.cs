using NUnit.Framework;
using System;
using System.Collections.Generic;

using UnityEngine;
using LunarPlugin;
using LunarPluginInternal;

namespace LunarPlugin.Test
{
    using Assert = NUnit.Framework.Assert;
    using LunarAssert = LunarPlugin.CAssert;

    [TestFixture]
    public class AssertTest : TestFixtureBase
    {
        private List<string> messages;

        [SetUp]
        public void SetUp()
        {
            messages = new List<string>();
            CTestingPlatform.AssertDelegate = delegate(string message, string stackTrace)
            {
                messages.Add(message);
            };
        }

        [TearDown]
        public void TearDown()
        {
            CTestingPlatform.AssertDelegate = null;
        }

        [Test]
        public void TestAssertIsTrue()
        {
            LunarAssert.IsTrue(true);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.IsTrue(false);
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertIsFalse()
        {
            LunarAssert.IsFalse(false);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.IsFalse(true);
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertIsNull()
        {
            LunarAssert.IsNull(null);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.IsNull(string.Empty);
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertIsNotNull()
        {
            LunarAssert.IsNotNull(string.Empty);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.IsNotNull(null);
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertEquality()
        {
            //bool
            messages.Clear();

            bool boolExpected = true;
            bool boolActual = true;

            LunarAssert.AreEqual(boolExpected, boolActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(boolExpected, boolActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            boolExpected = true;
            boolActual = false;

            LunarAssert.AreNotEqual(boolExpected, boolActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(boolExpected, boolActual);
            Assert.AreEqual(1, messages.Count);

            //byte
            messages.Clear();

            byte byteExpected = 10;
            byte byteActual = 10;

            LunarAssert.AreEqual(byteExpected, byteActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(byteExpected, byteActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            byteExpected = 10;
            byteActual = 11;

            LunarAssert.AreNotEqual(byteExpected, byteActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(byteExpected, byteActual);
            Assert.AreEqual(1, messages.Count);

            //sbyte
            messages.Clear();

            sbyte sbyteExpected = 10;
            sbyte sbyteActual = 10;

            LunarAssert.AreEqual(sbyteExpected, sbyteActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(sbyteExpected, sbyteActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            sbyteExpected = 10;
            sbyteActual = 11;

            LunarAssert.AreNotEqual(sbyteExpected, sbyteActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(sbyteExpected, sbyteActual);
            Assert.AreEqual(1, messages.Count);

            //short
            messages.Clear();

            short shortExpected = 10;
            short shortActual = 10;

            LunarAssert.AreEqual(shortExpected, shortActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(shortExpected, shortActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            shortExpected = 10;
            shortActual = 11;

            LunarAssert.AreNotEqual(shortExpected, shortActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(shortExpected, shortActual);
            Assert.AreEqual(1, messages.Count);

            //ushort
            messages.Clear();

            ushort ushortExpected = 10;
            ushort ushortActual = 10;

            LunarAssert.AreEqual(ushortExpected, ushortActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(ushortExpected, ushortActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            ushortExpected = 10;
            ushortActual = 11;

            LunarAssert.AreNotEqual(ushortExpected, ushortActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(ushortExpected, ushortActual);
            Assert.AreEqual(1, messages.Count);

            //char
            messages.Clear();

            char charExpected = 'a';
            char charActual = 'a';

            LunarAssert.AreEqual(charExpected, charActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(charExpected, charActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            charExpected = 'a';
            charActual = 'b';

            LunarAssert.AreNotEqual(charExpected, charActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(charExpected, charActual);
            Assert.AreEqual(1, messages.Count);

            //int
            messages.Clear();

            int intExpected = 10;
            int intActual = 10;

            LunarAssert.AreEqual(intExpected, intActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(intExpected, intActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            intExpected = 10;
            intActual = 11;

            LunarAssert.AreNotEqual(intExpected, intActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(intExpected, intActual);
            Assert.AreEqual(1, messages.Count);

            //uint
            messages.Clear();

            uint uintExpected = 10;
            uint uintActual = 10;

            LunarAssert.AreEqual(uintExpected, uintActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(uintExpected, uintActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            uintExpected = 10;
            uintActual = 11;

            LunarAssert.AreNotEqual(uintExpected, uintActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(uintExpected, uintActual);
            Assert.AreEqual(1, messages.Count);

            //long
            messages.Clear();

            long longExpected = 10;
            long longActual = 10;

            LunarAssert.AreEqual(longExpected, longActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(longExpected, longActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            longExpected = 10;
            longActual = 11;

            LunarAssert.AreNotEqual(longExpected, longActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(longExpected, longActual);
            Assert.AreEqual(1, messages.Count);

            //ulong
            messages.Clear();

            ulong ulongExpected = 10;
            ulong ulongActual = 10;

            LunarAssert.AreEqual(ulongExpected, ulongActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(ulongExpected, ulongActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            ulongExpected = 10;
            ulongActual = 11;

            LunarAssert.AreNotEqual(ulongExpected, ulongActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(ulongExpected, ulongActual);
            Assert.AreEqual(1, messages.Count);

            //float
            messages.Clear();

            float floatExpected = 10;
            float floatActual = 10;

            LunarAssert.AreEqual(floatExpected, floatActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(floatExpected, floatActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            floatExpected = 10;
            floatActual = 11;

            LunarAssert.AreNotEqual(floatExpected, floatActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(floatExpected, floatActual);
            Assert.AreEqual(1, messages.Count);

            //double
            messages.Clear();

            double doubleExpected = 10;
            double doubleActual = 10;

            LunarAssert.AreEqual(doubleExpected, doubleActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(doubleExpected, doubleActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            doubleExpected = 10;
            doubleActual = 11;

            LunarAssert.AreNotEqual(doubleExpected, doubleActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(doubleExpected, doubleActual);
            Assert.AreEqual(1, messages.Count);

            //decimal
            messages.Clear();

            decimal decimalExpected = 10;
            decimal decimalActual = 10;

            LunarAssert.AreEqual(decimalExpected, decimalActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(decimalExpected, decimalActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            decimalExpected = 10;
            decimalActual = 11;

            LunarAssert.AreNotEqual(decimalExpected, decimalActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(decimalExpected, decimalActual);
            Assert.AreEqual(1, messages.Count);

            //object

            messages.Clear();

            string stringExpected = "test";
            string stringActual = "test";

            LunarAssert.AreEqual(stringExpected, stringActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(null, null);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(stringExpected, stringActual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            stringExpected = "test";
            stringActual = "TEST";

            LunarAssert.AreNotEqual(stringExpected, stringActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(null, stringActual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotEqual(stringExpected, null);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreEqual(stringExpected, stringActual);
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertSame()
        {
            object expected = new object();
            object actual = expected;

            LunarAssert.AreSame(expected, actual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreSame(null, null);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotSame(expected, actual);
            Assert.AreEqual(1, messages.Count);

            messages.Clear();

            expected = new object();
            actual = new object();

            LunarAssert.AreNotSame(expected, actual);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.AreNotSame(null, null);
            Assert.AreEqual(1, messages.Count);

            LunarAssert.AreSame(expected, actual);
            Assert.AreEqual(2, messages.Count);
        }

        [Test]
        public void TestAssertContains()
        {
            List<string> list = new List<string>();
            list.Add("test");

            LunarAssert.Contains("test", list);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.Contains("TEST", list);
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertNotContains()
        {
            List<string> list = new List<string>();
            list.Add("test");

            LunarAssert.NotContains("TEST", list);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.NotContains("test", list);
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertFail()
        {
            LunarAssert.Fail();
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertGreater()
        {
            LunarAssert.Greater(1, 0);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.Greater(1, 1);
            Assert.AreEqual(1, messages.Count);

            LunarAssert.Greater(1, 2);
            Assert.AreEqual(2, messages.Count);
        }

        [Test]
        public void TestAssertGreaterOrEqual()
        {
            LunarAssert.GreaterOrEqual(1, 0);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.GreaterOrEqual(1, 1);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.GreaterOrEqual(1, 2);
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertLess()
        {
            LunarAssert.Less(0, 1);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.Less(1, 1);
            Assert.AreEqual(1, messages.Count);

            LunarAssert.Less(2, 1);
            Assert.AreEqual(2, messages.Count);
        }

        [Test]
        public void TestAssertLessOrEqual()
        {
            LunarAssert.LessOrEqual(0, 1);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.LessOrEqual(1, 1);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.LessOrEqual(2, 1);
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertInstanceOf()
        {
            Base a = new DerivedA();

            LunarAssert.IsInstanceOfType<Base>(a);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.IsInstanceOfType<DerivedB>(a);
            Assert.AreEqual(1, messages.Count);

            LunarAssert.IsInstanceOfType<DerivedB>(null);
            Assert.AreEqual(2, messages.Count);
        }

        [Test]
        public void TestAssertIsNotInstanceOfType()
        {
            Base a = new DerivedA();

            LunarAssert.IsNotInstanceOfType<DerivedB>(a);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.IsNotInstanceOfType<DerivedB>(null);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.IsNotInstanceOfType<Base>(a);
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertIsEmpty()
        {
            LunarAssert.IsEmpty("");
            Assert.AreEqual(0, messages.Count);

            LunarAssert.IsEmpty(null);
            Assert.AreEqual(0, messages.Count);

            LunarAssert.IsEmpty("test");
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void TestAssertIsNotEmpty()
        {
            LunarAssert.IsNotEmpty("test");
            Assert.AreEqual(0, messages.Count);

            LunarAssert.IsNotEmpty("");
            Assert.AreEqual(1, messages.Count);

            LunarAssert.IsNotEmpty(null);
            Assert.AreEqual(2, messages.Count);
        }
    }

    class Base {}
    class DerivedA : Base {}
    class DerivedB : Base {}
}

