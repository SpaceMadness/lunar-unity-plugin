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
            Execute("test", false);

            AssertResult("echo 'some string'");
        }

        #region Setup

        [SetUp]
        public void SetUp()
        {
            RunSetUp();

            RegisterCommand(typeof(Cmd_alias));
            RegisterCommand(typeof(Cmd_unalias));

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

