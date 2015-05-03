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
            #if LUNAR_DEVELOPMENT
            ConsoleWindow.ShowWindow();
            #endif
        }

        [MenuItem("Window/Lunar/Console %#&c", true)]
        public static bool ShowConsoleCheck()
        {
            #if LUNAR_DEVELOPMENT
            return true;
            #else
            return false;
            #endif
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

