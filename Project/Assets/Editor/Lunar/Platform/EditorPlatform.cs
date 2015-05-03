using System;

using UnityEngine;
using UnityEditor;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class EditorPlatform : PlatformImpl
    {
        public override void AssertMessage(string message, string stackTrace)
        {
            Editor.ShowDialog("Assertion", message + "\n" + stackTrace,
                new Editor.DialogButton("Ignore"),
                new Editor.DialogButton("Show", delegate()
                    {
                        Editor.OpenFileExternal(stackTrace);
                    }),
                new Editor.DialogButton("Stop", delegate()
                    {
                        if (Application.isPlaying)
                        {
                            Debug.DebugBreak();
                        }
                    })
            );
        }
    }
}

