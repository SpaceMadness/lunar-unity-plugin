//
//  CAssert.cs
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
using System.Collections.Generic;

using LunarPluginInternal;

public delegate void CAssertCallback(string message, string stackTrace);

namespace LunarPlugin
{
    public static class CAssert
    {
        public static CAssertCallback callback;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsTrue(bool condition)
        {
            if (IsEnabled && !condition)
                AssertHelper("Assertion failed: 'true' expected");
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsTrue(bool condition, string message)
        {
            if (IsEnabled && !condition)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsTrue(bool condition, string format, params object[] args)
        {
            if (IsEnabled && !condition)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsTrue<A0>(bool condition, string format, A0 arg0)
        {
            if (IsEnabled && !condition)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsTrue<A0, A1>(bool condition, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && !condition)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsTrue<A0, A1, A2>(bool condition, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && !condition)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsFalse(bool condition)
        {
            if (IsEnabled && condition)
                AssertHelper("Assertion failed: 'false' expected");
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsFalse(bool condition, string message)
        {
            if (IsEnabled && condition)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsFalse(bool condition, string format, params object[] args)
        {
            if (IsEnabled && condition)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsFalse<A0>(bool condition, string format, A0 arg0)
        {
            if (IsEnabled && condition)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsFalse<A0, A1>(bool condition, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && condition)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsFalse<A0, A1, A2>(bool condition, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && condition)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNull(object obj)
        {
            if (IsEnabled && obj != null)
                AssertHelper("Assertion failed: expected 'null' but was '{0}'", obj);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNull(object obj, string message)
        {
            if (IsEnabled && obj != null)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNull(object obj, string format, params object[] args)
        {
            if (IsEnabled && obj != null)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNull<A0>(object obj, string format, A0 arg0)
        {
            if (IsEnabled && obj != null)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNull<A0, A1>(object obj, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && obj != null)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNull<A0, A1, A2>(object obj, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && obj != null)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object obj)
        {
            if (IsEnabled && obj == null)
                AssertHelper("Assertion failed: object is 'null'");
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object obj, string message)
        {
            if (IsEnabled && obj == null)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object obj, string format, params object[] args)
        {
            if (IsEnabled && obj == null)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotNullElement<T>(IList<T> list)
            where T : class
        {
            if (IsEnabled)
            {
                int index = 0;
                foreach (T t in list)
                {
                    CAssert.IsNotNull(t, "Element at {0} is null", index.ToString());
                    ++index;
                }
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotNull<A0>(object obj, string format, A0 arg0)
        {
            if (IsEnabled && obj == null)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotNull<A0, A1>(object obj, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && obj == null)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotNull<A0, A1, A2>(object obj, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && obj == null)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(bool expected, bool actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(bool expected, bool actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(bool expected, bool actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(bool expected, bool actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(bool expected, bool actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(bool expected, bool actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(byte expected, byte actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(byte expected, byte actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(byte expected, byte actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(byte expected, byte actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(byte expected, byte actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(byte expected, byte actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(sbyte expected, sbyte actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(sbyte expected, sbyte actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(sbyte expected, sbyte actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(sbyte expected, sbyte actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(sbyte expected, sbyte actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(sbyte expected, sbyte actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(short expected, short actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(short expected, short actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(short expected, short actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(short expected, short actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(short expected, short actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(short expected, short actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(ushort expected, ushort actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(ushort expected, ushort actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(ushort expected, ushort actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(ushort expected, ushort actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(ushort expected, ushort actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(ushort expected, ushort actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(char expected, char actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(char expected, char actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(char expected, char actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(char expected, char actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(char expected, char actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(char expected, char actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(int expected, int actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(int expected, int actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(int expected, int actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(int expected, int actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(int expected, int actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(int expected, int actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(uint expected, uint actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(uint expected, uint actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(uint expected, uint actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(uint expected, uint actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(uint expected, uint actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(uint expected, uint actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(long expected, long actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(long expected, long actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(long expected, long actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(long expected, long actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(long expected, long actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(long expected, long actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(ulong expected, ulong actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(ulong expected, ulong actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(ulong expected, ulong actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(ulong expected, ulong actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(ulong expected, ulong actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(ulong expected, ulong actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(float expected, float actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(float expected, float actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(float expected, float actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(float expected, float actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(float expected, float actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(float expected, float actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(double expected, double actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(double expected, double actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(double expected, double actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(double expected, double actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(double expected, double actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(double expected, double actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(decimal expected, decimal actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", expected.ToString(), actual.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(decimal expected, decimal actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(decimal expected, decimal actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(decimal expected, decimal actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(decimal expected, decimal actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(decimal expected, decimal actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(object expected, object actual)
        {
            if (IsEnabled && !(expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper("Assertion failed: expected '{0}' but was '{1}'", toString(expected), toString(actual));
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(object expected, object actual, string message)
        {
            if (IsEnabled && !(expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual(object expected, object actual, string format, params object[] args)
        {
            if (IsEnabled && !(expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0>(object expected, object actual, string format, A0 arg0)
        {
            if (IsEnabled && !(expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1>(object expected, object actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && !(expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<A0, A1, A2>(object expected, object actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && !(expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(bool expected, bool actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(bool expected, bool actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(bool expected, bool actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(bool expected, bool actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(bool expected, bool actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(bool expected, bool actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(byte expected, byte actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(byte expected, byte actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(byte expected, byte actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(byte expected, byte actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(byte expected, byte actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(byte expected, byte actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(sbyte expected, sbyte actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(sbyte expected, sbyte actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(sbyte expected, sbyte actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(sbyte expected, sbyte actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(sbyte expected, sbyte actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(sbyte expected, sbyte actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(short expected, short actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(short expected, short actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(short expected, short actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(short expected, short actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(short expected, short actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(short expected, short actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(ushort expected, ushort actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(ushort expected, ushort actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(ushort expected, ushort actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(ushort expected, ushort actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(ushort expected, ushort actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(ushort expected, ushort actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(char expected, char actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(char expected, char actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(char expected, char actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(char expected, char actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(char expected, char actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(char expected, char actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(int expected, int actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(int expected, int actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(int expected, int actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(int expected, int actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(int expected, int actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(int expected, int actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(uint expected, uint actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(uint expected, uint actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(uint expected, uint actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(uint expected, uint actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(uint expected, uint actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(uint expected, uint actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(long expected, long actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(long expected, long actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(long expected, long actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(long expected, long actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(long expected, long actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(long expected, long actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(ulong expected, ulong actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(ulong expected, ulong actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(ulong expected, ulong actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(ulong expected, ulong actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(ulong expected, ulong actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(ulong expected, ulong actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(float expected, float actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(float expected, float actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(float expected, float actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(float expected, float actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(float expected, float actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(float expected, float actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(double expected, double actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(double expected, double actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(double expected, double actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(double expected, double actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(double expected, double actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(double expected, double actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(decimal expected, decimal actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: values are equal '{0}'", expected.ToString());
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(decimal expected, decimal actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(decimal expected, decimal actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(decimal expected, decimal actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(decimal expected, decimal actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(decimal expected, decimal actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(object expected, object actual)
        {
            if (IsEnabled && (expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper("Assertion failed: objects are equal '{0}'", toString(expected));
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(object expected, object actual, string message)
        {
            if (IsEnabled && (expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual(object expected, object actual, string format, params object[] args)
        {
            if (IsEnabled && (expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0>(object expected, object actual, string format, A0 arg0)
        {
            if (IsEnabled && (expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1>(object expected, object actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && (expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotEqual<A0, A1, A2>(object expected, object actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && (expected != null && actual != null && expected.Equals(actual) || expected == null && actual == null))
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreSame(object expected, object actual)
        {
            if (IsEnabled && expected != actual)
                AssertHelper("Assertion failed: object references are not the same '{0}' but was '{1}'", toString(expected), toString(actual));
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreSame(object expected, object actual, string message)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreSame(object expected, object actual, string format, params object[] args)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreSame<A0>(object expected, object actual, string format, A0 arg0)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreSame<A0, A1>(object expected, object actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreSame<A0, A1, A2>(object expected, object actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected != actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotSame(object expected, object actual)
        {
            if (IsEnabled && expected == actual)
                AssertHelper("Assertion failed: object references are the same '{0}'", toString(expected));
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotSame(object expected, object actual, string message)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotSame(object expected, object actual, string format, params object[] args)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotSame<A0>(object expected, object actual, string format, A0 arg0)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotSame<A0, A1>(object expected, object actual, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreNotSame<A0, A1, A2>(object expected, object actual, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && expected == actual)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Contains<T>(T expected, ICollection<T> collection)
        {
            if (IsEnabled && (collection == null || !collection.Contains(expected)))
            {
                if (collection == null)
                    AssertHelper("Assertion failed: collection is null");
                else
                    AssertHelper("Assertion failed: collection doesn't contain the item {0}", expected);
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void NotContains<T>(T expected, ICollection<T> collection)
        {
            if (IsEnabled && (collection != null && collection.Contains(expected)))
            {
                if (collection == null)
                    AssertHelper("Assertion failed: collection is null");
                else
                    AssertHelper("Assertion failed: collection contains the item {0}", expected);
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Fail()
        {
            if (IsEnabled)
                AssertHelper("Assertion failed");
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Fail(string message)
        {
            if (IsEnabled)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Fail(string format, params object[] args)
        {
            if (IsEnabled)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Fail<A0>(string format, A0 arg0)
        {
            if (IsEnabled)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Fail<A0, A1>(string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Fail<A0, A1, A2>(string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Greater<T>(T a, T b) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) <= 0)
                AssertHelper("Assertion failed: '{0}' is not greater than '{1}'", a, b);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Greater<T>(T a, T b, string message) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) <= 0)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Greater<T>(T a, T b, string format, params object[] args) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) <= 0)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Greater<T, A0>(T a, T b, string format, A0 arg0) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) <= 0)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Greater<T, A0, A1>(T a, T b, string format, A0 arg0, A1 arg1) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) <= 0)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Greater<T, A0, A1, A2>(T a, T b, string format, A0 arg0, A1 arg1, A2 arg2) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) <= 0)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void GreaterOrEqual<T>(T a, T b) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) < 0)
                AssertHelper("Assertion failed: '{0}' is not greater or equal to '{1}'", a, b);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void GreaterOrEqual<T>(T a, T b, string message) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) < 0)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void GreaterOrEqual<T>(T a, T b, string format, params object[] args) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) < 0)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void GreaterOrEqual<T, A0>(T a, T b, string format, A0 arg0) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) < 0)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void GreaterOrEqual<T, A0, A1>(T a, T b, string format, A0 arg0, A1 arg1) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) < 0)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void GreaterOrEqual<T, A0, A1, A2>(T a, T b, string format, A0 arg0, A1 arg1, A2 arg2) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) < 0)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Less<T>(T a, T b) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) >= 0)
                AssertHelper("Assertion failed: '{0}' is not less than '{1}'", a, b);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Less<T>(T a, T b, string message) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) >= 0)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Less<T>(T a, T b, string format, params object[] args) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) >= 0)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Less<T, A0>(T a, T b, string format, A0 arg0) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) >= 0)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Less<T, A0, A1>(T a, T b, string format, A0 arg0, A1 arg1) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) >= 0)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Less<T, A0, A1, A2>(T a, T b, string format, A0 arg0, A1 arg1, A2 arg2) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) >= 0)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LessOrEqual<T>(T a, T b) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) > 0)
                AssertHelper("Assertion failed: '{0}' is not less or equal to '{1}'", a, b);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LessOrEqual<T>(T a, T b, string message) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) > 0)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LessOrEqual<T>(T a, T b, string format, params object[] args) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) > 0)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LessOrEqual<T, A0>(T a, T b, string format, A0 arg0) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) > 0)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LessOrEqual<T, A0, A1>(T a, T b, string format, A0 arg0, A1 arg1) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) > 0)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LessOrEqual<T, A0, A1, A2>(T a, T b, string format, A0 arg0, A1 arg1, A2 arg2) where T : IComparable<T>
        {
            if (IsEnabled && a.CompareTo(b) > 0)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType(Type type, object o)
        {
            if (IsEnabled && (type == null || !type.IsInstanceOfType(o)))
                AssertHelper("Assertion failed: expected type of '{0}' but was '{1}'", type, o != null ? o.GetType() : (Type)null);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType(Type type, object o, string message)
        {
            if (IsEnabled && (type == null || !type.IsInstanceOfType(o)))
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType(Type type, object o, string format, params object[] args)
        {
            if (IsEnabled && (type == null || !type.IsInstanceOfType(o)))
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType<A0>(Type type, object o, string format, A0 arg0)
        {
            if (IsEnabled && (type == null || !type.IsInstanceOfType(o)))
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType<A0, A1>(Type type, object o, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && (type == null || !type.IsInstanceOfType(o)))
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType<A0, A1, A2>(Type type, object o, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && (type == null || !type.IsInstanceOfType(o)))
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType<T>(object o)
        {
            if (IsEnabled && !(o is T))
                AssertHelper("Assertion failed: expected type of '{0}' but was '{1}'", typeof(T), o != null ? o.GetType() : (Type)null);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType<T>(object o, string message)
        {
            if (IsEnabled && !(o is T))
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType<T>(object o, string format, params object[] args)
        {
            if (IsEnabled && !(o is T))
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType<T, A0>(object o, string format, A0 arg0)
        {
            if (IsEnabled && !(o is T))
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType<T, A0, A1>(object o, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && !(o is T))
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsInstanceOfType<T, A0, A1, A2>(object o, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && !(o is T))
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType(Type type, object o)
        {
            if (IsEnabled && (type != null && type.IsInstanceOfType(o)))
                AssertHelper("Assertion failed: object '{0}' is subtype of '{1}'", type, o != null ? o.GetType() : (Type)null);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType(Type type, object o, string message)
        {
            if (IsEnabled && (type != null && type.IsInstanceOfType(o)))
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType(Type type, object o, string format, params object[] args)
        {
            if (IsEnabled && (type != null && type.IsInstanceOfType(o)))
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType<A0>(Type type, object o, string format, A0 arg0)
        {
            if (IsEnabled && (type != null && type.IsInstanceOfType(o)))
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType<A0, A1>(Type type, object o, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && (type != null && type.IsInstanceOfType(o)))
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType<A0, A1, A2>(Type type, object o, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && (type != null && type.IsInstanceOfType(o)))
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType<T>(object o)
        {
            if (IsEnabled && (o is T))
                AssertHelper("Assertion failed: object '{0}' is subtype of '{1}'", typeof(T), o != null ? o.GetType() : (Type)null);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType<T>(object o, string message)
        {
            if (IsEnabled && (o is T))
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType<T>(object o, string format, params object[] args)
        {
            if (IsEnabled && (o is T))
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType<T, A0>(object o, string format, A0 arg0)
        {
            if (IsEnabled && (o is T))
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType<T, A0, A1>(object o, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && (o is T))
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotInstanceOfType<T, A0, A1, A2>(object o, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && (o is T))
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty(string str)
        {
            if (IsEnabled && !string.IsNullOrEmpty(str))
                AssertHelper("Assertion failed: string is not empty '{0}'", str);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty(string str, string message)
        {
            if (IsEnabled && !string.IsNullOrEmpty(str))
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty(string str, string format, params object[] args)
        {
            if (IsEnabled && !string.IsNullOrEmpty(str))
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty<A0>(string str, string format, A0 arg0)
        {
            if (IsEnabled && !string.IsNullOrEmpty(str))
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty<A0, A1>(string str, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && !string.IsNullOrEmpty(str))
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty<A0, A1, A2>(string str, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && !string.IsNullOrEmpty(str))
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty(string str)
        {
            if (IsEnabled && string.IsNullOrEmpty(str))
                AssertHelper("Assertion failed: string is null or empty '{0}'", str);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty(string str, string message)
        {
            if (IsEnabled && string.IsNullOrEmpty(str))
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty(string str, string format, params object[] args)
        {
            if (IsEnabled && string.IsNullOrEmpty(str))
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty<A0>(string str, string format, A0 arg0)
        {
            if (IsEnabled && string.IsNullOrEmpty(str))
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty<A0, A1>(string str, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && string.IsNullOrEmpty(str))
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty<A0, A1, A2>(string str, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && string.IsNullOrEmpty(str))
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty<T>(ICollection<T> collection)
        {
            if (IsEnabled && collection != null && collection.Count == 0)
                AssertHelper("Assertion failed: collection is null or not empty '{0}'", collection);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty<T>(ICollection<T> collection, string message)
        {
            if (IsEnabled && collection != null && collection.Count == 0)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty<T>(ICollection<T> collection, string format, params object[] args)
        {
            if (IsEnabled && collection != null && collection.Count == 0)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty<T, A0>(ICollection<T> collection, string format, A0 arg0)
        {
            if (IsEnabled && collection != null && collection.Count == 0)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty<T, A0, A1>(ICollection<T> collection, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && collection != null && collection.Count == 0)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsEmpty<T, A0, A1, A2>(ICollection<T> collection, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && collection != null && collection.Count == 0)
                AssertHelper(format, arg0, arg1, arg2);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty<T>(ICollection<T> collection)
        {
            if (IsEnabled && collection != null && collection.Count != 0)
                AssertHelper("Assertion failed: collection is null or empty '{0}'", collection);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty<T>(ICollection<T> collection, string message)
        {
            if (IsEnabled && collection != null && collection.Count != 0)
                AssertHelper(message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty<T>(ICollection<T> collection, string format, params object[] args)
        {
            if (IsEnabled && collection != null && collection.Count != 0)
                AssertHelper(format, args);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty<T, A0>(ICollection<T> collection, string format, A0 arg0)
        {
            if (IsEnabled && collection != null && collection.Count != 0)
                AssertHelper(format, arg0);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty<T, A0, A1>(ICollection<T> collection, string format, A0 arg0, A1 arg1)
        {
            if (IsEnabled && collection != null && collection.Count != 0)
                AssertHelper(format, arg0, arg1);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotEmpty<T, A0, A1, A2>(ICollection<T> collection, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (IsEnabled && collection != null && collection.Count != 0)
                AssertHelper(format, arg0, arg1, arg2);
        }

        private static void AssertHelper(string format, params object[] args)
        {
            string message = CStringUtils.TryFormat(format, args);
            string stackTrace = CStackTrace.ExtractStackTrace(3);

            CPlatform.AssertMessage(message, stackTrace);

            try
            {
                if (callback != null)
                    callback(message, stackTrace);
            }
            catch (Exception)
            {
            }
        }

        private static String toString(object obj)
        {
            return obj != null ? obj.ToString() : "null";
        }

        private static bool IsEnabled
        {
            get
            {
                #if LUNAR_DEVELOPMENT
                return true;
                #else
                return false;
                #endif
            }
        }
    }
}
