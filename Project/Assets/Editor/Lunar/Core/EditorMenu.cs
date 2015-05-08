using System;

using LunarPlugin;
using LunarPluginInternal;

using UnityEngine;
using UnityEditor;

namespace LunarEditor
{
    public class EditorMenu
    {
        [MenuItem("Window/Lunar/Terminal %#t")]
        public static void ShowTerminal()
        {
            TerminalWindow.ShowWindow();
        }

        [MenuItem("Window/Lunar/Console %#&c")]
        public static void ShowConsole()
        {
            // might get released
        }

        [MenuItem("Window/Lunar/Console %#&c", true)]
        public static bool ShowConsoleCheck()
        {
            // disabled for now
            return false;
        }

        [MenuItem("Window/Lunar/")]
        private static void Separator()
        {
        }

        [MenuItem("Window/Lunar/About Lunar...")]
        private static void ShowAbout()
        {
            AboutWindow.ShowWindow();
        }
    }
}

