using System;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using LunarPlugin;
using LunarPluginInternal;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class CCommandTestCommandsListing : CCommandTest
    {
        #region List commands

        [Test]
        public void TestListCommands()
        {
            Execute("cmdlist");
            AssertResult(
                "cmd_1," +
                "cmd_12," +
                "cmd_2," +
                "cmd_alias1," +
                "cmd_alias12," +
                "cmd_alias2," +
                "cmd_debug_1," +
                "cmd_debug_12," +
                "cmd_debug_2," +
                "cmd_delegate_1," +
                "cmd_delegate_12," +
                "cmd_delegate_2"
            );
        }

        [Test]
        public void TestListCommandsPrefix()
        {
            Execute("cmdlist cmd_1");
            AssertResult(
                "cmd_1," +
                "cmd_12"
            );
        }

        [Test]
        public void TestListCommandsPrefix2()
        {
            Execute("cmdlist cmd_123");
            AssertResult();
        }

        [Test]
        public void TestListCommandsPrefixIgnoreCase()
        {
            Execute("cmdlist CMD_1");
            AssertResult(
                "cmd_1," +
                "cmd_12"
            );
        }

        [Test]
        public void TestListCommandsPrefixDebugCommands()
        {
            Execute("cmdlist cmd_debug_1");
            AssertResult(
                "cmd_debug_1," +
                "cmd_debug_12"
            );
        }

        [Test]
        public void TestListAllCommands()
        {
            Execute("cmdlist -a");
            AssertResult(
                "cmd_1," +
                "cmd_12," +
                "cmd_2," +
                "cmd_alias1," +
                "cmd_alias12," +
                "cmd_alias2," +
                "cmd_debug_1," +
                "cmd_debug_12," +
                "cmd_debug_2," +
                "cmd_delegate_1," +
                "cmd_delegate_12," +
                "cmd_delegate_2," +
                "cmd_system_1," +
                "cmd_system_12," +
                "cmd_system_2"
            );
        }

        [Test]
        public void TestListCommandsPrefixAllCommands()
        {
            Execute("cmdlist -a cmd_system_1");
            AssertResult(
                "cmd_system_1," +
                "cmd_system_12"
            );
        }

        [Test]
        public void TestListCommandsRelease()
        {
            OverrideDebugMode(false);

            Execute("cmdlist");
            AssertResult(
                "cmd_1," +
                "cmd_12," +
                "cmd_2," +
                "cmd_alias1," +
                "cmd_alias12," +
                "cmd_alias2," +
                "cmd_delegate_1," +
                "cmd_delegate_12," +
                "cmd_delegate_2"
            );
        }

        [Test]
        public void TestListAllCommandsRelease()
        {
            OverrideDebugMode(false);

            Execute("cmdlist -a");
            AssertResult(
                "cmd_1," +
                "cmd_12," +
                "cmd_2," +
                "cmd_alias1," +
                "cmd_alias12," +
                "cmd_alias2," +
                "cmd_delegate_1," +
                "cmd_delegate_12," +
                "cmd_delegate_2," +
                "cmd_system_1," +
                "cmd_system_12," +
                "cmd_system_2"
            );
        }

        [Test]
        public void TestListCommandsPrefixDebugCommandsRelease()
        {
            OverrideDebugMode(false);

            Execute("cmdlist cmd_debug_1");
            AssertResult();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region List cvars

        [Test]
        public void TestListVars()
        {
            Execute("cvarlist -s");
            AssertResult(
                "cvar_debug_1," +
                "cvar_debug_12," +
                "cvar_debug_2," +
                "cvar_normal_1," +
                "cvar_normal_12," +
                "cvar_normal_2"
            );
        }

        [Test]
        public void TestListCvarPrefix()
        {
            Execute("cvarlist -s cvar_normal");
            AssertResult(
                "cvar_normal_1," +
                "cvar_normal_12," +
                "cvar_normal_2"
            );
        }

        [Test]
        public void TestListVarsPrefix2()
        {
            Execute("cvarlist -s cvar_normaly");
            AssertResult();
        }

        [Test]
        public void TestListVarsPrefixIgnoreCase()
        {
            Execute("cvarlist -s CVAR_NORMAL");
            AssertResult(
                "cvar_normal_1," +
                "cvar_normal_12," +
                "cvar_normal_2"
            );
        }

        [Test]
        public void TestListVarsPrefixDebugCommands()
        {
            Execute("cvarlist -s cvar_debug");
            AssertResult(
                "cvar_debug_1," +
                "cvar_debug_12," +
                "cvar_debug_2"
            );
        }

        [Test]
        public void TestListAllVars()
        {
            Execute("cvarlist -s -a");
            AssertResult(
                "cvar_debug_1," +
                "cvar_debug_12," +
                "cvar_debug_2," +
                "cvar_normal_1," +
                "cvar_normal_12," +
                "cvar_normal_2," + 
                "cvar_system_1," +
                "cvar_system_12," +
                "cvar_system_2"
            );
        }

        [Test]
        public void TestListVarsPrefixAllCommands()
        {
            Execute("cvarlist -s -a cvar_system");
            AssertResult(
                "cvar_system_1," +
                "cvar_system_12," +
                "cvar_system_2"
            );
        }

        [Test]
        public void TestListVarsRelease()
        {
            OverrideDebugMode(false);

            Execute("cvarlist -s");
            AssertResult(
                "cvar_normal_1," +
                "cvar_normal_12," +
                "cvar_normal_2"
            );
        }

        [Test]
        public void TestListAllVarsRelease()
        {
            OverrideDebugMode(false);

            Execute("cvarlist -s -a");
            AssertResult(
                "cvar_normal_1," +
                "cvar_normal_12," +
                "cvar_normal_2," + 
                "cvar_system_1," +
                "cvar_system_12," +
                "cvar_system_2"
            );
        }

        [Test]
        public void TestListCommandsPrefixDebugVarsRelease()
        {
            OverrideDebugMode(false);

            Execute("cvarlist -s cvar_debug_");
            AssertResult();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region List Aliases

        [Test]
        public void TestListAliases()
        {
            Execute("aliaslist -s");
            AssertResult(
                "cmd_alias1," +
                "cmd_alias12," +
                "cmd_alias2"
            );
        }

        [Test]
        public void TestListAliasesPrefix()
        {
            Execute("aliaslist -s cmd_alias1");
            AssertResult(
                "cmd_alias1," +
                "cmd_alias12"
            );
        }

        [Test]
        public void TestListAliasesPrefix2()
        {
            Execute("aliaslist -s cmd_aliases");
            AssertResult();
        }

        [Test]
        public void TestListAliasesPrefixIgnoreCase()
        {
            Execute("aliaslist -s CMD_ALIAS1");
            AssertResult(
                "cmd_alias1," +
                "cmd_alias12"
            );
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Setup

        [SetUp]
        public void SetUp()
        {
            RunSetUp();

            OverrideDebugMode(true);

            this.IsTrackTerminalLog = true;

            RegisterCommands(
                new cmdlist(),
                new cvarlist(),
                new alias(),
                new aliaslist(),
                new cmd_hidden("hidden_cmd"),
                new cmd_system("cmd_system_1"),
                new cmd_system("cmd_system_12"),
                new cmd_system("cmd_system_2"),
                new cmd_debug("cmd_debug_1"),
                new cmd_debug("cmd_debug_12"),
                new cmd_debug("cmd_debug_2")
            );

            new CVar("cvar_normal_1",  "value_normal_1");
            new CVar("cvar_normal_12", "value_normal_12");
            new CVar("cvar_normal_2",  "value_normal_2");
            new CVar("cvar_debug_1",  "value_debug_1", CFlags.Debug);
            new CVar("cvar_debug_12", "value_debug_12", CFlags.Debug);
            new CVar("cvar_debug_2",  "value_debug_2", CFlags.Debug);
            new CVar("cvar_system_1",  "value_debug_1", CFlags.System);
            new CVar("cvar_system_12", "value_debug_12", CFlags.System);
            new CVar("cvar_system_2",  "value_debug_2", CFlags.System);
            new CVar("cvar_hidden",  "cvar_hidden", CFlags.Hidden);

            RegisterCommands("cmd_1", "cmd_12", "cmd_2");

            Execute("alias cmd_alias1 cmdlist");
            Execute("alias cmd_alias12 cmdlist");
            Execute("alias cmd_alias2 cmdlist");

            Lunar.RegisterCommand("cmd_delegate_1", delegate(string[] args) {});
            Lunar.RegisterCommand("cmd_delegate_12", delegate(string[] args) {});
            Lunar.RegisterCommand("cmd_delegate_2", delegate(string[] args) {});
        }

        [TearDown]
        public void TearDown()
        {
            RunTearDown();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Commands

        public class cmd_hidden : CCommandMock
        {
            public cmd_hidden(string name)
                : base(name)
            {
                this.Flags = CCommandFlags.Hidden;
            }
        }  

        public class cmd_system : CCommandMock
        {
            public cmd_system(string name)
                : base(name)
            {
                this.Flags = CCommandFlags.System;
            }
        }

        public class cmd_debug : CCommandMock
        {
            public cmd_debug(string name)
                : base(name)
            {
                this.Flags = CCommandFlags.Debug;
            }
        }

        #endregion
    }
}

