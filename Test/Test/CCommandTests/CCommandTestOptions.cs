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
    using CommandActionEx = CommandAction<CCommand, string[]>;

    [TestFixture]
    public class CCommandTestOptions : CCommandTest
    {
        #region List options

        [Test()]
        public void TestListOptions()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListOptions(cmd);
            AssertArray(names, "op1", "op12", "op123");
        }

        [Test()]
        public void TestListOptionsWithToken()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListOptions(cmd, "o");
            AssertArray(names, "op1", "op12", "op123");
        }

        [Test()]
        public void TestListOptionsWithToken2()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListOptions(cmd, "op");
            AssertArray(names, "op1", "op12", "op123");
        }

        [Test()]
        public void TestListOptionsWithToken3()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListOptions(cmd, "op1");
            AssertArray(names, "op1", "op12", "op123");
        }

        [Test()]
        public void TestListOptionsWithToken4()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListOptions(cmd, "op12");
            AssertArray(names, "op12", "op123");
        }

        [Test()]
        public void TestListOptionsWithToken5()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListOptions(cmd, "op123");
            AssertArray(names, "op123");
        }

        [Test()]
        public void TestListOptionsWithToken6()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListOptions(cmd, "op1234");
            AssertArray(names);
        }

        [Test()]
        public void TestListOptionsWithToken7()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListOptions(cmd, "");
            AssertArray(names, "op1", "op12", "op123");
        }

        #endregion

        #region List short options

        [Test()]
        public void TestListShortOptions()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListShortOptions(cmd);
            AssertArray(names, "o1", "o12", "o123");
        }

        [Test()]
        public void TestListShortOptionsWithToken()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListShortOptions(cmd, "o");
            AssertArray(names, "o1", "o12", "o123");
        }

        [Test()]
        public void TestListShortOptionsWithToken2()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListShortOptions(cmd, "o1");
            AssertArray(names, "o1", "o12", "o123");
        }

        [Test()]
        public void TestListShortOptionsWithToken3()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListShortOptions(cmd, "o12");
            AssertArray(names, "o12", "o123");
        }

        [Test()]
        public void TestListShortOptionsWithToken4()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListShortOptions(cmd, "o123");
            AssertArray(names, "o123");
        }

        [Test()]
        public void TestListShortOptionsWithToken5()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListShortOptions(cmd, "o1234");
            AssertArray(names);
        }

        [Test()]
        public void TestListShortOptionsWithToken6()
        {
            CCommand cmd = new cmd_test_list();
            string[] names = ListShortOptions(cmd, "");
            AssertArray(names, "o1", "o12", "o123");
        }

        #endregion

        #region Settings options

        [Test()]
        public void TestOptions()
        {
            List<string> argsList = new List<string>(new string[] 
            {
                "test", 
                "--privateStr", "test",
                "--privateInt", "10",
                "--privateFloat", "3.14",
                "--privateBool",
                "--publicStr", "test 2",
                "--publicInt", "20",
                "--publicFloat", "6.28",
                "--publicBool",
                "--ints", "1", "2",
                "--floats", "3.14", "-3.14",
                "--bools", "yes", "no",
                "--strings", "A", "B",
                "-s", "test 3",
            });

            bool delegateCalled = false;
            CCommand cmd = new cmd_test(delegate(CCommand command, string[] args) {
                cmd_test test = (cmd_test)command;
                Assert.AreEqual(test.privateStr, "test");
                Assert.AreEqual(test.privateInt, 10);
                Assert.AreEqual(test.privateFloat, 3.14f);
                Assert.AreEqual(test.privateBool, true);
                Assert.AreEqual(test.publicStr, "test 2");
                Assert.AreEqual(test.publicInt, 20);
                Assert.AreEqual(test.publicFloat, 6.28f);
                Assert.AreEqual(test.publicBool, true);
                AssertArray(test.ints, 1, 2);
                AssertArray(test.floats, 3.14f, -3.14f);
                AssertArray(test.bools, true, false);
                AssertArray(test.strings, "A", "B");
                Assert.AreEqual(test.shortie, "test 3");
                delegateCalled = true;
            });
            cmd.ExecuteTokens(argsList);
            Assert.IsTrue(delegateCalled);
        }

        [Test()]
        public void TestEmptyOptionValue()
        {
            List<string> argsList = new List<string>(new string[] 
            {
                "test", 
                "-s", "",
                "arg1 with space",
                "arg2",
                "arg3"
            });

            bool delegateCalled = false;
            CCommand cmd = new cmd_test(delegate(CCommand command, string[] args) {
                cmd_test test = (cmd_test)command;
                Assert.AreEqual(test.shortie, "");
                AssertArray(args, "arg1 with space", "arg2", "arg3");
                delegateCalled = true;
            });
            cmd.ExecuteTokens(argsList);
            Assert.IsTrue(delegateCalled);
        }

        [Test()]
        public void TestDefaultOptionsValues()
        {
            List<string> argsList = new List<string>(new string[] 
            {
                "test_default", 
            });

            bool delegateCalled = false;
            cmd_test_default cmd = new cmd_test_default();
            cmd.ExecutionDelegate = delegate(CCommand command, string[] args) {
                cmd_test_default test = (cmd_test_default)command;
                Assert.AreEqual(test.s, "string");
                Assert.AreEqual(test.i, 10);
                Assert.AreEqual(test.f, 3.14f);
                Assert.AreEqual(test.b, true);
                AssertArray(test.ints, 10, 20);
                AssertArray(test.floats, 3.14f, -3.14f);
                AssertArray(test.bools, true, false);
                AssertArray(test.strings, "one", "two");
                delegateCalled = true;
            };

            cmd.ExecuteTokens(argsList);
            Assert.IsTrue(delegateCalled);

            delegateCalled = false;
            cmd.ExecuteTokens(argsList);

            Assert.IsTrue(delegateCalled);
        }

        [Test()]
        public void TestOverrideDefaultOptions()
        {
            cmd_test_default cmd = new cmd_test_default();

            List<string> argsList = new List<string>(new string[] 
            {
                "test_default",
                "--s", "foo",
                "--i", "20",
                "--f", "6.28",
                "--ints", "30", "40",
                "--floats", "6.28", "-6.28",
                "--bools", "false", "true",
                "--strings", "three", "four",
            });

            bool delegateCalled = false;
            cmd.ExecutionDelegate = delegate(CCommand command, string[] args) 
            {
                cmd_test_default test = (cmd_test_default)command;
                Assert.AreEqual(test.s, "foo");
                Assert.AreEqual(test.i, 20);
                Assert.AreEqual(test.f, 6.28f);
                AssertArray(test.ints, 30, 40);
                AssertArray(test.floats, 6.28f, -6.28f);
                AssertArray(test.bools, false, true);
                AssertArray(test.strings, "three", "four");
                delegateCalled = true;
            };

            cmd.ExecuteTokens(argsList);
            Assert.IsTrue(delegateCalled);

            argsList = new List<string>(new string[] 
            {
                "test_default",
            });
            cmd.ExecutionDelegate = delegate(CCommand command, string[] args) 
            {
                cmd_test_default test = (cmd_test_default)command;
                Assert.AreEqual(test.s, "string");
                Assert.AreEqual(test.i, 10);
                Assert.AreEqual(test.f, 3.14f);
                Assert.AreEqual(test.b, true);
                AssertArray(test.ints, 10, 20);
                AssertArray(test.floats, 3.14f, -3.14f);
                AssertArray(test.bools, true, false);
                AssertArray(test.strings, "one", "two");
                delegateCalled = true;
            };

            delegateCalled = false;
            cmd.ExecuteTokens(argsList);
            Assert.IsTrue(delegateCalled);

            argsList = new List<string>(new string[] 
            {
                "test_default",
                // "--s", "foo",
                "--i", "20",
                // "--f", "6.28",
                "--ints", "30", "40",
                // "--floats", "6.28", "-6.28",
                "--bools", "false", "true",
                // "--strings", "three", "four",
            });
            cmd.ExecutionDelegate = delegate(CCommand command, string[] args) 
            {
                cmd_test_default test = (cmd_test_default)command;
                Assert.AreEqual(test.s, "string");
                Assert.AreEqual(test.i, 20);
                Assert.AreEqual(test.f, 3.14f);
                AssertArray(test.ints, 30, 40);
                AssertArray(test.floats, 3.14f, -3.14f);
                AssertArray(test.bools, false, true);
                AssertArray(test.strings, "one", "two");
                delegateCalled = true;
            };

            delegateCalled = false;
            cmd.ExecuteTokens(argsList);
            Assert.IsTrue(delegateCalled);
        }

        #endregion

        #region Helpers

        private string[] ListOptions(CCommand cmd, string token = null)
        {
            IList<Option> options = cmd.ListOptions(token);
            string[] names = new string[options.Count];
            for (int i = 0; i < options.Count; ++i)
            {
                names[i] = options[i].Name;
            }

            return names;
        }

        private string[] ListShortOptions(CCommand cmd, string token = null)
        {
            IList<Option> options = cmd.ListShortOptions(token);
            string[] names = new string[options.Count];
            for (int i = 0; i < options.Count; ++i)
            {
                names[i] = options[i].ShortName;
            }

            return names;
        }

        #endregion

        #region Private classes

        class cmd_test : CCommand
        {
            private CommandActionEx m_delegate;

            [CCommandOption(Name = "privateStr")]
            private string m_privateStr;

            [CCommandOption(Name = "privateInt")]
            private int m_privateInt;

            [CCommandOption(Name = "privateFloat")]
            private float m_privateFloat;

            [CCommandOption(Name = "privateBool")]
            private bool m_privateBool;

            [CCommandOption()]
            public string publicStr;

            [CCommandOption()]
            public int publicInt;

            [CCommandOption()]
            public float publicFloat;

            [CCommandOption()]
            public bool publicBool;

            [CCommandOption()]
            public int[] ints = new int[2];

            [CCommandOption()]
            public float[] floats = new float[2];

            [CCommandOption()]
            public bool[] bools = new bool[2];

            [CCommandOption()]
            public string[] strings = new string[2];

            [CCommandOption(ShortName="s")]
            public string shortie;

            public cmd_test(CommandActionEx del)
            {
                m_delegate = del;
                ResolveOptions(this);
            }

            void Execute(string[] args)
            {
                m_delegate(this, args);
            }

            public string privateStr { get { return m_privateStr; }}
            public int privateInt { get { return m_privateInt; } }
            public float privateFloat { get { return m_privateFloat; } }
            public bool privateBool { get { return m_privateBool; } }
        }

        class cmd_test_default : CCommand
        {
            [CCommandOption()]
            public string s = "string";

            [CCommandOption()]
            public int i = 10;

            [CCommandOption()]
            public float f = 3.14f;

            [CCommandOption()]
            public bool b = true;

            [CCommandOption()]
            public int[] ints = { 10, 20 };

            [CCommandOption()]
            public float[] floats = { 3.14f, -3.14f };

            [CCommandOption()]
            public bool[] bools = { true, false };

            [CCommandOption()]
            public string[] strings = { "one", "two" };

            public cmd_test_default()
            {
                ResolveOptions(this);
            }

            void Execute(string[] args)
            {
                ExecutionDelegate(this, args);
            }

            public CommandActionEx ExecutionDelegate { get; set; }
        }

        class cmd_test_list : CCommand
        {
            [CCommandOption(ShortName="o1")]
            public string op1;

            [CCommandOption(ShortName="o12")]
            public string op12;

            [CCommandOption(ShortName="o123")]
            public string op123;

            public cmd_test_list()
            {
                ResolveOptions(this);
            }

            void Execute()
            {
            }
        }

        #endregion
    }
}

