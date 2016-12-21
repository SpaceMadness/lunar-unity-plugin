//
//  CPlayerPrefsHelper.cs
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
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace LunarPluginInternal
{
    public static class CPlayerPrefsHelper
    {
        private static CPreferences s_prefs;

        static CPlayerPrefsHelper()
        {
            if (CRuntime.IsOSXEditor)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/" + "unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist";
                if (!File.Exists(path))
                {
                    Debug.LogError("Player prefs path doesn't exist: " + path);
                    return;
                }

                s_prefs = new CPreferences(path);
                s_prefs.Load();
            }
        }

        internal static IDictionary<string, object> ListPreferences(string token = null)
        {
            return s_prefs.ListPreferences(token);
        }

        internal static void Reload()
        {
            s_prefs.Load();
        }
    }
}

