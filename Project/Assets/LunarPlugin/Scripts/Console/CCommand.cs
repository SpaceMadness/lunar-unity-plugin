//
//  CCommand.cs
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
using System.Text;

using UnityEngine;

using LunarPluginInternal;

namespace LunarPlugin
{
    delegate bool ListOptionsFilter(CCommand.Option opt);

    public enum CCommandFlags
    {
        None         = 0,
        Debug        = 1 << 0,
        Hidden       = 1 << 1,
        System       = 1 << 2,
        PlayModeOnly = 1 << 3,
    }

    interface ICCommandDelegate // FIXME: remove the delegate
    {
        void LogTerminal(string message);
        void LogTerminal(string[] table);
        void LogTerminal(Exception e, string message);

        void ClearTerminal();

        bool ExecuteCommandLine(string commandLine, bool manual = false);
        void PostNotification(CCommand cmd, string name, params object[] data);

        bool IsPromptEnabled { get; }
    }

    internal class NullCommandDelegate : ICCommandDelegate
    {
        public static readonly NullCommandDelegate Instance = new NullCommandDelegate();

        private NullCommandDelegate()
        {
        }

        public void LogTerminal(string message)
        {
        }

        public void LogTerminal(string[] table)
        {
        }

        public void LogTerminal(Exception e, string message)
        {
        }

        public void ClearTerminal()
        {
        }

        public bool ExecuteCommandLine(string commandLine, bool manual = false)
        {
            return false;
        }

        public void PostNotification(CCommand cmd, string name, params object[] data)
        {
        }

        public bool IsPromptEnabled
        {
            get { return false; }
        }
    }

    public abstract class CCommand : IComparable<CCommand>
    {
        private static readonly string[] EMPTY_COMMAND_ARGS = new string[0];

        private Dictionary<string, Option> m_optionsLookup;

        private MethodInfo[] executeMethods;

        private List<Option> m_options;
        private string[] m_values;

        private ICCommandDelegate m_delegate;

        public CCommand()
        {
            this.Delegate = null;
        }

        public CCommand(string name)
            : this()
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.Name = name;
        }

        internal static void ResolveOptions(CCommand command)
        {
            RuntimeResolver.ResolveOptions(command);
        }

        internal static void ResolveOptions(CCommand command, Type commandType)
        {
            RuntimeResolver.ResolveOptions(command, commandType);
        }

        internal bool ExecuteTokens(IList<string> tokens, string commandLine = null)
        {
            try
            {
                return ExecuteGuarded(tokens, commandLine);
            }
            catch (CCommandException e)
            {
                PrintError(e.Message);
            }
            catch (TargetInvocationException e)
            {
                PrintError(e.InnerException, "Error while executing command");
            }
            catch (Exception e)
            {
                PrintError(e, "Error while executing command");
            }

            return false;
        }

        private bool ExecuteGuarded(IList<string> tokens, string commandLine = null)
        {
            ResetOptions();
            
            Iterator<string> iter = new Iterator<string>(tokens);
            iter.Next(); // first token is a command name

            if (this.IsManualMode)
            {
                PrintPrompt(commandLine);
            }

            if (this.IsPlayModeOnly && !Runtime.IsPlaying)
            {
                PrintError("Command is available in the play mode only");
                return false;
            }

            ReusableList<string> argsList = ReusableLists.NextAutoRecycleList<string>();
            while (iter.HasNext())
            {
                string token = StringUtils.UnArg(iter.Next());

                // first, try to parse options
                if (!TryParseOption(iter, token))
                {
                    // consume the rest of the args
                    argsList.Add(token);
                    while (iter.HasNext())
                    {
                        token = StringUtils.UnArg(iter.Next());
                        argsList.Add(token);
                    }

                    break;
                }
            }

            if (m_values != null)
            {
                if (argsList.Count != 1)
                {
                    PrintError("Unexpected arguments count {0}", argsList.Count);
                    PrintUsage();
                    return false;
                }

                string arg = argsList[0];
                if (Array.IndexOf(m_values, arg) == -1)
                {
                    PrintError("Unexpected argument '{0}'", arg);
                    PrintUsage();
                    return false;
                }
            }

            if (m_options != null)
            {
                for (int i = 0; i < m_options.Count; ++i)
                {
                    Option opt = m_options[i];
                    if (opt.IsRequired && !opt.IsHandled)
                    {
                        PrintError("Missing required option --{0}{1}", opt.Name, opt.ShortName != null ? "(-" + opt.ShortName + ")" : "");
                        PrintUsage();
                        return false;
                    }
                }
            }

            string[] args = argsList.Count > 0 ? argsList.ToArray() : EMPTY_COMMAND_ARGS;
            MethodInfo executeMethod = resolveExecuteMethod(args);
            if (executeMethod == null)
            {
                PrintError("Wrong arguments");
                PrintUsage();
                return false;
            }

            return CCommandUtils.Invoke(this, executeMethod, args);
        }

        private MethodInfo resolveExecuteMethod(String[] args)
        {
            if (executeMethods == null)
            {
                executeMethods = resolveExecuteMethods();
            }
            
            MethodInfo method = null;
            foreach (MethodInfo m in executeMethods)
            {
                if (CCommandUtils.CanInvokeMethodWithArgsCount(m, args.Length))
                {
                    if (method != null) // multiple methods found
                    {
                        return null;
                    }
                    
                    method = m;
                }
            }
            
            return method;
        }
        
        private MethodInfo[] resolveExecuteMethods()
        {
            List<MethodInfo> result = new List<MethodInfo>();

            ListMethodsFilter filter = delegate(MethodInfo method)
            {
                if (method.Name != "Execute")
                {
                    return false;
                }
                
                if (method.IsAbstract)
                {
                    return false;
                }

                return method.ReturnType == typeof(void) || method.ReturnType == typeof(bool);
            };
            
            Type type = GetCommandType();
            while (type != null)
            {
                ClassUtils.ListInstanceMethods(result, type, filter);
                type = type.BaseType;
            }
            
            return result.ToArray();
        }

        private bool TryParseOption(Iterator<string> iter, string token)
        {
            if (this.IsIgnoreOptions)
            {
                return false;
            }

            // first, try to parse options
            if (token.StartsWith("--"))
            {
                string optionName = token.Substring(2);
                ParseOption(iter, optionName);
                return true;
            }

            if (token.StartsWith("-") && !StringUtils.IsNumeric(token))
            {
                string optionName = token.Substring(1);
                ParseOption(iter, optionName);
                return true;
            }

            return false;
        }

        private Option ParseOption(Iterator<string> iter, string name)
        {
            Option option = FindOption(name);
            if (option != null)
            {
                ParseOption(iter, option);
                return option;
            }
            else
            {
                throw new CCommandException("Can't find option: " + name);
            }
        }

        private void ParseOption(Iterator<string> iter, Option opt) // FIXME: fix '-' and '--' args
        {
            Type type = opt.Target.FieldType;
            opt.IsHandled = true;

            if (type == typeof(int))
            {
                object value = CCommandUtils.NextIntArg(iter);
                CheckValue(opt, value);
                opt.Target.SetValue(this, value);
            }
            else if (type == typeof(bool))
            {
                opt.Target.SetValue(this, true);
            }
            else if (type == typeof(float))
            {
                object value = CCommandUtils.NextFloatArg(iter);
                CheckValue(opt, value);
                opt.Target.SetValue(this, value);
            }
            else if (type == typeof(string))
            {
                string value = CCommandUtils.NextArg(iter);
                CheckValue(opt, value);
                opt.Target.SetValue(this, value);
            }
            else if (type == typeof(string[]))
            {
                string[] arr = (string[]) opt.Target.GetValue(this);
                if (arr == null)
                {
                    throw new CCommandException("Field should be initialized: " + opt.Target.Name);
                }

                for (int i = 0; i < arr.Length; ++i)
                {
                    arr[i] = CCommandUtils.NextArg(iter);
                }
            }
            else if (type == typeof(int[]))
            {
                int[] arr = (int[]) opt.Target.GetValue(this);
                if (arr == null)
                {
                    throw new CCommandException("Field should be initialized: " + opt.Target.Name);
                }
                
                for (int i = 0; i < arr.Length; ++i)
                {
                    arr[i] = CCommandUtils.NextIntArg(iter);
                }
            }
            else if (type == typeof(float[]))
            {
                float[] arr = (float[]) opt.Target.GetValue(this);
                if (arr == null)
                {
                    throw new CCommandException("Field should be initialized: " + opt.Target.Name);
                }
                
                for (int i = 0; i < arr.Length; ++i)
                {
                    arr[i] = CCommandUtils.NextFloatArg(iter);
                }
            }
            else if (type == typeof(bool[]))
            {
                bool[] arr = (bool[]) opt.Target.GetValue(this);
                if (arr == null)
                {
                    throw new CCommandException("Field should be initialized: " + opt.Target.Name);
                }
                
                for (int i = 0; i < arr.Length; ++i)
                {
                    arr[i] = CCommandUtils.NextBoolArg(iter);
                }
            }
            else
            {
                throw new CCommandException("Unsupported field type: " + type);
            }
        }

        private bool SkipOption(Iterator<string> iter, string name)
        {
            Option option = FindOption(name);
            return option != null && SkipOption(iter, option);
        }

        private bool SkipOption(Iterator<string> iter, Option opt)
        {
            Type type = opt.Target.FieldType;

            if (type.IsArray)
            {
                Array arr = (Array) opt.Target.GetValue(this);
                if (arr != null)
                {
                    int length = arr.Length;
                    Type elementType = arr.GetType().GetElementType();

                    int index = 0;
                    if (iter.HasNext() && index++ < length)
                    {
                        string value = iter.Next();
                        if (!Option.IsValidValue(elementType, value))
                        {
                            if (!iter.HasNext()) // this was the last arg
                            {
                                return false;
                            }

                            throw new CCommandParseException("'{0}' is an invalid value for the array option '{1}'", value, opt.Name);
                        }
                    }

                    return index == length;
                }

                return false;
            }

            if (type == typeof(int) || 
                type == typeof(float) || 
                type == typeof(string))
            {
                if (iter.HasNext())
                {
                    string value = iter.Next();
                    if (!opt.IsValidValue(value))
                    {
                        if (!iter.HasNext()) // this was the last arg
                        {
                            return false;
                        }

                        throw new CCommandParseException("'{0}' is an invalid value for the option '{1}'", value, opt.Name);
                    }

                    return true;
                }

                return false;
            }

            if (type == typeof(bool))
            {
                return true;
            }

            return false;
        }

        private void CheckValue(Option opt, object value)
        {
            if (opt.Values != null)
            {
                foreach (object v in opt.Values)
                {
                    if (v.Equals(value))
                    {
                        return;
                    }
                }

                string optionDesc = opt.ShortName != null ? 
                    StringUtils.TryFormat("--{0}(-{1})", opt.Name, opt.ShortName) : 
                    StringUtils.TryFormat("--{0}", opt.Name);

                StringBuilder buffer = new StringBuilder();
                buffer.AppendFormat("Invalid value '{0}' for option {1}\n", value, optionDesc);
                buffer.Append("Valid values:");
                foreach (object v in opt.Values)
                {
                    buffer.Append("\n• ");
                    buffer.Append(v);
                }

                throw new CCommandException(buffer.ToString());
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        internal void AddOption(Option opt)
        {
            if (m_optionsLookup == null) 
            {
                m_optionsLookup = new Dictionary<string, Option>();
                m_options = new List<Option>();
            }

            string name = opt.Name;
            string shortName = opt.ShortName;

            if (m_optionsLookup.ContainsKey(name)) 
            {
                Log.e("Option already registered: {0}", name);
                return;
            }

            if (shortName != null && m_optionsLookup.ContainsKey(name)) 
            {
                Log.e("Short option already registered: {0}", shortName);
                return;
            }

            m_optionsLookup[name] = opt;
            m_options.Add(opt);

            if (shortName != null)
            {
                m_optionsLookup[shortName] = opt;
            }
        }

        internal IList<Option> ListShortOptions(string prefix = null)
        {
            return ListShortOptions(ReusableLists.NextAutoRecycleList<Option>(), prefix);
        }

        internal IList<Option> ListShortOptions(IList<Option> outList, string prefix = null)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                return ListOptions(outList, delegate(Option opt) {
                    return StringUtils.StartsWithIgnoreCase(opt.ShortName, prefix);
                });
            }

            return ListOptions(outList, DefaultListShortOptionsFilter);
        }

        internal IList<Option> ListOptions(string prefix = null)
        {
            return ListOptions(ReusableLists.NextAutoRecycleList<Option>(), prefix);
        }

        internal IList<Option> ListOptions(IList<Option> outList, string prefix = null)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                return ListOptions(outList, delegate(Option opt) {
                    return StringUtils.StartsWithIgnoreCase(opt.Name, prefix);
                });
            }
            return ListOptions(outList, DefaultListOptionsFilter);
        }

        internal IList<Option> ListOptions(IList<Option> outList, ListOptionsFilter filter)
        {
            if (filter == null)
            {
                throw new NullReferenceException("Filter is null");
            }

            if (m_optionsLookup != null)
            {
                foreach (Option opt in m_options)
                {
                    if (filter(opt))
                    {
                        outList.Add(opt);
                    }
                }
            }

            return outList;
        }

        private static bool DefaultListOptionsFilter(Option opt)
        {
            return true;
        }

        private static bool DefaultListShortOptionsFilter(Option opt)
        {
            return opt.ShortName != null;
        }

        internal Option FindOption(string name)
        {
            if (name.Length == 0)
            {
                return null;
            }

            Option option;
            if (m_optionsLookup != null)
            {
                if (m_optionsLookup.TryGetValue(name, out option))
                {
                    return option;
                }
            }

            return null;
        }

        internal Option FindNonAmbiguousOption(string name, bool useShort)
        {
            if (name.Length == 0)
            {
                return null;
            }
            
            if (m_options != null)
            {
                Option targetOpt = null;
                foreach (Option opt in m_options)
                {
                    String optName = useShort ? opt.ShortName : opt.Name;
                    if (optName == null)
                    {
                        continue;
                    }
                    
                    if (targetOpt == null)
                    {
                        if (StringUtils.EqualsIgnoreCase(optName, name))
                        {
                            targetOpt = opt;
                        }
                    }
                    else if (StringUtils.StartsWithIgnoreCase(optName, name))
                    {
                        return null;
                    }
                }
                
                return targetOpt;
            }
            
            return null;
        }

        private void ResetOptions()
        {
            if (m_options != null)
            {
                for (int i = 0; i < m_options.Count; ++i)
                {
                    Option opt = m_options[i];
                    if (opt.IsHandled)
                    {
                        ResetOption(opt);
                    }
                }
            }
        }

        private void ResetOption(Option opt)
        {
            Type type = opt.Target.FieldType;
            if (type.IsArray)
            {
                Array source = (Array)opt.DefaultValue;
                Array target = (Array)opt.Target.GetValue(this);
                Array.Copy(source, target, source.Length);
            }
            else
            {
                opt.Target.SetValue(this, opt.DefaultValue);
            }
            opt.IsHandled = false;
        }

        //////////////////////////////////////////////////////////////////////////////

        internal IList<string> AutoCompleteCustomArgs(string commandLine, string token)
        {
            try
            {
                return AutoCompleteArgs(commandLine, token);
            }
            catch (Exception e)
            {
                throw new CommandAutoCompleteException(e);
            }
        }

        protected virtual IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return null;
        }

        internal void Clear()
        {
            this.Delegate = null;
            this.Args = null;
            this.CommandString = null;
            this.IsManualMode = false;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Output

        internal static string Prompt(string commandLine)
        {
            return "> " + commandLine;
        }

        internal void PrintPrompt(string commandLine)
        {
            if (Delegate.IsPromptEnabled)
            {
                Print(Prompt(commandLine));
            }
        }

        /// <summary>
        /// Print the specified message.
        /// </summary>
        protected void Print(string message)
        {
            m_delegate.LogTerminal(message);
        }

        /// <summary>
        /// Print the specified formatted message.
        /// </summary>
        protected void Print(string format, params object[] args)
        {
            m_delegate.LogTerminal(StringUtils.TryFormat(format, args));
        }

        /// <summary>
        /// Pretty print the table of strings.
        /// 
        /// Example:
        /// alias            cvar_restart     reset
        /// aliaslist        cvarlist         tag
        /// bind             echo             test
        /// bindlist         exec             throw_exception
        /// break            exit             timeScale
        /// cat              log              toggle
        /// clear            man              unalias
        /// clearprefs       menu             unbind
        /// cmdlist          next             unbindall
        /// colors           prefs            writeconfig
        /// </summary>
        protected void Print(string[] table)
        {
            if (table != null && table.Length > 0)
            {
                m_delegate.LogTerminal(table);
            }
        }

        /// <summary>
        /// Prints message with indent.
        /// </summary>
        protected void PrintIndent(string message)
        {
            m_delegate.LogTerminal("  " + message);
        }

        /// <summary>
        /// Prints formatted message with indent.
        /// </summary>
        protected void PrintIndent(string format, params object[] args)
        {
            m_delegate.LogTerminal("  " + StringUtils.TryFormat(format, args));
        }

        /// <summary>
        /// Prints error message.
        /// </summary>
        protected void PrintError(string format, params object[] args)
        {
            PrintIndent(C(StringUtils.TryFormat(format, args), ColorCode.Error));
        }

        /// <summary>
        /// Prints exceptions.
        /// </summary>
        protected void PrintError(Exception e)
        {
            PrintError(e, null);
        }

        /// <summary>
        /// Prints exceptions.
        /// </summary>
        protected void PrintError(Exception e, string format, params object[] args)
        {
            PrintError(e, StringUtils.TryFormat(format, args));
        }

        /// <summary>
        /// Prints exceptions.
        /// </summary>
        protected void PrintError(Exception e, string message)
        {
            m_delegate.LogTerminal(e, message);
        }

        /// <summary>
        /// Prints command's usage.
        /// </summary>
        public virtual void PrintUsage(bool showDescription = false)
        {
            StringBuilder buffer = new StringBuilder();

            // description
            if (showDescription && this.Description != null)
            {
                buffer.AppendFormat("  {0}\n", this.Description);
            }

            string optionsUsage = GetOptionsUsage(m_options);
            string[] argsUsages = GetArgsUsages();

            // name
            if (argsUsages != null && argsUsages.Length > 0)
            {
                string name = StringUtils.C(this.Name, ColorCode.TableCommand);

                // first usage line
                buffer.AppendFormat("  usage: {0}", name);
                if (!string.IsNullOrEmpty(optionsUsage))
                {
                    buffer.Append(optionsUsage);
                }
                buffer.Append(argsUsages[0]);

                // optional usage lines
                for (int i = 1; i < argsUsages.Length; ++i)
                {
                    buffer.AppendFormat("\n         {0}", name);
                    if (!string.IsNullOrEmpty(optionsUsage))
                    {
                        buffer.Append(optionsUsage);
                    }
                    buffer.Append(argsUsages[i]);
                }
            }
            else
            {
                buffer.Append(StringUtils.C("'Execute' method is not resolved", ColorCode.Error)); 
            }

            Print(buffer.ToString());
        }

        private string GetOptionsUsage(List<Option> options)
        {
            if (options != null && options.Count > 0)
            {
                StringBuilder buffer = new StringBuilder();
                for (int i = 0; i < options.Count; ++i)
                {
                    Option opt = options[i];

                    buffer.Append(' ');

                    if (!opt.IsRequired)
                    {
                        buffer.Append('[');
                    }

                    if (opt.ShortName != null)
                    {
                        buffer.AppendFormat("-{0}|", StringUtils.C(opt.ShortName, ColorCode.TableVar));
                    }

                    buffer.AppendFormat("--{0}", StringUtils.C(opt.Name, ColorCode.TableVar));

                    if (opt.Type != typeof(bool))
                    {
                        if (opt.HasValues())
                        {
                            string[] values = opt.Values;
                            buffer.Append(" <");
                            for (int valueIndex = 0; valueIndex < values.Length; ++valueIndex)
                            {
                                buffer.Append(StringUtils.Arg(values[valueIndex]));
                                if (valueIndex < values.Length - 1)
                                {
                                    buffer.Append("|");
                                }
                            }
                            buffer.Append('>');
                        }
                        else
                        {
                            buffer.AppendFormat(" <{0}>", UsageOptionName(opt));
                        }
                    }

                    if (!opt.IsRequired)
                    {
                        buffer.Append(']');
                    }
                }

                return buffer.ToString();
            }

            return null;
        }

        private string UsageOptionName(Option opt)
        {
            try
            {
                return GetUsageOptionName(opt);
            }
            catch
            {
                return "#err";
            }
        }

        private string GetUsageOptionName(Option opt)
        {
            Type type = opt.Type;
            if (type != null && type.IsArray)
            {
                Array arr = (Array) opt.Target.GetValue(this);
                if (arr != null)
                {
                    int length = arr.Length;
                    string elementTypeName = ClassUtils.TypeShortName(arr.GetType().GetElementType());

                    StringBuilder result = new StringBuilder();
                    for (int i = 0; i < length; ++i)
                    {
                        result.Append(elementTypeName);
                        if (i < length - 1)
                        {
                            result.Append(',');
                        }
                    }
                    return result.ToString();
                }
            }

            string typename = ClassUtils.TypeShortName(type);
            return typename != null ? typename : "opt";
        }

        private string[] GetArgsUsages()
        {
            try
            {
                return GetUsageArgs();
            }
            catch
            {
                return new string[] { "#err" };
            }
        }

        protected virtual String[] GetUsageArgs()
        {
            if (m_values != null && m_values.Length > 0)
            {
                return new String[] { " " + StringUtils.Join(m_values, "|") };
            }

            if (executeMethods == null)
            {
                executeMethods = resolveExecuteMethods();
            }

            if (executeMethods.Length > 0)
            {
                String[] result = new String[executeMethods.Length];
                for (int i = 0; i < result.Length; ++i)
                {
                    result[i] = CCommandUtils.GetMethodParamsUsage(executeMethods[i]);
                }
                return result;
            }

            return null;
        }

        internal void ClearTerminal()
        {
            m_delegate.ClearTerminal();
        }

        protected bool ExecCommand(string commandLine, bool manualMode = false)
        {
            return m_delegate.ExecuteCommandLine(commandLine, manualMode);
        }

        internal void PostNotification(string name, params object[] data)
        {
            m_delegate.PostNotification(this, name, data);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        public bool HasFlag(CCommandFlags flag)
        {
            return (Flags & flag) != 0;
        }

        public void SetFlag(CCommandFlags flag, bool value)
        {
            if (value)
            {
                Flags |= flag;
            }
            else
            {
                Flags &= ~flag;
            }
        }

        internal bool StartsWith(string prefix)
        {
            return StringUtils.StartsWithIgnoreCase(Name, prefix);
        }

        internal static string Arg(string value) { return StringUtils.Arg(value); }

        internal static string C(string str, ColorCode color) { return StringUtils.C(str, color); }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public CCommand ParentCommand
        {
            set
            {
                if (value != null)
                {
                    this.IsManualMode = value.IsManualMode;
                    this.Delegate = value.Delegate;
                }
                else
                {
                    this.IsManualMode = false;
                    this.Delegate = null;
                }
            }
        }

        public string Name { get; internal protected set; }
        public string Description { get; set; }

        private Type GetCommandType()
        {
            return GetType();
        }

        public string[] Values
        {
            get { return m_values; }
            set { m_values = value; }
        }

        internal ICCommandDelegate Delegate
        { 
            get { return m_delegate; } 
            set { m_delegate = value != null ? value : NullCommandDelegate.Instance; }
        }

        internal List<string> Args { get; set; }

        public string CommandString { get; internal set; } // TODO: rename to CommandLine
        public CCommandFlags Flags { get; set; }

        public bool IsHidden
        {
            get { return HasFlag(CCommandFlags.Hidden); }
            set { SetFlag(CCommandFlags.Hidden, value); }
        }

        public bool IsSystem
        {
            get { return HasFlag(CCommandFlags.System); }
            set { SetFlag(CCommandFlags.System, value); }
        }

        public bool IsDebug
        {
            get { return HasFlag(CCommandFlags.Debug); }
            set { SetFlag(CCommandFlags.Debug, value); }
        }

        public bool IsPlayModeOnly
        {
            get { return HasFlag(CCommandFlags.PlayModeOnly); }
            set { SetFlag(CCommandFlags.PlayModeOnly, value); }
        }

        internal bool IsManualMode { get; set; }

        internal static CommandListOptions DefaultListOptions
        {
            get
            {
                CommandListOptions options = CommandListOptions.None;
                if (Config.isDebugBuild)
                {
                    options |= CommandListOptions.Debug;
                }

                return options;
            }
        }

        protected bool IsIgnoreOptions { get; set; }

        internal ColorCode ColorCode
        {
            get { return IsPlayModeOnly && !Runtime.IsPlaying ? ColorCode.TableCommandDisabled : ColorCode.TableCommand; }
        }

        #endregion

        #region IComparable

        public int CompareTo(CCommand other)
        {
            return Name.CompareTo(other.Name);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        internal class Option
        {
            public Option(FieldInfo target, string name, string description)
            {
                Target = target;
                Name = name;
                Description = description;
            }

            public bool IsValidValue(string value)
            {
                if (HasValues())
                {
                    return Array.IndexOf(Values, value) != -1;
                }

                return IsValidValue(Target.FieldType, value);
            }

            internal static bool IsValidValue(Type type, string value)
            {
                if (type == typeof(string))
                {
                    if (value.StartsWith("--")) // can't be long option
                    {
                        return false;
                    }
                    if (value.StartsWith("-")) // can't be short option
                    {
                        return StringUtils.IsNumeric(value); // but can be a negative number
                    }
                    if (value.Equals("&&") || value.Equals("||"))
                    {
                        return false;
                    }
                    
                    return value.Length > 0; // can't be empty
                }

                if (type == typeof(int))
                {
                    return StringUtils.IsInteger(value);
                }

                if (type == typeof(float))
                {
                    return StringUtils.IsNumeric(value);
                }

                return false;
            }

            public bool HasValues()
            {
                return Values != null && Values.Length > 0;
            }

            public override string ToString()
            {
                StringBuilder result = new StringBuilder();
                result.Append(Target.GetType().Name);
                result.Append(" ");
                result.Append(Name);
                if (ShortName != null)
                {
                    result.Append("(");
                    result.Append(ShortName);
                    result.Append(")");
                }
                
                if (DefaultValue != null)
                {
                    result.Append(" = ");
                    result.Append(DefaultValue.ToString());
                }
                
                return result.ToString();
            }

            //////////////////////////////////////////////////////////////////////////////

            #region Properties

            public FieldInfo Target { get; private set; }

            public string Name { get; protected internal set; }
            public string Description { get; protected internal set; }
            public Type Type { get { return Target != null ? Target.FieldType : null; } }
            public object DefaultValue { get; set; }

            public string ShortName { get; set; }
            public bool IsRequired { get; set; }
            public bool IsHandled { get; set; }

            public string[] Values { get; set; }

            public string[] ListValues(string token = null)
            {
                if (HasValues())
                {
                    List<string> list = new List<string>(Values.Length);
                    for (int i = 0; i < Values.Length; ++i)
                    {
                        string str = Values[i];
                        if (token == null || StringUtils.StartsWithIgnoreCase(str, token))
                        {
                            list.Add(str);
                        }
                    }

                    return list.ToArray();
                }

                return new string[0];
            }

            #endregion
        }
    }

    public abstract class CPlayModeCommand : CCommand
    {
        public CPlayModeCommand()
        {
            this.Flags = CCommandFlags.PlayModeOnly;
        }
    }

    public abstract class CDebugCommand : CCommand
    {
        public CDebugCommand()
        {
            this.Flags = CCommandFlags.Debug;
        }
    }

    public abstract class CHiddenCommand : CCommand
    {
        public CHiddenCommand()
        {
            this.Flags = CCommandFlags.Hidden;
        }
    }

    class CCommandException : Exception
    {
        public CCommandException(string message)
            : base(message)
        {
        }

        public CCommandException(string format, params object[] args)
            : this(StringUtils.TryFormat(format, args))
        {
        }
    }

    class CCommandParseException : Exception
    {
        public CCommandParseException(string message)
            : base(message)
        {
        }

        public CCommandParseException(string format, params object[] args)
            : this(StringUtils.TryFormat(format, args))
        {
        }
    }
}
