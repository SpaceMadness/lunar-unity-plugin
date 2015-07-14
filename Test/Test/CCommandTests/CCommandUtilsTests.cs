using System;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using LunarPlugin;
using LunarPluginInternal;
using LunarEditor;

namespace CCommandTests
{
    [TestFixture]
    public class CCommandUtilsTests : TestFixtureBase
    {
        [Test]
        public void TestCanInvokeMethodWithArgsCount()
        {
            Type dummyType = typeof(Dummy);
            
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute"), 0));
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute", typeof(String)), 1));
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute", typeof(String), typeof(String)), 2));
            
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute", typeof(String[])), 0));
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute", typeof(String[])), 1));
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute", typeof(String[])), 2));
            
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute", typeof(int), typeof(String[])), 1));
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute", typeof(int), typeof(String[])), 2));
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute", typeof(int), typeof(String[])), 3));
            
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute", typeof(bool), typeof(String[])), 1));
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute", typeof(bool), typeof(String[])), 2));
            assertTrue(CCommandUtils.CanInvokeMethodWithArgsCount(dummyType.GetMethod("canExecute", typeof(bool), typeof(String[])), 3));
        }

        class Dummy
        {
            bool canExecute()
            {
                return false;
            }
            
            bool canExecute(String arg)
            {
                return false;
            }
            
            bool canExecute(String arg1, String arg2)
            {
                return false;
            }
            
            bool canExecute(String[] args)
            {
                return false;
            }
            
            bool canExecute(int arg1, String[] args)
            {
                return false;
            }
            
            bool canExecute(bool arg1, params String[] args)
            {
                return false;
            }
        }
    }
}

