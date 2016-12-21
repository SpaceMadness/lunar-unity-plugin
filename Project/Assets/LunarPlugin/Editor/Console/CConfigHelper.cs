//
//  CConfigHelper.cs
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
using UnityEditor;
using UnityEditorInternal;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    static class CConfigHelper
    {
        private static string configPath;

        public static IList<string> ListConfigs(string token = null)
        {
            List<string> result = new List<string>();

            string configsPath = ConfigPath;
            if (Directory.Exists(configsPath))
            {
                string[] files = Directory.GetFiles(configsPath, "*.cfg");
                foreach (string file in files)
                {
                    string filename = CFileUtils.GetFileName(file);
                    if (token == null || CStringUtils.StartsWithIgnoreCase(filename, token))
                    {
                        result.Add(filename);
                    }
                }
            }

            return result;
        }

        public static void WriteConfig(string filename, IList<string> lines)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            if (lines == null)
            {
                throw new ArgumentNullException("lines");
            }

            string path = GetConfigPath(filename);
            CFileUtils.Write(path, lines);
        }

        public static IList<string> ReadConfig(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            string path = GetConfigPath(filename);
            return CFileUtils.Read(path);
        }

        public static void DeleteConfigs()
        {
            CFileUtils.Delete(ConfigPath);
        }

        public static string GetConfigPath(string filename)
        {
            string path = CFileUtils.ChangeExtension(filename, ".cfg");
            if (CFileUtils.IsPathRooted(path))
            {
                return path;
            }

            return Path.Combine(ConfigPath, path);
        }

        public static string ConfigPath
        {
            get
            {
                if (configPath == null)
                {
                    configPath = ResolveConfigsPath();
                }
                return configPath;
            }
        }

        private static string ResolveConfigsPath()
        {
            try
            {
                return Path.Combine(CFileUtils.DataPath, "configs");
            }
            catch (MissingMethodException)
            {
                return Path.Combine(Path.GetTempPath(), "configs");
            }
        }
    }
}

