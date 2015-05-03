using System;
using System.IO;

using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

namespace LunarEditor
{
    static partial class Editor
    {
        public static readonly string ProjectPath;

        static Editor()
        {
            try
            {
                ProjectPath = Directory.GetParent(Application.dataPath).FullName;
            }
            catch (Exception)
            {
                ProjectPath = "";
            }
        }

        internal static bool OpenFileExternal(string stackTrace)
        {
            SourcePathEntry element;
            if (UnityStackTraceParser.TryParse(stackTrace, out element))
            {
                return Editor.OpenFileAtLineExternal(element.sourcePath, element.lineNumber);
            }

            return false;
        }

        internal static bool OpenFileAtLineExternal(string sourcePath, int lineNumber)
        {
            if (sourcePath == null)
            {
                throw new NullReferenceException("Source path is null");
            }

            return InternalEditorUtility.OpenFileAtLineExternal(sourcePath, lineNumber);
        }

        internal static void OnPlayModeChanged(bool isPlaying)
        {
            UIHelper.RecycleTextures();
        }

        internal static void CopyToClipboard(string text)
        {
            EditorGUIUtility.systemCopyBuffer = text;
        }

        internal static string SaveFilePanel(string title, string directory, string defaultName, string extension)
        {
            return EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
        }

        internal static bool DeleteAsset(string path, bool logConsole = false)
        {
            bool deleted = AssetDatabase.DeleteAsset(path);
            if (!deleted && logConsole)
            {
                Debug.LogError("Unable to delete asset: " + path);
            }

            return deleted;
        }
    }
}

