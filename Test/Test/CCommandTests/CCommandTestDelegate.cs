using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using LunarPlugin;
using LunarPluginInternal;

using UnityEngine;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;
    using Option = CCommand.Option;

    [TestFixture]
    public class CCommandTestDelegate : CCommandTest
    {
        #region Registry

        [Test]
        public void TestDelegateAction()
        {
            Lunar.RegisterCommand("action", () =>
            {
                AddResult("action");
            });

            Execute("action");
            AssertResult("action");
        }

        [Test]
        public void TestOverrideDelegateAction()
        {
            Lunar.RegisterCommand("action", () =>
            {
                AddResult("action");
            });

            Lunar.RegisterCommand("action", () =>
            {
                AddResult("new action");
            });

            Execute("action");
            AssertResult("new action");
        }

        [Test]
        public void TestUnregisterDelegateAction()
        {
            Lunar.RegisterCommand("action", () =>
            {
                AddResult("action");
            });

            CRegistery.Unregister("action");

            Execute("action");
            AssertResult("action: command not found"); // FIXME);
        }

        [Test]
        public void TestDelegateActionCommand()
        {
            Lunar.RegisterCommand("action", (string[] args) =>
            {
                AddResult("action: {0}", StringUtils.Join(args, ", "));
            });

            Execute("action arg1 arg2 arg3");
            AssertResult("action: arg1, arg2, arg3");
        }

        [Test]
        public void TestOverrideDelegateActionCommand()
        {
            Lunar.RegisterCommand("action", (string[] args) =>
            {
                AddResult("action");
            });

            Lunar.RegisterCommand("action", (string[] args) =>
            {
                AddResult("new action");
            });

            Execute("action");
            AssertResult("new action");
        }

        [Test]
        public void TestUnregisterDelegateActionCommand()
        {
            Lunar.RegisterCommand("action", (string[] args) =>
            {
                AddResult("action");
            });

            CRegistery.Unregister("action");

            Execute("action");
            AssertResult("action: command not found"); // FIXME);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Test Action Parameters

        [Test]
        public void TestBoolArg()
        {
            Lunar.RegisterCommand("action", (bool arg) =>
            {
                AddResult("action " + arg);
            });

            Execute("action 1");
            AssertResult("action True");
        }

        [Test]
        public void TestBoolArg2()
        {
            Lunar.RegisterCommand("action", (bool arg1, bool arg2) =>
            {
                AddResult("action " + arg1 + " " + arg2);
            });

            Execute("action 1 0");
            AssertResult("action True False");
        }

        [Test]
        public void TestBoolArg3()
        {
            Lunar.RegisterCommand("action", (bool arg1, bool arg2, bool arg3) =>
            {
                AddResult("action " + arg1 + " " + arg2 + " " + arg3);
            });

            Execute("action 1 0 1");
            AssertResult("action True False True");
        }

        [Test]
        public void TestIntArg()
        {
            Lunar.RegisterCommand("action", (int arg) =>
            {
                AddResult("action " + arg);
            });

            Execute("action 10");
            AssertResult("action 10");
        }

        [Test]
        public void TestIntArg2()
        {
            Lunar.RegisterCommand("action", (int arg1, int arg2) =>
            {
                AddResult("action " + arg1 + " " + arg2);
            });

            Execute("action 10 20");
            AssertResult("action 10 20");
        }

        [Test]
        public void TestIntArg3()
        {
            Lunar.RegisterCommand("action", (int arg1, int arg2, int arg3) =>
            {
                AddResult("action " + arg1 + " " + arg2 + " " + arg3);
            });

            Execute("action 10 20 30");
            AssertResult("action 10 20 30");
        }

        [Test]
        public void TestFloatArg()
        {
            Lunar.RegisterCommand("action", (float arg) =>
            {
                AddResult("action " + arg);
            });

            Execute("action 3.14");
            AssertResult("action 3.14");
        }

        [Test]
        public void TestFloatArg2()
        {
            Lunar.RegisterCommand("action", (float arg1, float arg2) =>
            {
                AddResult("action " + arg1 + " " + arg2);
            });

            Execute("action 3.14 3.15");
            AssertResult("action 3.14 3.15");
        }

        [Test]
        public void TestFloatArg3()
        {
            Lunar.RegisterCommand("action", (float arg1, float arg2, float arg3) =>
            {
                AddResult("action " + arg1 + " " + arg2 + " " + arg3);
            });

            Execute("action 3.14 3.15 3.16");
            AssertResult("action 3.14 3.15 3.16");
        }

        [Test]
        public void TestStringArg()
        {
            Lunar.RegisterCommand("action", (string arg) =>
            {
                AddResult("action " + arg);
            });

            Execute("action arg1");
            AssertResult("action arg1");
        }

        [Test]
        public void TestStringArg2()
        {
            Lunar.RegisterCommand("action", (string arg1, string arg2) =>
            {
                AddResult("action " + arg1 + " " + arg2);
            });

            Execute("action arg1 arg2");
            AssertResult("action arg1 arg2");
        }

        [Test]
        public void TestStringArg3()
        {
            Lunar.RegisterCommand("action", (string arg1, string arg2, string arg3) =>
            {
                AddResult("action " + arg1 + " " + arg2 + " " + arg3);
            });

            Execute("action arg1 arg2 arg3");
            AssertResult("action arg1 arg2 arg3");
        }

        [Test]
        public void TestVector2()
        {
            Lunar.RegisterCommand("action", (Vector2 arg) =>
            {
                AddResult("action " + arg);
            });

            Execute("action 1.0 2.0");
            AssertResult("action (1.0, 2.0)");
        }

        [Test]
        public void TestVector3()
        {
            Lunar.RegisterCommand("action", (Vector3 arg) =>
            {
                AddResult("action " + arg);
            });

            Execute("action 1.0 2.0 3.0");
            AssertResult("action (1.0, 2.0, 3.0)");
        }

        [Test]
        public void TestVector4()
        {
            Lunar.RegisterCommand("action", (Vector4 arg) =>
            {
                AddResult("action " + arg);
            });

            Execute("action 1.0 2.0 3.0 4.0");
            AssertResult("action (1.0, 2.0, 3.0, 4.0)");
        }

        [Test]
        public void TestStrings()
        {
            Lunar.RegisterCommand("action", (string[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action");
            AssertResult("action ");
        }

        [Test]
        public void TestStrings1()
        {
            Lunar.RegisterCommand("action", (string[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action arg");
            AssertResult("action arg");
        }

        [Test]
        public void TestStrings2()
        {
            Lunar.RegisterCommand("action", (string[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action arg1 arg2");
            AssertResult("action arg1 arg2");
        }

        [Test]
        public void TestStrings3()
        {
            Lunar.RegisterCommand("action", (string[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action arg1 arg2 arg3");
            AssertResult("action arg1 arg2 arg3");
        }

        [Test]
        public void TestInts()
        {
            Lunar.RegisterCommand("action", (int[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action");
            AssertResult("action ");
        }

        [Test]
        public void TestInts1()
        {
            Lunar.RegisterCommand("action", (int[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action 10");
            AssertResult("action 10");
        }

        [Test]
        public void TestInts2()
        {
            Lunar.RegisterCommand("action", (int[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action 10 20");
            AssertResult("action 10 20");
        }

        [Test]
        public void TestInts3()
        {
            Lunar.RegisterCommand("action", (int[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action 10 20 30");
            AssertResult("action 10 20 30");
        }

        [Test]
        public void TestFloat()
        {
            Lunar.RegisterCommand("action", (float[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action");
            AssertResult("action ");
        }

        [Test]
        public void TestFloat1()
        {
            Lunar.RegisterCommand("action", (float[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action 3.14");
            AssertResult("action 3.14");
        }

        [Test]
        public void TestFloat2()
        {
            Lunar.RegisterCommand("action", (float[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action 3.14 3.15");
            AssertResult("action 3.14 3.15");
        }

        [Test]
        public void TestFloat3()
        {
            Lunar.RegisterCommand("action", (float[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action 3.14 3.15 3.16");
            AssertResult("action 3.14 3.15 3.16");
        }

        [Test]
        public void TestBool()
        {
            Lunar.RegisterCommand("action", (bool[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action");
            AssertResult("action ");
        }

        [Test]
        public void TestBool1()
        {
            Lunar.RegisterCommand("action", (bool[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action 1");
            AssertResult("action True");
        }

        [Test]
        public void TestBool2()
        {
            Lunar.RegisterCommand("action", (bool[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action 1 0");
            AssertResult("action True False");
        }

        [Test]
        public void TestBool3()
        {
            Lunar.RegisterCommand("action", (bool[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
            });

            Execute("action 1 0 1");
            AssertResult("action True False True");
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Test Function parameters

        [Test]
        public void TestBoolArgEx()
        {
            Lunar.RegisterCommandEx("action", (bool arg) =>
            {
                AddResult("action " + arg);
                return true;
            });

            Execute("action 1");
            AssertResult("action True");
        }

        [Test]
        public void TestBoolArg2Ex()
        {
            Lunar.RegisterCommandEx("action", (bool arg1, bool arg2) =>
            {
                AddResult("action " + arg1 + " " + arg2);
                return true;
            });

            Execute("action 1 0");
            AssertResult("action True False");
        }

        [Test]
        public void TestBoolArg3Ex()
        {
            Lunar.RegisterCommandEx("action", (bool arg1, bool arg2, bool arg3) =>
            {
                AddResult("action " + arg1 + " " + arg2 + " " + arg3);
                return true;
            });

            Execute("action 1 0 1");
            AssertResult("action True False True");
        }

        [Test]
        public void TestIntArgEx()
        {
            Lunar.RegisterCommandEx("action", (int arg) =>
            {
                AddResult("action " + arg);
                return true;
            });

            Execute("action 10");
            AssertResult("action 10");
        }

        [Test]
        public void TestIntArg2Ex()
        {
            Lunar.RegisterCommandEx("action", (int arg1, int arg2) =>
            {
                AddResult("action " + arg1 + " " + arg2);
                return true;
            });

            Execute("action 10 20");
            AssertResult("action 10 20");
        }

        [Test]
        public void TestIntArg3Ex()
        {
            Lunar.RegisterCommandEx("action", (int arg1, int arg2, int arg3) =>
            {
                AddResult("action " + arg1 + " " + arg2 + " " + arg3);
                return true;
            });

            Execute("action 10 20 30");
            AssertResult("action 10 20 30");
        }

        [Test]
        public void TestFloatArgEx()
        {
            Lunar.RegisterCommandEx("action", (float arg) =>
            {
                AddResult("action " + arg);
                return true;
            });

            Execute("action 3.14");
            AssertResult("action 3.14");
        }

        [Test]
        public void TestFloatArg2Ex()
        {
            Lunar.RegisterCommandEx("action", (float arg1, float arg2) =>
            {
                AddResult("action " + arg1 + " " + arg2);
                return true;
            });

            Execute("action 3.14 3.15");
            AssertResult("action 3.14 3.15");
        }

        [Test]
        public void TestFloatArg3Ex()
        {
            Lunar.RegisterCommandEx("action", (float arg1, float arg2, float arg3) =>
            {
                AddResult("action " + arg1 + " " + arg2 + " " + arg3);
                return true;
            });

            Execute("action 3.14 3.15 3.16");
            AssertResult("action 3.14 3.15 3.16");
        }

        [Test]
        public void TestStringArgEx()
        {
            Lunar.RegisterCommandEx("action", (string arg) =>
            {
                AddResult("action " + arg);
                return true;
            });

            Execute("action arg1");
            AssertResult("action arg1");
        }

        [Test]
        public void TestStringArg2Ex()
        {
            Lunar.RegisterCommandEx("action", (string arg1, string arg2) =>
            {
                AddResult("action " + arg1 + " " + arg2);
                return true;
            });

            Execute("action arg1 arg2");
            AssertResult("action arg1 arg2");
        }

        [Test]
        public void TestStringArg3Ex()
        {
            Lunar.RegisterCommandEx("action", (string arg1, string arg2, string arg3) =>
            {
                AddResult("action " + arg1 + " " + arg2 + " " + arg3);
                return true;
            });

            Execute("action arg1 arg2 arg3");
            AssertResult("action arg1 arg2 arg3");
        }

        [Test]
        public void TestVector2Ex()
        {
            Lunar.RegisterCommandEx("action", (Vector2 arg) =>
            {
                AddResult("action " + arg);
                return true;
            });

            Execute("action 1.0 2.0");
            AssertResult("action (1.0, 2.0)");
        }

        [Test]
        public void TestVector3Ex()
        {
            Lunar.RegisterCommandEx("action", (Vector3 arg) =>
            {
                AddResult("action " + arg);
                return true;
            });

            Execute("action 1.0 2.0 3.0");
            AssertResult("action (1.0, 2.0, 3.0)");
        }

        [Test]
        public void TestVector4Ex()
        {
            Lunar.RegisterCommandEx("action", (Vector4 arg) =>
            {
                AddResult("action " + arg);
                return true;
            });

            Execute("action 1.0 2.0 3.0 4.0");
            AssertResult("action (1.0, 2.0, 3.0, 4.0)");
        }

        [Test]
        public void TestStringsEx()
        {
            Lunar.RegisterCommandEx("action", (string[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action");
            AssertResult("action ");
        }

        [Test]
        public void TestStrings1Ex()
        {
            Lunar.RegisterCommandEx("action", (string[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action arg");
            AssertResult("action arg");
        }

        [Test]
        public void TestStrings2Ex()
        {
            Lunar.RegisterCommandEx("action", (string[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action arg1 arg2");
            AssertResult("action arg1 arg2");
        }

        [Test]
        public void TestStrings3Ex()
        {
            Lunar.RegisterCommandEx("action", (string[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action arg1 arg2 arg3");
            AssertResult("action arg1 arg2 arg3");
        }

        [Test]
        public void TestIntsEx()
        {
            Lunar.RegisterCommandEx("action", (int[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action");
            AssertResult("action ");
        }

        [Test]
        public void TestInts1Ex()
        {
            Lunar.RegisterCommandEx("action", (int[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action 10");
            AssertResult("action 10");
        }

        [Test]
        public void TestInts2Ex()
        {
            Lunar.RegisterCommandEx("action", (int[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action 10 20");
            AssertResult("action 10 20");
        }

        [Test]
        public void TestInts3Ex()
        {
            Lunar.RegisterCommandEx("action", (int[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action 10 20 30");
            AssertResult("action 10 20 30");
        }

        [Test]
        public void TestFloatEx()
        {
            Lunar.RegisterCommandEx("action", (float[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action");
            AssertResult("action ");
        }

        [Test]
        public void TestFloat1Ex()
        {
            Lunar.RegisterCommandEx("action", (float[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action 3.14");
            AssertResult("action 3.14");
        }

        [Test]
        public void TestFloat2Ex()
        {
            Lunar.RegisterCommandEx("action", (float[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action 3.14 3.15");
            AssertResult("action 3.14 3.15");
        }

        [Test]
        public void TestFloat3Ex()
        {
            Lunar.RegisterCommandEx("action", (float[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action 3.14 3.15 3.16");
            AssertResult("action 3.14 3.15 3.16");
        }

        [Test]
        public void TestBoolEx()
        {
            Lunar.RegisterCommandEx("action", (bool[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action");
            AssertResult("action ");
        }

        [Test]
        public void TestBool1Ex()
        {
            Lunar.RegisterCommandEx("action", (bool[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action 1");
            AssertResult("action True");
        }

        [Test]
        public void TestBool2Ex()
        {
            Lunar.RegisterCommandEx("action", (bool[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action 1 0");
            AssertResult("action True False");
        }

        [Test]
        public void TestBool3Ex()
        {
            Lunar.RegisterCommandEx("action", (bool[] args) =>
            {
                AddResult("action " + StringUtils.Join(args, " "));
                return true;
            });

            Execute("action 1 0 1");
            AssertResult("action True False True");
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Test command chain

        [Test]
        public void TestActionChain()
        {
            Lunar.RegisterCommand("action1", () =>
            {
                AddResult("action1");
            });

            Lunar.RegisterCommand("action2", () =>
            {
                AddResult("action2");
            });

            Execute("action1 && action2");
            AssertResult("action1", "action2");
        }

        [Test]
        public void TestFunctionChain()
        {
            Lunar.RegisterCommandEx("action1", () =>
            {
                AddResult("action1");
                return true;
            });

            Lunar.RegisterCommandEx("action2", () =>
            {
                AddResult("action2");
                return true;
            });

            Execute("action1 && action2");
            AssertResult("action1", "action2");
        }

        [Test]
        public void TestFunctionBrokenChain()
        {
            Lunar.RegisterCommandEx("action1", () =>
            {
                AddResult("action1");
                return true;
            });

            Lunar.RegisterCommandEx("action2", () =>
            {
                AddResult("action2");
                return false;
            });

            Lunar.RegisterCommandEx("action3", () =>
            {
                AddResult("action3");
                return false;
            });

            Execute("action1 && action2 && action3");
            AssertResult("action1", "action2");
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Test wrong args

        [Test]
        public void TestWrongNumberOfArgs()
        {
            Lunar.RegisterCommand("action", () =>
            {
                Assert.Fail("Command should not get executed");
            });

            Execute("action arg");
            AssertResult("  Wrong number of arguments", "  usage: action");
        }

        [Test]
        public void TestWrongNumberOfArgs2()
        {
            Lunar.RegisterCommand("action", (string arg) =>
            {
                Assert.Fail("Command should not get executed");
            });

            Execute("action arg1 arg2");
            AssertResult("  Wrong number of arguments", "  usage: action <arg>");
        }

        [Test]
        public void TestWrongNumberOfArgs3()
        {
            Lunar.RegisterCommand("action", (string arg1, string arg2) =>
            {
                Assert.Fail("Command should not get executed");
            });

            Execute("action arg1 arg2 arg3");
            AssertResult("  Wrong number of arguments", "  usage: action <arg1> <arg2>");
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        [SetUp]
        public void SetUp()
        {
            RunSetUp();

            this.IsTrackTerminalLog = true;
            this.IsTrackConsoleLog = true;

            RegisterCommands(
                new cmdlist()
            );
        }

        [TearDown]
        public void TearDown()
        {
            RunTearDown();
        }

        #endregion
    }
}

