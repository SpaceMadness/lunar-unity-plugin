using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace LunarPluginInternal
{
    public static class PlayerPrefsHelper
    {
        private static Preferences s_prefs;

        static PlayerPrefsHelper()
        {
            if (Runtime.IsOSXEditor)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/" + "unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist";
                if (!File.Exists(path))
                {
                    Debug.LogError("Player prefs path doesn't exist: " + path);
                    return;
                }

                s_prefs = new Preferences(path);
                s_prefs.Load();
            }
        }

        internal static IDictionary<string, object> ListPreferences(string token = null)
        {
            return s_prefs.ListPreferences(token);
        }

        internal static void Reload()
        {
            s_prefs.Load();
        }
    }
}

