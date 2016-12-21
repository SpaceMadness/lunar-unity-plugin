//
//  CRuntimeResolver.cs
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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using UnityEngine;
using LunarPlugin;

namespace LunarPluginInternal
{
    using Option = CCommand.Option;

    public class CRuntimeResolverException : Exception
    {
        public CRuntimeResolverException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    internal static class CRuntimeResolver // TODO: remove this class
    {
        public static Result Resolve()
        {
            Result result = new Result();

            try
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        CCommand cmd;
                        if ((cmd = ResolveCommand(type)) != null)
                        {
                            result.AddCommand(cmd);
                        }
                        else if (IsCVarContainer(type))
                        {
                            result.AddContainer(type);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                StringBuilder message = new StringBuilder("Unable to resolve Lunar commands:");
                
                foreach (Exception ex in e.LoaderExceptions)
                {
                    message.AppendFormat("\n\t{0}", ex.Message);
                }
                
                throw new CRuntimeResolverException(message.ToString(), e);
            }
            catch (Exception e)
            {
                throw new CRuntimeResolverException("Unable to resolve Lunar commands", e);
            }

            return result;
        }

        private static CCommand ResolveCommand(Type type)
        {
            CCommandAttribute cmdAttr = GetCustomAttribute<CCommandAttribute>(type);
            if (cmdAttr != null)
            {
                string commandName = cmdAttr.Name;
                if (!IsCorrectPlatform(cmdAttr.Flags))
                {
                    Debug.LogWarning("Skipping command: " + commandName);
                    return null;
                }

                CCommand command = CClassUtils.CreateInstance<CCommand>(type);
                if (command != null)
                {
                    command.Name = commandName;
                    command.Description = cmdAttr.Description;
                    if (cmdAttr.Values != null)
                    {
                        command.Values = cmdAttr.Values.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    command.Flags |= cmdAttr.Flags;
                    ResolveOptions(command);

                    return command;
                }
                else
                {
                    CLog.e("Unable to register command: name={0} type={1}", commandName, type);
                }
            }

            return null;
        }

        private static bool IsCVarContainer(Type type)
        {
            return GetCustomAttribute<CVarContainerAttribute>(type) != null;
        }

        private static T GetCustomAttribute<T>(Type type) where T : Attribute
        {
            object[] attributes = type.GetCustomAttributes(typeof(T), false);
            return attributes != null && attributes.Length == 1 ? attributes[0] as T : null;
        }

        public static void ResolveOptions(CCommand command)
        {
            ResolveOptions(command, command.GetType());
        }

        public static void ResolveOptions(CCommand command, Type commandType)
        {
            FieldInfo[] fields = commandType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo info = fields[i];
                object[] attributes = info.GetCustomAttributes(typeof(CCommandOptionAttribute), true);
                if (attributes.Length == 1)
                {
                    CCommandOptionAttribute attr = (CCommandOptionAttribute)attributes[0];

                    string name = attr.Name != null ? attr.Name : info.Name;

                    Option option = new Option(info, name, attr.Description);
                    if (attr.Values != null)
                    {
                        option.Values = ParseValues(attr.Values, info.FieldType);
                    }

                    option.ShortName = attr.ShortName;
                    option.IsRequired = attr.Required;
                    option.DefaultValue = GetDefaultValue(command, info);

                    command.AddOption(option);
                }
            }
        }

        private static string[] ParseValues(string str, Type type)
        {
            string[] tokens = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; ++i)
            {
                string token = tokens[i].Trim();
                if (Option.IsValidValue(type, token))
                {
                    tokens[i] = token;
                }
                else
                {
                    throw new CCommandParseException("Invalid value '{0}' for type '{1}'", token, type);
                }
            }

            Array.Sort(tokens);

            return tokens;
        }

        private static bool IsCorrectPlatform(CCommandFlags flags)
        {
            /*
                if ((flags & CCommandFlags.IOS) != 0 && !Runtime.IsIOS)
                {
                    return false;
                }
                
                if ((flags & CCommandFlags.Android) != 0 && !Runtime.IsAndroid)
                {
                    return false;
                }

                if ((flags & CCommandFlags.Mobile) != 0 && !Runtime.IsMobile)
                {
                    return false;
                }

                if ((flags & CCommandFlags.Standalone) != 0 && !Runtime.IsStandAlone)
                {
                    return false;
                }

                if ((flags & CCommandFlags.OSX) != 0 && !Runtime.IsOSX)
                {
                    Debug.Log(4);
                    return false;
                }

                if ((flags & CCommandFlags.Windows) != 0 && !Runtime.IsWindows)
                {
                    Debug.Log(5);
                    return false;
                }

                if ((flags & CCommandFlags.Linux) != 0 && !Runtime.IsLinux)
                {
                    Debug.Log(6);
                    return false;
                }

                if ((flags & CCommandFlags.Editor) != 0)
                {
                    if ((flags & CCommandFlags.OSXEditor) != 0 && !Runtime.IsOSXEditor)
                    {
                        return false;
                    }

                    if ((flags & CCommandFlags.WindowsEditor) != 0 && !Runtime.IsWindowsEditor)
                    {
                        return false;
                    }

                    return true;
                }
                */

            return true;
        }

        private static object GetDefaultValue(CCommand command, FieldInfo info)
        {
            object value = info.GetValue(command);
            if (info.FieldType.IsValueType)
            {
                return value;
            }

            return value is ICloneable ? ((ICloneable)value).Clone() : value;
        }

        public class Result
        {
            private IList<CCommand> commands;
            private IList<Type> cvarContainersTypes;

            public Result()
            {
                commands = new List<CCommand>();
            }

            public void AddCommand(CCommand cmd)
            {
                commands.Add(cmd);
            }

            public void AddContainer(Type type)
            {
                if (cvarContainersTypes == null)
                    cvarContainersTypes = new List<Type>();

                cvarContainersTypes.Add(type);
            }

            public IList<CCommand> Commands
            {
                get { return commands; }
            }

            public IList<Type> CvarContainersTypes
            {
                get { return cvarContainersTypes; }
            }
        }
    }
}

