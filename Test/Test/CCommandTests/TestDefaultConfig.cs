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
        public void TestOverwriteConfigVariables()
        {
            CVar c_int = new CVar("int", 10);
            CVar c_bool = new CVar("bool", true);
            CVar c_float = new CVar("float", 3.14f);
            CVar c_string = new CVar("string", "Default string");

            Execute("int 20");
            Execute("bool 0");
            Execute("float 6.28");
            Execute("string 'New value'");

            RunUpdate(); // dispatch notifications
            RunUpdate(); // save config

            AssertConfig(
                "// cvars",
                "bool 0",
                "float 6.28",
                "int 20",
                "string \"New value\""
            );

            Clear(false); // delete everything except configs

            c_int = new CVar("int", 10);
            c_bool = new CVar("bool", true);
            c_float = new CVar("float", 3.14f);
            c_string = new CVar("string", "Default string");

            // restore commands
            RegisterCommands();

            // load default config
            Execute("exec default.cfg"); // TODO: make a convinience method for it

            // check loaded values
            Assert.AreEqual(20, c_int.IntValue);
            Assert.AreEqual(false, c_bool.BoolValue);
            Assert.AreEqual(6.28f, c_float.FloatValue);
            Assert.AreEqual("New value", c_string.Value);

            // reset variables
            Execute("reset int");
            Execute("reset bool");
            Execute("reset float");
            Execute("reset string");

            RunUpdate(); // dispatch notifications
            RunUpdate(); // save config

            AssertConfig();

            Clear(false); // delete everything except configs

            c_int = new CVar("int", 10);
            c_bool = new CVar("bool", true);
            c_float = new CVar("float", 3.14f);
            c_string = new CVar("string", "Default string");

            // Restore commands
            RegisterCommands();

            // load default config
            Execute("exec default.cfg"); // TODO: make a convinience method for it

            // check default values
            Assert.AreEqual(10, c_int.IntValue);
            Assert.AreEqual(true, c_bool.BoolValue);
            Assert.AreEqual(3.14f, c_float.FloatValue);
            Assert.AreEqual("Default string", c_string.Value);
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

