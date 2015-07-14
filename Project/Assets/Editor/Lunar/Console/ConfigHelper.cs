using UnityEngine;
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
    static class ConfigHelper
    {
        public static void WriteConfig(string filename, IList<string> lines)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("Config file name is null");
            }

            if (lines == null)
            {
                throw new ArgumentNullException("Config lines is null");
            }

            string path = GetConfigPath(filename);
            FileUtils.Write(path, lines);
        }

        public static IList<string> ReadConfig(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("Config file name is null");
            }

            string path = GetConfigPath(filename);
            return FileUtils.Read(path);
        }

        public static void DeleteConfigs()
        {
            string path = ConfigPath;
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }
        }

        public static string GetConfigPath(string filename)
        {
            string path = FileUtils.ChangeExtension(filename, ".cfg");
            if (FileUtils.IsPathRooted(path))
            {
                return path;
            }

            return Path.Combine(ConfigPath, path);
        }

        public static string ConfigPath
        {
            get
            {
                try
                {
                    return Path.Combine(FileUtils.DataPath, "configs");
                }
                catch (MissingMethodException)
                {
                    return Path.Combine(Path.GetTempPath(), "configs");
                }
            }
        }
    }
}

