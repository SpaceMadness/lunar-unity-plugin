using NUnit.Framework;

using LunarPlugin;
using LunarPluginInternal;
using LunarEditor;

using LunarPlugin.Test;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class TestDefaultConfig : CCommandTestFixture
    {
        [Test]
        public void TestWriteConfigVars()
        {
            new CVar("int", 10);
            new CVar("bool", true);
            new CVar("float", 3.14f);
            new CVar("string", "Default string");

            Execute("int 20");
            Execute("bool 0");
            Execute("float 6.28");
            Execute("string 'New value'");

            AssertConfig(
                "// cvars",
                "bool 0",
                "float 6.28",
                "int 20",
                "string \"New value\""
            );
        }

        [Test]
        public void TestReadConfigVars()
        {
            WriteVarsConfig();

            CVar c_int = new CVar("int", 10);
            CVar c_bool = new CVar("bool", true);
            CVar c_float = new CVar("float", 3.14f);
            CVar c_string = new CVar("string", "Default string");

            // load default config
            Execute("exec default.cfg"); // TODO: make a convinience method for it

            // check loaded values
            Assert.AreEqual(20, c_int.IntValue);
            Assert.AreEqual(false, c_bool.BoolValue);
            Assert.AreEqual(6.28f, c_float.FloatValue);
            Assert.AreEqual("New value", c_string.Value);
        }

        [Test]
        public void TestResetConfigVars()
        {
            WriteVarsConfig();

            new CVar("int", 10);
            new CVar("bool", true);
            new CVar("float", 3.14f);
            new CVar("string", "Default string");

            // load default config
            Execute("exec default.cfg"); // TODO: make a convinience method for it

            // reset variables
            Execute("reset int");
            Execute("reset bool");
            Execute("reset float");
            Execute("reset string");

            AssertConfig();
        }

        [Test]
        public void TestResetSomeConfigVars()
        {
            WriteVarsConfig();

            new CVar("int", 10);
            new CVar("bool", true);
            new CVar("float", 3.14f);
            new CVar("string", "Default string");

            // load default config
            Execute("exec default.cfg"); // TODO: make a convinience method for it

            // reset variables
            Execute("reset int");
            Execute("reset bool");
            Execute("reset float");

            AssertConfig(
                "// cvars",
                "string \"New value\""
            );
        }

        [Test]
        public void TestResetAllConfigVars()
        {
            WriteVarsConfig();

            new CVar("int", 10);
            new CVar("bool", true);
            new CVar("float", 3.14f);
            new CVar("string", "Default string");

            // load default config
            Execute("exec default.cfg"); // TODO: make a convinience method for it

            // reset variables
            Execute("resetAll");

            AssertConfig();
        }

        private void RegisterCommands()
        {
            RegisterCommand(typeof(Cmd_alias));
            RegisterCommand(typeof(Cmd_unalias));
            RegisterCommand(typeof(Cmd_bind));
            RegisterCommand(typeof(Cmd_unbind));
            RegisterCommand(typeof(Cmd_toggle));
            RegisterCommand(typeof(Cmd_reset));
            RegisterCommand(typeof(Cmd_resetAll));
            RegisterCommand(typeof(Cmd_exec));
            RegisterCommand(typeof(Cmd_writeconfig));
        }

        private void WriteVarsConfig()
        {
            WriteConfig(
                "bool 0",
                "float 6.28",
                "int 20",
                "string \"New value\""
            );
        }

        private new void AssertConfig(params string[] expected)
        {
            RunUpdate(); // dispatch notifications
            RunUpdate(); // save config

            base.AssertConfig(expected);
        }

        protected void ClearElements()
        {
            base.Clear(false);
            RegisterCommands();
        }

        [SetUp]
        protected override void RunSetUp()
        {
            base.RunSetUp();

            RegisterCommands();
        }

        protected override TestAppConfig CreateTestConfig()
        {
            TestAppConfig config = base.CreateTestConfig();
            config.shouldRegisterCommandNotifications = true;
            return config;
        }
    }
}

