using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

namespace LunarPlugin.Test
{
    using Assert = NUnit.Framework.Assert;

    public abstract class TestFixtureBase
    {
        private List<String> result;

        static TestFixtureBase()
        {
            ThreadUtils.SetMainThread();
        }

        protected virtual void RunSetUp()
        {
            result = new List<String>();
            OverrideDebugMode(true);
        }

        protected virtual void RunTearDown()
        {
            result = null;
        }

        protected void AssertList<T>(IList<T> actual, params T[] expected) 
            where T : IEquatable<T>
        {
            Assert.AreEqual(expected.Length, actual.Count, StringUtils.TryFormat("Expected: [{0}]\nActual: [{1}]"), Join(", ", expected), Join(", ", actual));
            for (int i = 0; i < expected.Length; ++i)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        protected void AssertArray<T>(T[] actual, params T[] expected)
            where T : IEquatable<T>
        {
            Assert.IsNotNull(actual);
            Assert.IsNotNull(expected);

            Assert.AreEqual(actual.Length, expected.Length, StringUtils.TryFormat("Expected: [{0}]\nActual: [{1}]"), Join(", ", expected), Join(", ", actual));
            for (int i = 0; i < expected.Length; ++i)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        protected void AssertTypes<T>(IList<T> actual, params Type[] expected)
        {
            Assert.AreEqual(actual.Count, expected.Length, StringUtils.TryFormat("Expected: [{0}]\nActual: [{1}]"), Join(", ", expected), JoinTypes(", ", actual));
            for (int i = 0; i < expected.Length; ++i)
            {
                Assert.AreEqual(actual[i].GetType(), expected[i]);
            }
        }

        protected string Join<T>(string separator, IList<T> list)
        {
            return StringUtils.Join(list, separator);
        }

        private string JoinTypes<T>(string separator, IList<T> list)
        {
            Type[] types = new Type[list.Count];
            for (int i = 0; i < list.Count; ++i)
            {
                types[i] = list[i].GetType();
            }

            return Join(separator, types);
        }

        protected void OverrideDebugMode(bool value)
        {
            ClassUtilsEx.SetField(typeof(Config), "isDebugBuild", value);
        }

        protected void AssertResult(params string[] expected)
        {
            AssertList(result, expected);
        }

        protected void AddResult(string str)
        {
            result.Add(str);
        }

        protected List<String> Result
        {
            get { return result; }
        }
    }
}

