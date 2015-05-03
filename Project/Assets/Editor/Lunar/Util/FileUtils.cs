using UnityEditor;
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