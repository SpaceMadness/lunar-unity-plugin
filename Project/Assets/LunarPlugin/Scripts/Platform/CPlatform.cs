//
//  Platform.cs
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
    internal static class Platform
    {
        private static readonly string EditorPlatformType = "LunarEditor.EditorPlatform";
        private static PlatformImpl s_impl;

        static Platform()
        {
            s_impl = CreateImpl();
        }

        private static PlatformImpl CreateImpl()
        {
            try
            {
                if (Application.isEditor)
                {
                    Type type = ClassUtils.TypeForName(EditorPlatformType);
                    if (type != null)
                    {
                        return ClassUtils.CreateInstance<PlatformImpl>(type);
                    }
                    else
                    {
                        Debug.LogError("Can't find " + EditorPlatformType + " type");
                    }
                }

                return new PlatformDefault();
            }
            catch (MissingMethodException) // FIXME: I don't like this
            {
                // unit test running
                Type type = ClassUtils.TypeForName("LunarPluginInternal.TestingPlatform");
                return ClassUtils.CreateInstance<PlatformImpl>(type);
            }
        }

        internal static void AssertMessage(string message, string stackTrace)
        {
            s_impl.AssertMessage(message, stackTrace);
        }
    }

    abstract class PlatformImpl
    {
        public abstract void AssertMessage(string message, string stackTrace);
    }
}

