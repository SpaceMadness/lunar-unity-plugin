//
//  CommandProcessor.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

﻿using System;
using System.Collections.Generic;
using System.Text;

using LunarPlugin;

namespace LunarPluginInternal
{
    static class CCommandNotifications
    {
        public static readonly string CVarValueChanged              = "CVarValueChanged";
        public static readonly string CVarValueChangedKeyVar        = "var"; // CVar
        public static readonly string CVarValueChangedKeyOldValue   = "old_value"; // string

        public static readonly string CBindingsChanged              = "CBindingsChanged";
        public static readonly string CAliasesChanged               = "CAliasesChanged";

        public static readonly string ConfigLoaded                  = "ConfigLoaded";

        public static readonly string KeyManualMode = "manual_mode"; // bool
        public static readonly string KeyName = "name"; // string
    }

    interface ICommandProcessorDelegate
    {
        void OnCommandExecuted(CommandProcessor processor, CCommand cmd);
        void OnCommandUnknown(CommandProcessor processor, string cmdName);
    }

    class CommandProcessor
    {
        private ICCommandDelegate m_delegate;

        public CommandProcessor()
        {
            CommandDelegate = null;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Command buffer

        public bool TryExecute(string commandLine, bool manualMode = false)
        {
            try
            {
                IList<string> commandList = CommandSplitter.Split(commandLine);
                for (int commandIndex = 0; commandIndex < commandList.Count; ++commandIndex)
                {
                    if (!TryExecuteSingleCommand(commandList[commandIndex], manualMode))
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                m_delegate.LogTerminal(StringUtils.C(e.Message, ColorCode.ErrorUnknownCommand));
                m_delegate.LogTerminal(StringUtils.C(e.StackTrace, ColorCode.ErrorUnknownCommand));
                return false;
            }

            return true;
        }

        private bool TryExecuteSingleCommand(string commandLine, bool manualMode = false)
        {
            IList<string> tokensList = CommandTokenizer.Tokenize(commandLine);
            if (tokensList.Count > 0)
            {
                string commandName = tokensList[0];

                CCommand command = CRegistery.FindCommand(commandName);
                if (command != null)
                {
                    command.Delegate = m_delegate;
                    command.IsManualMode = manualMode;
                    command.CommandString = commandLine;
                    bool succeed = command.ExecuteTokens(tokensList, commandLine);
                    command.Clear();

                    return succeed;
                }

                if (manualMode)
                {
                    m_delegate.LogTerminal(StringUtils.C(commandName + ": command not found", ColorCode.ErrorUnknownCommand));
                }
            }

            return false;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Cvar lookup

        public CVarCommand FindCvarCommand(string name)
        {
            return CRegistery.FindCvarCommand(name);
        }

        public CVar FindCvar(string name)
        {
            return CRegistery.FindCvar(name);
        }

        public CCommand FindCommand(string name)
        {
            return CRegistery.FindCommand(name);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Delegate notification

        private void NotifyCommandExecuted(CCommand cmd)
        {
            if (Delegate != null)
            {
                Delegate.OnCommandExecuted(this, cmd);
            }
        }

        private void NotifyCommandUnknown(string cmdName)
        {
            if (Delegate != null)
            {
                Delegate.OnCommandUnknown(this, cmdName);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public ICommandProcessorDelegate Delegate { get; set; }
        public ICCommandDelegate CommandDelegate
        {
            get { return m_delegate; }
            set { m_delegate = value != null ? value : NullCommandDelegate.Instance; }
        }

        #endregion
    }

    class TokenizeException : Exception
    {
        public TokenizeException(string message)
            : base(message)
        {
        }
    }
}
