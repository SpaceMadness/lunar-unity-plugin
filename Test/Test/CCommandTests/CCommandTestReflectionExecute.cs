using UnityEngine;

using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using LunarPlugin;
using LunarPluginInternal;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;
    using Option = CCommand.Option;

    [TestFixture]
    public class CCommandTestReflectionExecute : CCommandTestFixture
    {
        #region Types

        [Test]
        public void TestExecuteStringNoArgs()
        {
            Execute("string ");
            AssertResult("string");
        }

        [Test]
        public void TestExecuteStringSingleArg()
        {
            Execute("string '10 and up'");
            AssertResult("string 10 and up");
        }

        [Test]
        public void TestExecuteStringTwoArgs()
        {
            Execute("string 10 '20 and up'");
            AssertResult("string 10 20 and up");
        }

        [Test]
        public void TestExecuteStringMultipleArgs()
        {
            Execute("string 10 20 '30 and up'");
            AssertResult("string 10 20 30 and up");
        }

        [Test]
        public void TestExecuteStringMultipleArgsWrongCount()
        {
            Execute("int 10 20 and up", false);
            AssertResult(
                "  Wrong arguments",
                "  usage: int\n" +
                "         int <value>\n" +
                "         int <value1> <value2>\n" +
                "         int <value1> <value2> <value3>"
            );
        }

        [Test]
        public void TestExecuteIntNoArgs()
        {
            Execute("int ");
            AssertResult("int");
        }

        [Test]
        public void TestExecuteIntSingleArg()
        {
            Execute("int 10");
            AssertResult("int 10");
        }

        [Test]
        public void TestExecuteIntTwoArgs()
        {
            Execute("int 10 20");
            AssertResult("int 10 20");
        }

        [Test]
        public void TestExecuteIntMultipleArgs()
        {
            Execute("int 10 20 30");
            AssertResult("int 10 20 30");
        }

        [Test]
        public void TestExecuteIntMultipleArgsWrongCount()
        {
            Execute("int 10 20 30 40", false);
            AssertResult(
                "  Wrong arguments",
                "  usage: int\n" +
                "         int <value>\n" +
                "         int <value1> <value2>\n" +
                "         int <value1> <value2> <value3>"
            );
        }

        [Test]
        public void TestExecuteFloatNoArgs()
        {
            Execute("float");
            AssertResult("float");
        }

        [Test]
        public void TestExecuteFloatSingleArg()
        {
            Execute("float 1.0");
            AssertResult("float 1");
        }

        [Test]
        public void TestExecuteFloatTwoArgs()
        {
            Execute("float 1.0 2.0");
            AssertResult("float 1 2");
        }

        [Test]
        public void TestExecuteFloatMultipleArgs()
        {
            Execute("float 1.0 2.0 3.0");
            AssertResult("float 1 2 3");
        }

        [Test]
        public void TestExecuteFloatMultipleArgsWrongCount()
        {
            Execute("float 1.0 2.0 3.0 4.0", false);
            AssertResult(
                "  Wrong arguments",
                "  usage: float\n" +
                "         float <value>\n" +
                "         float <value1> <value2>\n" +
                "         float <value1> <value2> <value3>"
            );
        }

        [Test]
        public void TestExecuteBoolNoArgs()
        {
            Execute("bool");
            AssertResult("bool");
        }

        [Test]
        public void TestExecuteBoolSingleArg()
        {
            Execute("bool 1");
            AssertResult("bool True");
        }

        [Test]
        public void TestExecuteBoolTwoArgs()
        {
            Execute("bool 1 0");
            AssertResult("bool True False");
        }

        [Test]
        public void TestExecuteBoolMultipleArgs()
        {
            Execute("bool 1 0 1");
            AssertResult("bool True False True");
        }

        [Test]
        public void TestExecuteBoolMultipleArgsWrongCount()
        {
            Execute("bool 1 0 1 0", false);
            AssertResult(
                "  Wrong arguments",
                "  usage: bool\n" +
                "         bool <value>\n" +
                "         bool <value1> <value2>\n" +
                "         bool <value1> <value2> <value3>"
            );
        }

        [Test]
        public void TestExecuteVector2NoArgs()
        {
            Execute("vector2 ");
            AssertResult("vector2");
        }

        [Test]
        public void TestExecuteVector2SingleArg()
        {
            Execute("vector2  1 2");
            AssertResult("vector2 (1.0, 2.0)");
        }

        [Test]
        public void TestExecuteVector2TwoArgs()
        {
            Execute("vector2  1 2  5 6");
            AssertResult("vector2 (1.0, 2.0) (5.0, 6.0)");
        }

        [Test]
        public void TestExecuteVector2MultipleArgs()
        {
            Execute("vector2  1 2  5 6  9 10");
            AssertResult("vector2 (1.0, 2.0) (5.0, 6.0) (9.0, 10.0)");
        }

        [Test]
        public void TestExecuteVector2MultipleArgsWrongCount()
        {
            Execute("vector2  1 2  5 6  9 10  13 14", false);
            AssertResult(
                "  Wrong arguments",
                "  usage: vector2\n" +
                "         vector2 <value>\n" +
                "         vector2 <value1> <value2>\n" +
                "         vector2 <value1> <value2> <value3>"
            );
        }

        [Test]
        public void TestExecuteVector3NoArgs()
        {
            Execute("vector3 ");
            AssertResult("vector3");
        }

        [Test]
        public void TestExecuteVector3SingleArg()
        {
            Execute("vector3  1 2 3");
            AssertResult("vector3 (1.0, 2.0, 3.0)");
        }

        [Test]
        public void TestExecuteVector3TwoArgs()
        {
            Execute("vector3  1 2 3  5 6 7");
            AssertResult("vector3 (1.0, 2.0, 3.0) (5.0, 6.0, 7.0)");
        }

        [Test]
        public void TestExecuteVector3MultipleArgs()
        {
            Execute("vector3  1 2 3  5 6 7  9 10 11");
            AssertResult("vector3 (1.0, 2.0, 3.0) (5.0, 6.0, 7.0) (9.0, 10.0, 11.0)");
        }

        [Test]
        public void TestExecuteVector3MultipleArgsWrongCount()
        {
            Execute("vector3  1 2 3  5 6 7  9 10 11  13 14 15", false);
            AssertResult(
                "  Wrong arguments",
                "  usage: vector3\n" +
                "         vector3 <value>\n" +
                "         vector3 <value1> <value2>\n" +
                "         vector3 <value1> <value2> <value3>"
            );
        }

        [Test]
        public void TestExecuteVector4NoArgs()
        {
            Execute("vector4 ");
            AssertResult("vector4");
        }

        [Test]
        public void TestExecuteVector4SingleArg()
        {
            Execute("vector4  1 2 3 4");
            AssertResult("vector4 (1.0, 2.0, 3.0, 4.0)");
        }

        [Test]
        public void TestExecuteVector4TwoArgs()
        {
            Execute("vector4  1 2 3 4  5 6 7 8");
            AssertResult("vector4 (1.0, 2.0, 3.0, 4.0) (5.0, 6.0, 7.0, 8.0)");
        }

        [Test]
        public void TestExecuteVector4MultipleArgs()
        {
            Execute("vector4  1 2 3 4  5 6 7 8  9 10 11 12");
            AssertResult("vector4 (1.0, 2.0, 3.0, 4.0) (5.0, 6.0, 7.0, 8.0) (9.0, 10.0, 11.0, 12.0)");
        }

        [Test]
        public void TestExecuteVector4MultipleArgsWrongCount()
        {
            Execute("vector4  1 2 3 4  5 6 7 8  9 10 11 12  13 14 15 16", false);
            AssertResult(
                "  Wrong arguments",
                "  usage: vector4\n" +
                "         vector4 <value>\n" +
                "         vector4 <value1> <value2>\n" +
                "         vector4 <value1> <value2> <value3>"
            );
        }

        #endregion

        #region Optionals

        [Test]
        public void TestExecuteOptionalArgs()
        {
            Execute("optionals");
            AssertResult("");
        }

        [Test]
        public void TestExecuteOptionalArgs1()
        {
            Execute("optionals arg");
            AssertResult("arg");
        }

        [Test]
        public void TestExecuteOptionalArgs2()
        {
            Execute("optionals arg1 arg2");
            AssertResult("arg1, arg2, null");
        }

        #endregion

        #region Setup

        [SetUp]
        public void SetUp()
        {
            RunSetUp();

            this.IsTrackTerminalLog = true;

            RegisterCommands(new Cmd_string(this.Result));
            RegisterCommands(new Cmd_int(this.Result));
            RegisterCommands(new Cmd_float(this.Result));
            RegisterCommands(new Cmd_bool(this.Result));
            RegisterCommands(new Cmd_vector2(this.Result));
            RegisterCommands(new Cmd_vector3(this.Result));
            RegisterCommands(new Cmd_vector4(this.Result));
            RegisterCommands(new Cmd_strings(this.Result));
            RegisterCommands(new Cmd_ints(this.Result));
            RegisterCommands(new Cmd_floats(this.Result));
            RegisterCommands(new Cmd_bools(this.Result));
            RegisterCommands(new Cmd_optionals(this.Result));
        }

        [TearDown]
        public void TearDown()
        {
            RunTearDown();
        }

        #endregion

        class GenericArgumentsCmd<T> : CCommand
        {
            private List<string> m_result;

            public GenericArgumentsCmd(string name, List<string> result)
                : base(name)
            {
                m_result = result;
            }

            void Execute()
            {
                m_result.Add(this.Name);
            }

            void Execute(T value)
            {
                m_result.Add(this.Name + " " + value);
            }

            void Execute(T value1, T value2)
            {
                m_result.Add(this.Name + " " + value1 + " " + value2);
            }

            void Execute(T value1, T value2, T value3)
            {
                m_result.Add(this.Name + " " + value1 + " " + value2 + " " + value3);
            }
        }

        class Cmd_string : GenericArgumentsCmd<string>
        {
            public Cmd_string(List<string> result)
                : base("string", result)
            {
            }
        }

        class Cmd_int : GenericArgumentsCmd<int>
        {
            public Cmd_int(List<string> result)
                : base("int", result)
            {
            }
        }

        class Cmd_float : GenericArgumentsCmd<float>
        {
            public Cmd_float(List<string> result)
                : base("float", result)
            {
            }
        }

        class Cmd_bool : GenericArgumentsCmd<bool>
        {
            public Cmd_bool(List<string> result)
                : base("bool", result)
            {
            }
        }

        class Cmd_vector2 : GenericArgumentsCmd<Vector2>
        {
            public Cmd_vector2(List<string> result)
                : base("vector2", result)
            {
            }
        }

        class Cmd_vector3 : GenericArgumentsCmd<Vector3>
        {
            public Cmd_vector3(List<string> result)
                : base("vector3", result)
            {
            }
        }

        class Cmd_vector4 : GenericArgumentsCmd<Vector4>
        {
            public Cmd_vector4(List<string> result)
                : base("vector4", result)
            {
            }
        }

        class Cmd_strings : CCommand
        {
            private List<string> m_result;

            public Cmd_strings(List<string> result)
                : base("strings")
            {
                m_result = result;
            }

            void Execute()
            {
                m_result.Add("strings no args");
            }

            void Execute(string[] args)
            {
                m_result.Add("strings " + StringUtils.Join(args));
            }
        }

        class Cmd_ints : CCommand
        {
            private List<string> m_result;

            public Cmd_ints(List<string> result)
                : base("ints")
            {
                m_result = result;
            }

            void Execute()
            {
                m_result.Add("ints no args");
            }

            void Execute(int[] args)
            {
                m_result.Add("ints " + StringUtils.Join(args));
            }
        }

        class Cmd_floats : CCommand
        {
            private List<string> m_result;

            public Cmd_floats(List<string> result)
                : base("floats")
            {
                m_result = result;
            }

            void Execute()
            {
                m_result.Add("floats no args");
            }

            void Execute(float[] args)
            {
                m_result.Add("floats " + StringUtils.Join(args));
            }
        }

        class Cmd_bools : CCommand
        {
            private List<string> m_result;

            public Cmd_bools(List<string> result)
                : base("bools")
            {
                m_result = result;
            }

            void Execute()
            {
                m_result.Add("bools no args");
            }

            void Execute(bool[] args)
            {
                m_result.Add("bools " + StringUtils.Join(args));
            }
        }

        class Cmd_optionals : CCommand
        {
            private List<string> m_result;

            public Cmd_optionals(List<string> result)
                : base("optionals")
            {
                m_result = result;
            }

            void Execute()
            {
                m_result.Add("");
            }

            void Execute(string arg1)
            {
                m_result.Add(ToArg(arg1));
            }

            void Execute(string arg1, string arg2, string arg3 = null)
            {
                m_result.Add(ToArg(arg1) + ", " + ToArg(arg2) + ", " + ToArg(arg3));
            }

            String ToArg(string arg)
            {
                return arg != null ? arg : "null";
            }
        }
    }
}

