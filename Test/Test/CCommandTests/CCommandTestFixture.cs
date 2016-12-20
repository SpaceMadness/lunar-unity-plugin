using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using LunarPlugin;
using LunarPlugin.Test;
using LunarPluginInternal;

using LunarEditor;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;
    using Option = CCommand.Option;

    public delegate bool ConfigReadFilter(string line);

    public abstract class CCommandTestFixture : AppTestFixtureBase
    {
        #region Setup

        protected override void RunSetUp()
        {
            base.RunSetUp();

            this.IsTrackConsoleLog = false;
            this.IsTrackTerminalLog = false;

            Clear();
        }

        protected override void RunTearDown()
        {
            Clear();
            base.RunTearDown();
        }

        protected virtual void Clear(bool deleteConfigs = true)
        {
            CBindings.Clear();
            CRegistery.Clear();

            if (deleteConfigs)
            {
                ConfigHelper.DeleteConfigs();
            }
        }

        #endregion

        #region Helpers

        protected void RegisterCommand(Type type, bool hidden = true)
        {
            CCommand command = CClassUtils.CreateInstance<CCommand>(type);
            if (command == null)
            {
                throw new ArgumentException("Can't create class instance: " + type.FullName);
            }

            String commandName = type.Name;
            if (commandName.StartsWith("Cmd_"))
            {
                commandName = commandName.Substring("Cmd_".Length);
            }

            command.Name = commandName;
            command.IsHidden = hidden;
            CRuntimeResolver.ResolveOptions(command);

            CRegistery.Register(command);
        }

        protected void RegisterCommands(params string[] names)
        {
            foreach (string name in names)
            {
                Lunar.RegisterCommand(name, delegate(string[] args)
                {
                    AddResult(name);
                });
            }
        }

        protected void RegisterCommands(params CCommand[] commands)
        {
            foreach (CCommand cmd in commands)
            {
                CRegistery.Register(cmd);
            }
        }

        protected void ClearResult()
        {
            this.Result.Clear();
        }

        protected bool Execute(string format, params object[] args)
        {
            return Execute(string.Format(format, args));
        }

        protected bool Execute(string commandLine, bool shouldSucceed = true)
        {
            bool result = CApp.ExecCommand(commandLine, true);
            Assert.IsFalse(result ^ shouldSucceed, "Command should " + (shouldSucceed ? "succeed" : "fail") + ": " + commandLine);
            return result;
        }

        #endregion

        #region Config

        protected void AssertConfig(params string[] expected)
        {
            AssertConfig(delegate(string line) { return true; }, expected);
        }

        protected void AssertConfig(ConfigReadFilter filter, params string[] expected)
        {
            IList<string> lines = ConfigHelper.ReadConfig(LPConstants.ConfigDefault);
            IList<string> actual = new List<string>();
            foreach (string line in lines)
            {
                if (filter(line))
                {
                    actual.Add(line);
                }
            }

            AssertList(actual, expected);
        }

        protected void WriteConfig(params string[] lines)
        {
            ConfigHelper.WriteConfig(LPConstants.ConfigDefault, lines);
        }

        #endregion
    }
}

