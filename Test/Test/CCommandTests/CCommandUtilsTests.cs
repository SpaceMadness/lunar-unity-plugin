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
        public void TestCanInvokeNoArgMethod()
        {
            AssertCanExecute<NoArg>(0);
            AssertCanNotExecute<NoArg>(1);
        }

        [Test]
        public void TestCanInvokeSingleArgMethod()
        {
            AssertCanExecute<SingleArg>(1);

            AssertCanNotExecute<SingleArg>(0);
            AssertCanNotExecute<SingleArg>(2);
        }

        [Test]
        public void TestCanInvokeMultipleArgMethod()
        {
            AssertCanExecute<MultipleArgs>(2);

            AssertCanNotExecute<MultipleArgs>(0);
            AssertCanNotExecute<MultipleArgs>(1);
            AssertCanNotExecute<MultipleArgs>(3);
        }

        [Test]
        public void TestCanInvokeArrayArgMethod()
        {
            AssertCanExecute<ArrayArg>(0);
            AssertCanExecute<ArrayArg>(1);
            AssertCanExecute<ArrayArg>(2);
            AssertCanExecute<ArrayArg>(3);
        }

        [Test]
        public void TestCanInvokeVarArgMethod()
        {
            AssertCanExecute<VarArgs>(0);
            AssertCanExecute<VarArgs>(1);
            AssertCanExecute<VarArgs>(2);
            AssertCanExecute<VarArgs>(3);
        }

        [Test]
        public void TestCanInvokeSingleArgAndArrayMethod()
        {
            AssertCanExecute<SingleArgAndArray>(1);
            AssertCanExecute<SingleArgAndArray>(2);
            AssertCanExecute<SingleArgAndArray>(3);

            AssertCanNotExecute<SingleArgAndArray>(0);
        }

        [Test]
        public void TestCanInvokeOptionalArgsMethod()
        {
            AssertCanExecute<OptionalArg>(0);
            AssertCanExecute<OptionalArg>(1);

            AssertCanNotExecute<OptionalArg>(2);
            AssertCanNotExecute<OptionalArg>(3);
        }

        [Test]
        public void TestCanInvokeArgAndOptionalArgMethod()
        {
            AssertCanExecute<ArgAndOptionalArg>(1);
            AssertCanExecute<ArgAndOptionalArg>(2);

            AssertCanNotExecute<ArgAndOptionalArg>(0);
            AssertCanNotExecute<ArgAndOptionalArg>(3);
        }

        [Test]
        public void TestCanInvokeArgAndOptionalArgsMethod()
        {
            AssertCanExecute<ArgAndOptionalArgs>(1);
            AssertCanExecute<ArgAndOptionalArgs>(2);
            AssertCanExecute<ArgAndOptionalArgs>(3);

            AssertCanNotExecute<ArgAndOptionalArgs>(0);
            AssertCanNotExecute<ArgAndOptionalArgs>(4);
        }

        [Test]
        public void TestCanInvokeArgsAndOptionalArgMethod()
        {   
            AssertCanExecute<ArgsAndOptionalArg>(2);
            AssertCanExecute<ArgsAndOptionalArg>(3);

            AssertCanNotExecute<ArgsAndOptionalArg>(0);
            AssertCanNotExecute<ArgsAndOptionalArg>(1);
            AssertCanNotExecute<ArgsAndOptionalArg>(4);
        }

        [Test]
        public void TestCanInvokeVector2ArgMethod()
        {   
            AssertCanExecute<Vector2Arg>(2);

            AssertCanNotExecute<Vector2Arg>(0);
            AssertCanNotExecute<Vector2Arg>(1);
            AssertCanNotExecute<Vector2Arg>(3);
        }

        [Test]
        public void TestCanInvokeArgAndVector2ArgMethod()
        {   
            AssertCanExecute<ArgAndVector2Arg>(3);

            AssertCanNotExecute<ArgAndVector2Arg>(0);
            AssertCanNotExecute<ArgAndVector2Arg>(1);
            AssertCanNotExecute<ArgAndVector2Arg>(2);
            AssertCanNotExecute<ArgAndVector2Arg>(4);
        }

        [Test]
        public void TestCanInvokeVector2ArgAndArgMethod()
        {   
            AssertCanExecute<Vector2ArgAndArg>(3);

            AssertCanNotExecute<Vector2ArgAndArg>(0);
            AssertCanNotExecute<Vector2ArgAndArg>(1);
            AssertCanNotExecute<Vector2ArgAndArg>(2);
            AssertCanNotExecute<Vector2ArgAndArg>(4);
        }

        #region Helpers

        private void AssertCanExecute<T>(int argsCount) where T : class
        {
            Assert.IsTrue(CanExecute<T>(argsCount));
        }

        private void AssertCanNotExecute<T>(int argsCount) where T : class
        {
            Assert.IsFalse(CanExecute<T>(argsCount));
        }

        private bool CanExecute<T>(int argsCount) where T : class
        {
            Type type = typeof(T);

            List<MethodInfo> methods = ClassUtils.ListInstanceMethods(type, delegate(MethodInfo method)
            {
                return method.Name.Equals("Execute");
            });

            Assert.AreEqual(1, methods.Count);

            return CCommandUtils.CanInvokeMethodWithArgsCount(methods[0], argsCount);
        }

        #endregion

        class NoArg
        {
            public void Execute()
            {
            }
        }

        class SingleArg
        {
            public void Execute(string arg)
            {
            }
        }

        class MultipleArgs
        {
            public void Execute(string arg1, string arg2)
            {
            }
        }

        class ArrayArg
        {
            public void Execute(string[] args)
            {
            }
        }

        class VarArgs
        {
            public void Execute(params string[] args)
            {
            }
        }

        class SingleArgAndArray
        {
            public void Execute(string arg, params string[] args)
            {
            }
        }

        class OptionalArg
        {
            public void Execute(string arg = null)
            {
            }
        }

        class ArgAndOptionalArg
        {
            public void Execute(string arg1, string arg2 = null)
            {
            }
        }

        class ArgAndOptionalArgs
        {
            public void Execute(string arg1, string arg2 = null, string arg3 = null)
            {
            }
        }

        class ArgsAndOptionalArg
        {
            public void Execute(string arg1, string arg2, string arg3 = null)
            {
            }
        }

        class Vector2Arg
        {
            public void Execute(Vector2 arg)
            {
            }
        }

        class ArgAndVector2Arg
        {
            public void Execute(string arg1, Vector2 arg2)
            {
            }
        }

        class Vector2ArgAndArg
        {
            public void Execute(Vector2 arg1, string arg2)
            {
            }
        }
    }
}

