//
//  CRegistery.cs
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
using System.Reflection;

using LunarPlugin;

namespace LunarPluginInternal
{
    using CommandList = LinkedList<CCommand>;
    using CommandLookup = Dictionary<string, CCommand>;

    delegate bool CCommandsFilter<T>(T cmd) where T : CCommand ; // TODO: rename

    enum CCommandListOptions
    {
        None       = 0,
        Debug      = 1 << 0,
        Hidden     = 1 << 1,
        System     = 1 << 2,
    }

    public delegate void CommandAction();
    public delegate void CommandAction<T1>(T1 arg1);
    public delegate void CommandAction<T1, T2>(T1 arg1, T2 arg2);
    public delegate void CommandAction<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate void CommandAction<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate void CommandAction<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate void CommandAction<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate void CommandAction<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    public delegate void CommandAction<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    public delegate void CommandAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    public delegate void CommandAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
    public delegate void CommandAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
    public delegate void CommandAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
    public delegate void CommandAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
    public delegate void CommandAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
    public delegate void CommandAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
    public delegate void CommandAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);

    public delegate bool CommandFunction();
    public delegate bool CommandFunction<T1>(T1 arg1);
    public delegate bool CommandFunction<T1, T2>(T1 arg1, T2 arg2);
    public delegate bool CommandFunction<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate bool CommandFunction<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
    public delegate bool CommandFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);

    static class CRegistery
    {
        private static CommandLookup m_commandsLookup = new CommandLookup();
        private static CommandList m_commands = new CommandList(); // TODO: use fast list

        //////////////////////////////////////////////////////////////////////////////

        #region Commands resolver

        public static void ResolveElements()
        {
            CRuntimeResolver.Result result = CRuntimeResolver.Resolve();

            IList<CCommand> commands = result.Commands;
            for (int i = 0; i < commands.Count; ++i)
            {
                Register(commands[i]);
            }

            IList<Type> containerTypes = result.CvarContainersTypes;
            if (containerTypes != null)
            {
                foreach (Type type in containerTypes)
                {
                    ForceStaticInit(type);
                }
            }
        }

        private static void ForceStaticInit(Type type)
        {
            try
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Static|BindingFlags.Public);
                if (fields != null && fields.Length > 0)
                {
                    fields[0].GetValue(null);
                }
            }
            catch (Exception e)
            {
                CLog.error(e, "Unable to initialize cvar container: {0}", type);
            }

        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Commands registry

        public static bool Register(CCommand cmd)
        {
            if (cmd.Name.StartsWith("@"))
            {
                cmd.Flags |= CCommandFlags.Hidden;
            }

            return AddCommand(cmd);
        }

        public static bool Unregister(CCommand cmd)
        {
            return RemoveCommand(cmd);
        }

        internal static bool Unregister(CCommandsFilter<CCommand> filter)
        {
            bool unregistered = false;

            for (LinkedListNode<CCommand> node = m_commands.First; node != null;)
            {
                LinkedListNode<CCommand> next = node.Next;
                CCommand cmd = node.Value;
                if (filter(cmd))
                {
                    unregistered |= Unregister(cmd);
                }

                node = next;
            }

            return unregistered;
        }

        public static void Clear()
        {
            m_commands.Clear();
            m_commandsLookup.Clear();
        }

        internal static IList<CCommand> ListCommands(string prefix = null, CCommandListOptions options = CCommandListOptions.None)
        {
            return ListCommands(new List<CCommand>(), prefix, options);
        }

        internal static IList<CCommand> ListCommands(IList<CCommand> outList, string prefix = null, CCommandListOptions options = CCommandListOptions.None)
        {
            return ListCommands(outList, delegate(CCommand cmd)
            {
                return ShouldListCommand(cmd, prefix, options);
            });
        }

        internal static IList<CCommand> ListCommands(CCommandsFilter<CCommand> filter)
        {
            return ListCommands(new List<CCommand>(), filter);
        }

        internal static IList<CCommand> ListCommands(IList<CCommand> outList, CCommandsFilter<CCommand> filter)
        {
            if (filter == null)
            {
                throw new NullReferenceException("Filter is null");
            }

            foreach (CCommand command in m_commands)
            {
                if (filter(command))
                {
                    outList.Add(command);
                }
            }

            return outList;
        }

        internal static bool ShouldListCommand(CCommand cmd, string prefix, CCommandListOptions options = CCommandListOptions.None)
        {
            if (cmd.IsDebug && (options & CCommandListOptions.Debug) == 0)
            {
                return false;
            }

            if (cmd.IsSystem && (options & CCommandListOptions.System) == 0)
            {
                return false;
            }

            if (cmd.IsHidden && (options & CCommandListOptions.Hidden) == 0)
            {
                return false;
            }

            return prefix == null || CStringUtils.StartsWithIgnoreCase(cmd.Name, prefix);
        }

        private static bool AddCommand(CCommand cmd)
        {
            string name = cmd.Name;
            for (LinkedListNode<CCommand> node = m_commands.First; node != null; node = node.Next)
            {
                CCommand otherCmd = node.Value;
                if (cmd == otherCmd)
                {
                    return false; // no duplicates
                }

                string otherName = otherCmd.Name;
                int compare = name.CompareTo(otherName);
                if (compare < 0)
                {
                    m_commands.AddBefore(node, cmd);
                    m_commandsLookup.Add(cmd.Name, cmd);
                    return true;
                }

                if (compare == 0)
                {
                    node.Value = cmd;
                    return true;
                }
            }

            m_commands.AddLast(cmd);
            m_commandsLookup.Add(cmd.Name, cmd);

            return true;
        }

        private static bool RemoveCommand(CCommand command)
        {
            if (m_commands.Remove(command))
            {
                m_commandsLookup.Remove(command.Name);
                return true;
            }

            return false;
        }

        public static CCommand FindCommand(string name)
        {
            CCommand cmd;
            if (name != null && m_commandsLookup.TryGetValue(name, out cmd))
            {
                return cmd;
            }

            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Delegate commands

        public static bool RegisterDelegate(Delegate action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return RegisterDelegate(action.Method.Name, action);
        }

        public static bool RegisterDelegate(string name, Delegate action)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            CCommand existingCmd = FindCommand(name);
            if (existingCmd != null)
            {
                CDelegateCommand delegateCmd = existingCmd as CDelegateCommand;
                if (delegateCmd != null)
                {
                    CLog.w("Overriding command: {0}", name);
                    delegateCmd.ActionDelegate = action;

                    return true;
                }

                CLog.e("Another command with the same name exists: {0}", name);
                return false;
            }
            
            return Register(new CDelegateCommand(name, action));
        }

        public static bool Unregister(string commandName)
        {
            CCommand cmd = FindCommand(commandName);
            if (cmd != null)
            {
                if (cmd is CDelegateCommand)
                {
                    Unregister(cmd);
                    return true;
                }

                CLog.e("Unable to unregister a non-delegate command: {0}", cmd);
                return false;
            }

            return false;
        }

        public static bool Unregister(Delegate del)
        {
            return Unregister(delegate(CCommand cmd)
            {
                CDelegateCommand delegateCmd = cmd as CDelegateCommand;
                return delegateCmd != null && delegateCmd.ActionDelegate == del;
            });
        }

        public static bool UnregisterAll(object target)
        {
            return target != null && Unregister(delegate(CCommand cmd)
            {
                CDelegateCommand delegateCmd = cmd as CDelegateCommand;
                return delegateCmd != null && delegateCmd.ActionDelegate.Target == target;
            });
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Cvars

        public static bool Register(CVar cvar)
        {
            return Register(new CVarCommand(cvar));
        }

        public static bool Unregister(CVar cvar)
        {
            CCommand cmd = FindCvarCommand(cvar.Name);
            return cmd != null && Unregister(cmd);
        }

        public static IList<CVar> ListVars(string prefix = null, CCommandListOptions options = CCommandListOptions.None)
        {
            return ListVars(new List<CVar>(), prefix, options);
        }

        public static IList<CVar> ListVars(IList<CVar> outList, string prefix = null, CCommandListOptions options = CCommandListOptions.None)
        {
            return ListVars(outList, delegate(CVarCommand cmd)
            {
                return ShouldListCommand(cmd, prefix, options);
            });
        }

        internal static IList<CVar> ListVars(CCommandsFilter<CVarCommand> filter)
        {
            return ListVars(new List<CVar>(), filter);
        }

        internal static IList<CVar> ListVars(IList<CVar> outList, CCommandsFilter<CVarCommand> filter)
        {
            if (filter == null)
            {
                throw new NullReferenceException("Filter is null");
            }

            foreach (CCommand cmd in m_commands)
            {
                CVarCommand varCmd = cmd as CVarCommand;
                if (varCmd != null && filter(varCmd))
                {
                    outList.Add(varCmd.cvar);
                }
            }

            return outList;
        }

        public static IList<string> ListVarNames(string prefix = null, CCommandListOptions options = CCommandListOptions.None)
        {
            IList<CVar> cvars = ListVars(new List<CVar>(), prefix, options);

            string[] names = new string[cvars.Count];

            int index = 0;
            foreach (CVar cvar in cvars)
            {
                names[index++] = cvar.Name;
            }

            return names;
        }

        internal static CVarCommand FindCvarCommand(string name)
        {
            foreach (CCommand cmd in m_commands)
            {
                CVarCommand varCmd = cmd as CVarCommand;
                if (varCmd != null && varCmd.cvar.Name == name)
                {
                    return varCmd;
                }
            }

            return null;
        }

        public static CVar FindCvar(string name)
        {
            CVarCommand cmd = FindCvarCommand(name);
            return cmd != null ? cmd.cvar : null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Aliases

        public static void AddAlias(string alias, string commandLine)
        {
            if (alias == null)
            {
                throw new ArgumentNullException("alias");
            }

            if (commandLine == null)
            {
                throw new ArgumentNullException("commandLine");
            }

            CCommand existingCmd = FindCommand(alias);
            if (existingCmd != null)
            {
                CAliasCommand cmd = existingCmd as CAliasCommand;
                if (cmd == null)
                {
                    throw new CCommandException("Can't override command with alias: " + alias);
                }

                cmd.Alias = commandLine;
            }
            else
            {
                Register(new CAliasCommand(alias, commandLine));
            }
        }

        public static bool RemoveAlias(string name)
        {
            CAliasCommand cmd = FindCommand(name) as CAliasCommand;
            return cmd != null && Unregister(cmd);
        }

        internal static IList<CAliasCommand> ListAliases(string prefix = null, CCommandListOptions options = CCommandListOptions.None)
        {
            return ListAliases(new List<CAliasCommand>(), prefix, options);
        }

        internal static IList<CAliasCommand> ListAliases(IList<CAliasCommand> outList, string prefix = null, CCommandListOptions options = CCommandListOptions.None)
        {
            foreach (CCommand cmd in m_commands)
            {
                CAliasCommand aliasCmd = cmd as CAliasCommand;
                if (aliasCmd != null && ShouldListCommand(aliasCmd, prefix, options))
                {
                    outList.Add(aliasCmd);
                }
            }

            return outList;
        }

        #endregion

        public static void GetSuggested(string token, CommandList outList)
        {
            foreach (CCommand command in m_commands)
            {
                if (command.StartsWith(token))
                {
                    outList.AddLast(command);
                }
            }
        }
    }
}
