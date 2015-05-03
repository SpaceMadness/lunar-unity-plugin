using System;

using UnityEditor;
using UnityEditorInternal;

namespace LunarEditor
{
    internal static class EditorConfig
    {
        public static readonly bool isBatchingMode;
        public static readonly bool isProSkin;

        static EditorConfig()
        {
            try
            {
                isBatchingMode = InternalEditorUtility.inBatchMode;
                isProSkin = EditorGUIUtility.isProSkin;
            }
            catch (Exception)
            {
                isBatchingMode = false;
                isProSkin = false;
            }
        }
    }
}

