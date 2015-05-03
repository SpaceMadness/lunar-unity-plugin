using UnityEngine;

using System;
using System.Collections;
using System.Runtime.CompilerServices;

using LunarPluginInternal;

[assembly: InternalsVisibleTo("Assembly-CSharp")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
[assembly: InternalsVisibleTo("Test")]
[assembly: InternalsVisibleTo("LunarEditor")]

namespace LunarPlugin
{
    public static class Lunar
    {
        #region Delegate Registration

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(Delegate action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, Delegate action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<bool, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<bool, bool, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<int> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<int, int> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<int, int, int> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<float> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<float, float> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<float, float, float> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<string> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<string, string> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<string, string, string> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<Vector2> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<Vector3> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<Vector4> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<string[]> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<float[]> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<int[]> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<bool[]> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, bool, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, bool, bool, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, int> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, int, int> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, int, int, int> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, float> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, float, float> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, float, float, float> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, string> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, string, string> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, string, string, string> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, Vector2> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, Vector3> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, Vector4> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, string[]> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, float[]> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, int[]> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(CommandAction<CCommand, bool[]> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<bool, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<bool, bool, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<int> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<int, int> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<int, int, int> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<float> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<float, float> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<float, float, float> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<string> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<string, string> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<string, string, string> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<Vector2> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<Vector3> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<Vector4> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<string[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<float[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<int[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<bool[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, bool, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, bool, bool, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, int> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, int, int> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, int, int, int> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, float> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, float, float> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, float, float, float> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, string> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, string, string> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, string, string, string> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, Vector2> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, Vector3> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, Vector4> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, string[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, float[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, int[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommand(string name, CommandAction<CCommand, bool[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<bool, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<bool, bool, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<bool, bool, bool, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<int, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<int, int, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<int, int, int, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<float, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<float, float, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<float, float, float, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<string, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<string, string, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<string, string, string, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<Vector2, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<Vector3, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<Vector4, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<string[], bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<float[], bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<int[], bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<bool[], bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, bool, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, bool, bool, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, bool, bool, bool, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, int, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, int, int, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, int, int, int, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, float, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, float, float, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, float, float, float, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, string, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, string, string, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, string, string, string, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, Vector2, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, Vector3, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, Vector4, bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, string[], bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, float[], bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, int[], bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(CommandFunction<CCommand, bool[], bool> action)
        {
            CRegistery.RegisterDelegate(action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<bool, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<bool, bool, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<int> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<int, int> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<int, int, int> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<float> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<float, float> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<float, float, float> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<string> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<string, string> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<string, string, string> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<Vector2> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<Vector3> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<Vector4> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<string[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<float[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<int[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<bool[]> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, bool, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, bool, bool, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, bool, bool, bool, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, int, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, int, int, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, int, int, int, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, float, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, float, float, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, float, float, float, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, string, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, string, string, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, string, string, string, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, Vector2, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, Vector3, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, Vector4, bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, string[], bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, float[], bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, int[], bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void RegisterCommandEx(string name, CommandFunction<CCommand, bool[], bool> action)
        {
            CRegistery.RegisterDelegate(name, action);
        }

        #endregion
    }
}