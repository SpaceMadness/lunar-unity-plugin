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

    public abstract class CCommandTestFixture : TestFixtureBase, ICCommandDelegate
    {
        private CommandProcessor m_commandProcessor;

        #region Setup

        protected override void RunSetUp()
        {
            base.RunSetUp();

            this.IsTrackConsoleLog = false;
            this.IsTrackTerminalLog = false;

            CBindings.Clear();
            CRegistery.Clear();

            m_commandProcessor = new CommandProcessor();
            m_commandProcessor.CommandDelegate = this;
        }

        protected override void RunTearDown()
        {
            CBindings.Clear();
            CRegistery.Clear();

            m_commandProcessor = null;

            base.RunTearDown();
        }

        #endregion

        #region ICCommandDelegate

        public void LogTerminal(string message)
        {
            if (IsTrackTerminalLog)
            {
                AddResult(message);
            }
        }

        public void LogTerminal(string[] table)
        {
            if (IsTrackTerminalLog)
            {
                AddResult(StringUtils.Join(table));
            }
        }

        public void LogTerminal(Exception e, string message)
        {
            throw new NotImplementedException();
        }

        public void ClearTerminal()
        {
        }

        public bool ExecuteCommandLine(string commandLine, bool manualMode = false)
        {
            return m_commandProcessor.TryExecute(commandLine, manualMode);
        }

        public void PostNotification(CCommand cmd, string name, params object[] data)
        {
        }

        public bool IsPromptEnabled
        {
            get { return false; }
        }

        #endregion

        #region Helpers

        protected void registerCommand(Type type, bool hidden = true)
        {
            CCommand command = ClassUtils.CreateInstance<CCommand>(type);
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
            RuntimeResolver.ResolveOptions(command);

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

        protected void AssertResult(params string[] expected)
        {
            AssertList(this.Result, expected);
        }

        protected bool Execute(string format, params object[] args)
        {
            return Execute(string.Format(format, args));
        }

        protected bool Execute(string commandLine)
        {
            return m_commandProcessor.TryExecute(commandLine, true);
        }

        protected void assertSuggestions(String line, params String[] expected)
        {
            int index = line.IndexOf('¶');
            Assert.IsTrue(index != -1);

            String[] actual = StringUtils.RemoveRichTextTags(CommandAutocompletion.getSuggestions(line.Replace("¶", ""), index));
            Assert.AreEqual(actual, expected);
        }

        protected void AddResult(string format, params object[] args)
        {
            this.Result.Add(StringUtils.RemoveRichTextTags(string.Format(format, args)));
        }

        #endregion

        #region Properties

        protected bool IsTrackConsoleLog { get; set; }
        protected bool IsTrackTerminalLog { get; set; }

        #endregion
    }
}

