//
//  CCommandUtils.cs
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

using LunarPlugin;

namespace LunarPluginInternal
{
    static class CCommandUtils
    {
        private static readonly object[] EMPTY_INVOKE_ARGS = new object[0];

        public static bool CanInvokeMethodWithArgsCount(MethodInfo method, int argsCount)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            Type returnType = method.ReturnType;
            if (returnType != typeof(bool) && returnType != typeof(void))
            {
                return false;
            }

            ParameterInfo[] parameters = method.GetParameters();

            int normalizedArgsCount = argsCount;
            int optionalParamsCount = 0;
            for (int i = 0; i < parameters.Length; ++i)
            {
                ParameterInfo param = parameters[i];
                if (param.IsIn || param.IsOut)
                {
                    return false; // 'ref' and 'out' are not permitted
                }

                Type paramType = param.ParameterType;
                if (paramType == typeof(Vector2)) normalizedArgsCount -= 1; 
                else if (paramType == typeof(Vector3)) normalizedArgsCount -= 2;
                else if (paramType == typeof(Vector4)) normalizedArgsCount -= 3;

                if (param.IsOptional)
                {
                    ++optionalParamsCount;
                }
            }

            if (normalizedArgsCount == parameters.Length) // exact match
            {
                return true;
            }

            if (parameters.Length > 0 && parameters[parameters.Length - 1].ParameterType.IsArray) // last param is array?
            {
                if (normalizedArgsCount > parameters.Length) // more args than method can accept
                {
                    return true;
                }

                return normalizedArgsCount == parameters.Length - 1; // no args passed to the array
            }

            if (optionalParamsCount > 0 && 
                normalizedArgsCount >= parameters.Length - optionalParamsCount &&
                normalizedArgsCount <= parameters.Length)
            {
                return true;
            }

            return false;
        }

        public static bool Invoke(Delegate del, string[] invokeArgs)
        {
            if (del == null)
            {
                throw new ArgumentNullException("del");
            }

            return Invoke(del.Target, del.Method, invokeArgs);
        }

        public static bool Invoke(object target, MethodInfo method, string[] invokeArgs)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                return Invoke(target, method, EMPTY_INVOKE_ARGS);
            }

            List<object> invokeList = new List<object>(invokeArgs.Length);

            Iterator<string> iter = new Iterator<string>(invokeArgs);
            foreach (ParameterInfo param in parameters)
            {
                invokeList.Add(ResolveInvokeParameter(param, iter));
            }

            return Invoke(target, method, invokeList.ToArray());
        }

        public static bool Invoke(Delegate del, object[] invokeArgs)
        {
            if (del == null)
            {
                throw new ArgumentNullException("del");
            }

            return Invoke(del.Target, del.Method, invokeArgs);
        }

        private static bool Invoke(object target, MethodInfo method, object[] args)
        {
            if (method.ReturnType == typeof(bool))
            {
                return (bool)method.Invoke(target, args);
            }

            method.Invoke(target, args);
            return true;
        }

        public static string GetMethodParamsUsage(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < parameters.Length; ++i)
                {
                    ParameterInfo param = parameters[i];
                    if (param.ParameterType.IsArray)
                    {
                        result.Append(" ...");
                    }
                    else if (param.IsOptional)
                    {
                        result.AppendFormat(" [<{0}>]", param.Name);
                    }
                    else
                    {
                        result.AppendFormat(" <{0}>", param.Name);
                    }
                }

                return result.ToString();
            }

            return null;
        }

        internal static List<object> ResolveInvokeParameters(ParameterInfo[] parameters, string[] invokeArgs)
        {
            List<object> list = new List<object>(invokeArgs.Length);

            Iterator<string> iter = new Iterator<string>(invokeArgs);
            foreach (ParameterInfo param in parameters)
            {
                list.Add(ResolveInvokeParameter(param, iter));
            }

            return list;
        }

        private static object ResolveInvokeParameter(ParameterInfo param, Iterator<string> iter)
        {
            if (param.IsOptional && !iter.HasNext())
            {
                return param.DefaultValue;
            }

            Type type = param.ParameterType;

            if (type == typeof(string[]))
            {
                List<string> values = new List<string>();
                while (iter.HasNext())
                {
                    values.Add(NextArg(iter));
                }
                return values.ToArray();
            }

            if (type == typeof(string))
            {
                return NextArg(iter);
            }

            if (type == typeof(float))
            {
                return NextFloatArg(iter);
            }

            if (type == typeof(int))
            {
                return NextIntArg(iter);
            }

            if (type == typeof(bool))
            {
                return NextBoolArg(iter);
            }

            if (type == typeof(Vector2))
            {
                float x = NextFloatArg(iter);
                float y = NextFloatArg(iter);

                return new Vector2(x, y);
            }

            if (type == typeof(Vector3))
            {
                float x = NextFloatArg(iter);
                float y = NextFloatArg(iter);
                float z = NextFloatArg(iter);

                return new Vector3(x, y, z);
            }

            if (type == typeof(Vector4))
            {
                float x = NextFloatArg(iter);
                float y = NextFloatArg(iter);
                float z = NextFloatArg(iter);
                float w = NextFloatArg(iter);

                return new Vector4(x, y, z, w);
            }

            if (type == typeof(int[]))
            {
                List<int> values = new List<int>();
                while (iter.HasNext())
                {
                    values.Add(NextIntArg(iter));
                }
                return values.ToArray();
            }

            if (type == typeof(float[]))
            {
                List<float> values = new List<float>();
                while (iter.HasNext())
                {
                    values.Add(NextFloatArg(iter));
                }
                return values.ToArray();
            }

            if (type == typeof(bool[]))
            {
                List<bool> values = new List<bool>();
                while (iter.HasNext())
                {
                    values.Add(NextBoolArg(iter));
                }
                return values.ToArray();
            }

            throw new CCommandException("Unsupported value type: " + type);
        }

        public static int NextIntArg(Iterator<string> iter)
        {
            string arg = NextArg(iter);
            int value;

            if (int.TryParse(arg, out value))
            {
                return value;
            }

            throw new CCommandException("Can't parse int arg: '" + arg + "'"); 
        }

        public static float NextFloatArg(Iterator<string> iter)
        {
            string arg = NextArg(iter);
            float value;

            if (float.TryParse(arg, out value))
            {
                return value;
            }

            throw new CCommandException("Can't parse float arg: '" + arg + "'"); 
        }

        public static bool NextBoolArg(Iterator<string> iter)
        {
            string arg = NextArg(iter).ToLower();
            if (arg == "1" || arg == "yes" || arg == "true") return true;
            if (arg == "0" || arg == "no"  || arg == "false") return false;

            throw new CCommandException("Can't parse bool arg: '" + arg + "'"); 
        }

        public static string NextArg(Iterator<string> iter)
        {
            if (iter.HasNext())
            {
                string arg = StringUtils.UnArg(iter.Next());
                if (!IsValidArg(arg)) 
                {
                    throw new CCommandException("Invalid arg: " + arg);
                }

                return arg;
            }

            throw new CCommandException("Unexpected end of args");
        }

        public static bool IsValidArg(string arg)
        {
            // TODO: think about a better way of checking args
            // check is omitted since it messes up with operation commands (e.g. "bind x -variable")
            // return !arg.StartsWith("-") || StringUtils.IsNumeric(arg);
            return true;
        }
    }
}

