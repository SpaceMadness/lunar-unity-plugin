//
//  CDelegateCommand.cs
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

﻿using UnityEngine;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using LunarPlugin;

namespace LunarPluginInternal
{
    class CDelegateCommand : CCommand
    {
        private Delegate m_actionDelegate;

        public CDelegateCommand(string name, Delegate action)
            : base(name)
        {
            if (action == null)
            {
                throw new ArgumentException("Action is null");
            }

            m_actionDelegate = action;
        }

        bool Execute(string[] args)
        {
            List<object> invokeArgs;
            int argsCount = args.Length;

            ParameterInfo[] parameters = m_actionDelegate.Method.GetParameters();
            if (parameters.Length > 0 && parameters[0].ParameterType == typeof(CCommand))
            {
                string[] trimmedArgs = new string[args.Length - 1];
                Array.Copy(args, 1, trimmedArgs, 0, trimmedArgs.Length);

                ParameterInfo[] trimmedParameters = new ParameterInfo[parameters.Length - 1];
                Array.Copy(parameters, 1, trimmedParameters, 0, trimmedParameters.Length);

                invokeArgs = CCommandUtils.ResolveInvokeParameters(trimmedParameters, trimmedArgs);
                invokeArgs.Insert(0, this);

                ++argsCount;
            }
            else
            {
                invokeArgs = CCommandUtils.ResolveInvokeParameters(m_actionDelegate.Method.GetParameters(), args);
            }

            if (!CCommandUtils.CanInvokeMethodWithArgsCount(m_actionDelegate.Method, argsCount))
            {
                PrintError("Wrong number of arguments");
                PrintUsage();
                return false;
            }

            return CCommandUtils.Invoke(m_actionDelegate, invokeArgs.ToArray());
        }

        public override void PrintUsage(bool showDescription = false)
        {
            StringBuilder buffer = new StringBuilder();

            // description
            if (showDescription && this.Description != null)
            {
                buffer.AppendFormat("  {0}\n", this.Description);
            }

            string argsUsage = GetArgsUsage();

            string name = StringUtils.C(this.Name, ColorCode.TableCommand);
            buffer.AppendFormat("  usage: {0}", name);
            buffer.Append(argsUsage);

            Print(buffer.ToString());
        }

        private string GetArgsUsage()
        {
            return CCommandUtils.GetMethodParamsUsage(m_actionDelegate.Method);
        }

        #region Properties

        public bool IsAlive
        {
            get { return true; }
        }

        public Delegate ActionDelegate
        {
            get { return m_actionDelegate; }
            set { m_actionDelegate = value; }
        }

        #endregion
    }
}