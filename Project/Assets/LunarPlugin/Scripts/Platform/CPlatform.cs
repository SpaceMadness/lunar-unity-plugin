//
//  CPlatform.cs
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
using System.Reflection;

using UnityEngine;

using LunarPlugin;

namespace LunarPluginInternal
{
    internal static class CPlatform
    {
        private static readonly string EditorPlatformType = "LunarEditor.CEditorPlatform";
        private static CPlatformImpl s_impl;

        static CPlatform()
        {
            s_impl = CreateImpl();
        }

        private static CPlatformImpl CreateImpl()
        {
            try
            {
                if (Application.isEditor)
                {
                    Type type = CClassUtils.TypeForName(EditorPlatformType);
                    if (type != null)
                    {
                        return CClassUtils.CreateInstance<CPlatformImpl>(type);
                    }
                    else
                    {
                        Debug.LogError("Can't find " + EditorPlatformType + " type");
                    }
                }

                return new CPlatformDefault();
            }
            catch (MissingMethodException) // FIXME: I don't like this
            {
                // unit test running
                Type type = CClassUtils.TypeForName("LunarPluginInternal.CTestingPlatform");
                return CClassUtils.CreateInstance<CPlatformImpl>(type);
            }
        }

        internal static void AssertMessage(string message, string stackTrace)
        {
            s_impl.AssertMessage(message, stackTrace);
        }
    }

    abstract class CPlatformImpl
    {
        public abstract void AssertMessage(string message, string stackTrace);
    }
}

