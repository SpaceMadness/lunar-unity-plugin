using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using LunarPluginInternal;

#if LUNAR_DEVELOPMENT
[assembly: InternalsVisibleTo("Test")]
#endif

namespace LunarEditor
{
    [InitializeOnLoad]
    partial class Autorun
    {
        private static bool isPlaying;

        static Autorun()
        {
            isPlaying = EditorApplication.isPlaying;

            EditorApplication.update += EditorApp.Update;
            EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
        }

        private static void OnPlaymodeStateChanged()
        {
            if (isPlaying ^ EditorApplication.isPlaying)
            {
                isPlaying = EditorApplication.isPlaying;
                Editor.OnPlayModeChanged(isPlaying);
            }
        }
    }
}
