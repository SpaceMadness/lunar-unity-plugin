using System;
using System.Collections.Generic;

using NUnit.Framework;

using LunarPlugin;
using LunarPluginInternal;
using LunarEditor;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class CCommandTestAlias : CCommandTestFixture
    {
        [Test]
        public void TestSingleAlias()
        {
            Execute("alias test \"echo 'some string'\"");
            Execute("test");

            AssertResult("echo 'some string'");
        }

        [Test]
        public void TestSingleAliasSingleQuote()
        {
            Execute("alias test 'echo \"some string\"'");
            Execute("test");

            AssertResult("echo \"some string\"");
        }

        [Test]
        public void TestOverrideAlias()
        {
            Execute("alias test 'echo \"some string\"'");
            Execute("alias test 'echo \"some other string\"'");
            Execute("test");

            AssertResult("echo \"some other string\"");
        }

        [Test]
        public void TestMultipleCommandAlias()
        {
            Execute("alias test \"echo 'some string' && echo 'some other string'\"");
            Execute("test");

            AssertResult(
                "echo 'some string'",
                "echo 'some other string'"
            );
        }

        [Test]
        public void TestUnalias()
        {
            Execute("alias test \"echo 'some string'\"");
            Execute("test");
            Execute("unalias test");
            Execute("test");

            AssertResult("echo 'some string'");
        }

        [Test]
        public void TestConfigAliases()
        {
            Execute("alias a1 'echo \"alias 1\"'");
            Execute("alias a2 \"echo 'alias 2'\"");
            Execute("alias a3 \"echo \\\"alias 3\\\"\"");

            List<string> lines = new List<string>();
            Cmd_alias.ListAliasesConfig(lines);

            SetUp(); // reset everything

            foreach (string cmd in lines)
            {
                Execute(cmd);
            }

            Execute("a1");
            Execute("a2");
            Execute("a3");

            AssertResult(
                "echo \"alias 1\"",
                "echo 'alias 2'",
                "echo \"alias 3\""
            );
        }

        #region Setup

        [SetUp]
        public void SetUp()
        {
            RunSetUp();

            CRegistery.Register(new alias());
            CRegistery.Register(new unalias());
            Lunar.RegisterCommand("echo", delegate(CCommand cmd, string[] args)
            {
                AddResult(cmd.CommandString);
            });
        }

        [TearDown]
        public void TearDown()
        {
            RunTearDown();
        }

        #endregion
    }
}

