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

    public abstract class CCommandTest : TestFixtureBase, ICCommandDelegate
    {
        private CommandProcessor m_commandProcessor;
        protected List<string> m_result;

        #region Setup

        protected void RunSetUp()
        {
            this.IsTrackConsoleLog = false;
            this.IsTrackTerminalLog = false;

            m_result = new List<string>();

            CBindings.Clear();
            CRegistery.Clear();

            m_commandProcessor = new CommandProcessor();
            m_commandProcessor.CommandDelegate = this;
        }

        protected void RunTearDown()
        {
            CBindings.Clear();
            CRegistery.Clear();

            m_commandProcessor = null;
            m_result = null;
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

        public void LogConsole(Tag tag, LogLevel level, string message, string stackTrace)
        {
            if (IsTrackConsoleLog)
            {
                AddResult(message);
            }
        }

        public void LogTerminal(CVar[] cvars)
        {
            throw new NotImplementedException();
        }

        public void ClearTerminal()
        {
        }

        public void ClearConsole()
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
            AssertList(m_result, expected);
        }

        protected bool Execute(string format, params object[] args)
        {
            return Execute(string.Format(format, args));
        }

        protected bool Execute(string commandLine)
        {
            return m_commandProcessor.TryExecute(commandLine, true);
        }

        protected void AddResult(string format, params object[] args)
        {
            m_result.Add(StringUtils.RemoveRichTextTags(string.Format(format, args)));
        }

        #endregion

        #region Properties

        protected bool IsTrackConsoleLog { get; set; }
        protected bool IsTrackTerminalLog { get; set; }

        #endregion
    }

    class cmdlist : Cmd_cmdlist
    {
        public cmdlist()
        {
            this.Name = "cmdlist";
            this.IsHidden = true;
            ResolveOptions(this, typeof(Cmd_cmdlist));
        }

        public override Type GetCommandType()
        {
            return typeof(Cmd_cmdlist);
        }
    }

    class cvarlist : Cmd_cvarlist
    {
        public cvarlist()
        {
            this.Name = "cvarlist";
            this.IsHidden = true;
            ResolveOptions(this, typeof(Cmd_cvarlist));
        }

        public override Type GetCommandType()
        {
            return typeof(Cmd_cvarlist);
        }
    }

    class reset : Cmd_reset
    {
        public reset()
        {
            this.Name = "reset";
            this.IsHidden = true;
            ResolveOptions(this, typeof(Cmd_reset));
        }

        public override Type GetCommandType()
        {
            return typeof(Cmd_reset);
        }
    }

    class alias : Cmd_alias
    {
        public alias()
        {
            this.Name = "alias";
            this.IsHidden = true;
            ResolveOptions(this, typeof(Cmd_alias));
        }

        public override Type GetCommandType()
        {
            return typeof(Cmd_alias);
        }
    }

    class aliaslist : Cmd_aliaslist
    {
        public aliaslist()
        {
            this.Name = "aliaslist";
            this.IsHidden = true;
            ResolveOptions(this, typeof(Cmd_aliaslist));
        }

        public override Type GetCommandType()
        {
            return typeof(Cmd_aliaslist);
        }
    }

    class unalias : Cmd_unalias
    {
        public unalias()
        {
            this.Name = "unalias";
            this.IsHidden = true;
        }

        public override Type GetCommandType()
        {
            return typeof(Cmd_unalias);
        }
    }

    class bind : Cmd_bind
    {
        public bind()
        {
            this.Name = "bind";
            this.IsHidden = true;
        }

        public override Type GetCommandType()
        {
            return typeof(Cmd_bind);
        }
    }

    class unbind : Cmd_unbind
    {
        public unbind()
        {
            this.Name = "unbind";
            this.IsHidden = true;
        }

        public override Type GetCommandType()
        {
            return typeof(Cmd_unbind);
        }
    }

    class bindlist : Cmd_bindlist
    {
        public bindlist()
        {
            this.Name = "bindlist";
            this.IsHidden = true;
        }

        public override Type GetCommandType()
        {
            return typeof(Cmd_bindlist);
        }
    }

    class unbindall : Cmd_unbindall
    {
        public unbindall()
        {
            this.Name = "unbindall";
            this.IsHidden = true;
        }

        public override Type GetCommandType()
        {
            return typeof(Cmd_unbindall);
        }
    }

    class man : Cmd_man
    {
        public man()
        {
            this.Name = "man";
            this.IsHidden = true;
        }

        public override Type GetCommandType()
        {
            return typeof(Cmd_man);
        }
    }
}

