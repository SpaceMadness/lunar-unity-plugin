using System;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using LunarPlugin;
using LunarPluginInternal;
using LunarEditor;
using LunarPlugin.Test;
using System.Reflection;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class CCommandUtilsTests : TestFixtureBase
    {
        [Test]
        public void TestCanInvokeMethodWithArgsCount()
        {
            Type dummyType = typeof(Dummy);
            
            AssertCanExecute(0);
            AssertCanExecute(1, typeof(String));
            AssertCanExecute(2, typeof(String), typeof(String));
            
            AssertCanExecute(0, typeof(String[]));
            AssertCanExecute(1, typeof(String[]));
            AssertCanExecute(2, typeof(String[]));
            
            AssertCanExecute(1, typeof(int), typeof(String[]));
            AssertCanExecute(2, typeof(int), typeof(String[]));
            AssertCanExecute(3, typeof(int), typeof(String[]));
            
            AssertCanExecute(1, typeof(bool), typeof(String[]));
            AssertCanExecute(2, typeof(bool), typeof(String[]));
            AssertCanExecute(3, typeof(bool), typeof(String[]));
        }

        private void AssertCanExecute(int argsCount, params Type[] types)
        {
            Type type = typeof(Dummy);
            MethodInfo method = type.GetMethod("CanExecute", types);
            Assert.IsTrue(CCommandUtils.CanInvokeMethodWithArgsCount(method, argsCount));
        }

        class Dummy
        {
            public bool CanExecute()
            {
                return false;
            }
            
            public bool CanExecute(String arg)
            {
                return false;
            }
            
            public bool CanExecute(String arg1, String arg2)
            {
                return false;
            }
            
            public bool CanExecute(String[] args)
            {
                return false;
            }
            
            public bool CanExecute(int arg1, String[] args)
            {
                return false;
            }
            
            public bool CanExecute(bool arg1, params String[] args)
            {
                return false;
            }
        }
    }
}

