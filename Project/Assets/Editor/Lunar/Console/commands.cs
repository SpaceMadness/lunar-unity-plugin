using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using System;
using System.Collections.Generic;
using System.Text;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    #if LUNAR_DEVELOPMENT

    [CCommand("clearprefs")]
    class Cmd_clearprefs : CCommand
    {
        void Execute()
        {
            EditorApp.Prefs.DeleteAll();
        }
    }

    [CCommand("colors")]
    class Cmd_colors : CCommand
    {
        void Execute()
        {
            string[] names = Enum.GetNames(typeof(ColorCode));
            for (int i = 0; i < names.Length; ++i)
            {
                Print("{0}: {1}", i, StringUtils.C(names[i], (ColorCode)i));
            }
        }

        bool Execute(int index, string rgb)
        {
            uint value;
            try
            {
                value = Convert.ToUInt32(rgb, 16);
            }
            catch (Exception)
            {
                PrintError("Wrong color value");
                return false; 
            }

            ColorCode[] values = (ColorCode[]) Enum.GetValues(typeof(ColorCode));
            if (index >= 0 && index < values.Length)
            {
                Color color = ColorUtils.FromRGB(value);
                EditorSkin.SetColor(values[index], color);

                Print("{0}: {1}", index, StringUtils.C(values[index].ToString(), values[index]));
            }
            else
            {
                PrintError("Wrong index");
                Execute();
            }

            return true;
        }
    }


    #endif

    [CCommand("break")]
    class Cmd_break : CPlayModeCommand
    {
        void Execute()
        {
            Editor.Break();
        }
    }

    [CCommand("clear", Description="Clears current terminal window.")]
    class Cmd_clear : CCommand
    {
        void Execute()
        {
            ClearTerminal();
        }
    }

    [CCommand("prefs")]
    class Cmd_prefs : CCommand
    {
        private static readonly string[] kSystemNames = {
            // lunar
            "com.lunarplugin.AppUniqueIdentifier",

            // unity
            "Screenmanager Resolution Height",
            "NSWindow Frame ScreenSetup",
            "Screenmanager Resolution Width",
            "Screenmanager Is Fullscreen mode",
            "UnityGraphicsQuality",
            "Screenmanager Press alt to display"
        };

        void Execute()
        {
        }

        protected override string[] AutoCompleteArgs(string commandLine, string token)
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, object> e in ListPreferences(token))
            {
                if (IsSystemName(e.Key))
                {
                    break;
                }

                list.Add(e.Key);
            }

            string[] names = list.ToArray();
            Array.Sort(names);

            for (int i = 0; i < names.Length; ++i)
            {
                names[i] = StringUtils.C(names[i], ColorCode.TableVar);
            }

            return names;
        }

        #region Helpers

        internal static IDictionary<string, object> ListPreferences(string token)
        {
            PlayerPrefsHelper.Reload();
            return PlayerPrefsHelper.ListPreferences(token);
        }

        private static bool IsSystemName(string name)
        {
            return name != null && Array.IndexOf(kSystemNames, name) != -1;
        }

        #endregion
    }
}
