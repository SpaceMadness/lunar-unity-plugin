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
    public class CVarDelegateTest : CCommandTest
    {
        private List<string> m_result;

        [Test]
        public void TestBoolDelegate()
        {
            CVar cvarBool = new CVar("bool", false);
            cvarBool.AddDelegate(delegate(CVar cvar)
            {
                m_result.Add(cvar.Name + " " + cvar.BoolValue);
            });

            Execute("bool 0");
            AssertList(m_result);

            Execute("bool 1");
            AssertList(m_result, "bool True");
        }

        [Test]
        public void TestIntDelegate()
        {
            CVar cvarInt = new CVar("int", 10);
            cvarInt.AddDelegate(delegate(CVar cvar)
            {
                m_result.Add(cvar.Name + " " + cvar.IntValue);
            });

            Execute("int 10");
            AssertList(m_result);

            Execute("int 20");
            AssertList(m_result, "int 20");
        }

        [Test]
        public void TestFloatDelegate()
        {
            CVar cvarFloat = new CVar("float", 3.14f);
            cvarFloat.AddDelegate(delegate(CVar cvar)
            {
                m_result.Add(cvar.Name + " " + cvar.FloatValue);
            });

            Execute("float 3.14");
            AssertList(m_result);

            Execute("float -3.14");
            AssertList(m_result, "float -3.14");
        }

        [Test]
        public void TestStringDelegate()
        {
            CVar cvarString = new CVar("string", "This is string");
            cvarString.AddDelegate(delegate(CVar cvar)
            {
                m_result.Add(cvar.Name + " \"" + cvar.Value + "\"");
            });

            Execute("string \"This is string\"");
            AssertList(m_result);

            Execute("string \"This another string\"");
            AssertList(m_result, "string \"This another string\"");
        }

        [Test]
        public void TestBoolDefaultDelegate()
        {
            CVar cvarBool = new CVar("bool", false);
            Execute("bool 1");

            cvarBool.AddDelegate(delegate(CVar cvar)
            {
                m_result.Add(cvar.Name + " " + cvar.BoolValue);
            });

            Execute("reset bool");
            AssertList(m_result, "bool False");
        }

        [Test]
        public void TestIntDefaultDelegate()
        {
            CVar cvarInt = new CVar("int", 10);
            Execute("int 20");

            cvarInt.AddDelegate(delegate(CVar cvar)
            {
                m_result.Add(cvar.Name + " " + cvar.IntValue);
            });

            Execute("reset int");
            AssertList(m_result, "int 10");
        }

        [Test]
        public void TestFloatDefaultDelegate()
        {
            CVar cvarFloat = new CVar("float", 3.14f);
            Execute("float -3.14");

            cvarFloat.AddDelegate(delegate(CVar cvar)
            {
                m_result.Add(cvar.Name + " " + cvar.FloatValue);
            });

            Execute("reset float");
            AssertList(m_result, "float 3.14");
        }

        [Test]
        public void TestStringDefaultDelegate()
        {
            CVar cvarString = new CVar("string", "This is string");
            Execute("string \"This another string\"");

            cvarString.AddDelegate(delegate(CVar cvar)
            {
                m_result.Add(cvar.Name + " \"" + cvar.Value + "\"");
            });

            Execute("reset string");
            AssertList(m_result, "string \"This is string\"");
        }

        [Test]
        public void TestBoolDelegateMultipleDelegates()
        {
            CVar cvarBool = new CVar("bool", false);
            cvarBool.AddDelegate(delegate(CVar cvar)
            {
                m_result.Add("delegate1 " + cvar.BoolValue);
            });
            cvarBool.AddDelegate(delegate(CVar cvar)
            {
                m_result.Add("delegate2 " + cvar.BoolValue);
            });
            cvarBool.AddDelegate(delegate(CVar cvar)
            {
                m_result.Add("delegate3 " + cvar.BoolValue);
            });

            Execute("bool 0");
            AssertList(m_result);

            Execute("bool 1");
            AssertList(m_result,
                "delegate1 True",
                "delegate2 True",
                "delegate3 True"
            );
        }

        [Test]
        public void TestBoolDelegateRemoveMultipleDelegates()
        {
            CVarChangedDelegate del1 = delegate(CVar cvar)
            {
                m_result.Add("delegate1 " + cvar.BoolValue);
            };
            CVarChangedDelegate del2 = delegate(CVar cvar)
            {
                m_result.Add("delegate2 " + cvar.BoolValue);
            };
            CVarChangedDelegate del3 = delegate(CVar cvar)
            {
                m_result.Add("delegate3 " + cvar.BoolValue);
            };

            CVar cvarBool = new CVar("bool", false);
            cvarBool.AddDelegate(del1);
            cvarBool.AddDelegate(del2);
            cvarBool.AddDelegate(del3);

            Execute("bool 0");
            AssertList(m_result);
            m_result.Clear();

            Execute("bool 1");
            AssertList(m_result,
                "delegate1 True",
                "delegate2 True",
                "delegate3 True"
            );
            m_result.Clear();

            cvarBool.RemoveDelegate(del3);
            Execute("bool 0");
            AssertList(m_result,
                "delegate1 False",
                "delegate2 False"
            );
            m_result.Clear();

            cvarBool.RemoveDelegate(del2);
            Execute("bool 1");
            AssertList(m_result,
                "delegate1 True"
            );
            m_result.Clear();

            cvarBool.RemoveDelegate(del1);
            Execute("bool 0");
            AssertList(m_result);
        }

        [Test]
        public void TestBoolDelegateRemoveMultipleDelegatesInLoop()
        {
            CVarChangedDelegate del1 = delegate(CVar cvar)
            {
                m_result.Add("delegate1 " + cvar.BoolValue);
            };
            CVarChangedDelegate del2 = delegate(CVar cvar)
            {
                m_result.Add("delegate2 " + cvar.BoolValue);
                cvar.RemoveDelegate(del1);
            };

            CVar cvarBool = new CVar("bool", false);
            cvarBool.AddDelegate(del2);
            cvarBool.AddDelegate(del1);

            Execute("bool 1");
            AssertList(m_result,
                "delegate2 True"
            );
            m_result.Clear();
        }

        [Test]
        public void TestBoolDelegateRemoveAllDelegatesInLoop()
        {
            CVarChangedDelegate del1 = delegate(CVar cvar)
            {
                m_result.Add("delegate1 " + cvar.BoolValue);
            };

            CVarChangedDelegate del2 = delegate(CVar cvar)
            {
                m_result.Add("delegate2 " + cvar.BoolValue);
            };
            CVarChangedDelegate del3 = delegate(CVar cvar)
            {
                m_result.Add("delegate3 " + cvar.BoolValue);
                cvar.RemoveDelegates(del1.Target);
            };

            CVar cvarBool = new CVar("bool", false);
            cvarBool.AddDelegate(del3);
            cvarBool.AddDelegate(del2);
            cvarBool.AddDelegate(del1);

            Execute("bool 1");
            AssertList(m_result,
                "delegate3 True"
            );
            m_result.Clear();
        }

        [SetUp]
        public void SetUp()
        {
            RunSetUp();
            m_result = new List<string>();

            CRegistery.Register(new reset());
        }

        [TearDown]
        public void TearDown()
        {
            RunTearDown();
        }
    }
}

