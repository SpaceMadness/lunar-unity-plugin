using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using LunarPlugin;
using LunarPluginInternal;
using LunarEditor;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;
    using Option = CCommand.Option;

    [TestFixture]
    public class CCommandTestPrintUsage : CCommandTestFixture
    {
        [Test]
        public void TestPrintUsageArrayArg()
        {
            Execute("man test1");
            AssertResult("  usage: test1 ...");
        }

        [Test]
        public void TestPrintUsageMultipleExecuteMethods()
        {
            Execute("man test2");
            AssertResult(
                "  usage: test2\n" + 
                "         test2 <name>\n" + 
                "         test2 <name> <command>"
            );
        }

        [Test]
        public void TestPrintUsageExecuteMethodOptionalArgs()
        {
            Execute("man test3");
            AssertResult("  usage: test3 <name> [<command>]");
        }

        [Test]
        public void TestPrintOptionsUsage()
        {
            Execute("man test4");
            AssertResult("  usage:" +
                " test4" + 
                " [--str <string>]" +
                " [--int <int>]" +
                " [--float <float>]" +
                " [--bool]" +
                " [--ints <int,int>]" +
                " [--floats <float,float,float>]" +
                " [--bools <bool,bool,bool,bool>]" +
                " [--strings <string,string,string,string,string>]" +
                " <arg>"
            );
        }

        #region Setup

        [SetUp]
        public void SetUp()
        {
            RunSetUp();

            this.IsTrackTerminalLog = true;

            registerCommand(typeof(Cmd_man));

            registerCommand(typeof(Cmd_test1), false);
            registerCommand(typeof(Cmd_test2), false);
            registerCommand(typeof(Cmd_test3), false);
            registerCommand(typeof(Cmd_test4), false);
        }

        [TearDown]
        public void TearDown()
        {
            RunTearDown();
        }

        #endregion

        #region Inner classes

        class Cmd_test1 : CCommand
        {
            void Execute(string[] args)
            {
            }
        }

        class Cmd_test2 : CCommand
        {
            void Execute()
            {
            }

            void Execute(string name)
            {
            }

            void Execute(string name, string command)
            {
            }
        }

        class Cmd_test3 : CCommand
        {
            void Execute(string name, string command = null)
            {
            }
        }

        class Cmd_test4 : CCommand
        {
            [CCommandOption(Name = "str")]
            public string m_str;

            [CCommandOption(Name = "int")]
            public int m_int;

            [CCommandOption(Name = "float")]
            public float m_float;

            [CCommandOption(Name = "bool")]
            public bool m_bool;

            [CCommandOption()]
            public int[] ints = new int[2];

            [CCommandOption()]
            public float[] floats = new float[3];

            [CCommandOption()]
            public bool[] bools = new bool[4];

            [CCommandOption()]
            public string[] strings = new string[5];

            void Execute(string arg)
            {
            }
        }

        #endregion
    }
}

