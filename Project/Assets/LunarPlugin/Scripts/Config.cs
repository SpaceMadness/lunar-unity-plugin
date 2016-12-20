//
//  Config.cs
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
using System.Collections;

namespace LunarPluginInternal
{
    static class Config
    {
        public static readonly bool isUnityFree;
        public static readonly bool isUnityPro;

        public static readonly bool isUnityBuild;
        public static bool isDebugBuild;

        static Config()
        {
            try
            {
                isDebugBuild = Debug.isDebugBuild;
                isUnityPro = Application.HasProLicense();
                isUnityFree = !isUnityPro;
                isUnityBuild = true;
            }
            catch (Exception)
            {
                isUnityFree = isUnityPro = false;
                isDebugBuild = true;
                isUnityBuild = false;
            }
        }
    }
}
