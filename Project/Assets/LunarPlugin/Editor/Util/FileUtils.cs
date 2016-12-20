//
//  FileUtils.cs
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

﻿using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using LunarPlugin;

namespace LunarPluginInternal
{
    static class EditorFileUtils
    {
        public static bool PathExists(string path)
        {
            return path != null && Path.IsPathRooted(path) ? FileUtils.FileExists(path) : AssetPathExists(path);
        }

        public static bool AssetPathExists(string path)
        {
            string fullPath = Path.Combine(LunarEditor.Editor.ProjectPath, path);
            return FileUtils.FileExists(fullPath);
        }
    }
}