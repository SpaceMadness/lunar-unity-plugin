//
//  CBindings.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

﻿using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;

namespace LunarPluginInternal
{
    enum CModifiers
    {
        Shift   = 1 << 0,
        Control = 1 << 1,
        Alt     = 1 << 2,
        Command = 1 << 3
    };

    struct CShortCut
    {
        public readonly KeyCode key;
        public readonly CModifiers modifiers;

        public CShortCut(KeyCode key, CModifiers modifiers)
        {
            this.key = key;
            this.modifiers = modifiers;
        }

        public static bool TryParse(string token, out CShortCut shortCut)
        {
            string[] tokens = token.Split('+');

            CModifiers modifiers = 0;
            if (tokens.Length > 1)
            {
                for (int i = 0; i < tokens.Length - 1; ++i)
                {
                    string name = tokens[i].ToLower();
                    if (name == "ctrl")
                        modifiers |= CModifiers.Control;
                    else if (name == "shift")
                        modifiers |= CModifiers.Shift;
                    else if (name == "alt")
                        modifiers |= CModifiers.Alt;
                    else if (name == "cmd" || name == "command")
                        modifiers |= CModifiers.Command;
                    else
                    {
                        shortCut = default(CShortCut);
                        return false;
                    }
                }
            }

            string keyName = tokens[tokens.Length - 1].ToLower();
            KeyCode key;
            if (!CBindings.TryParse(keyName, out key))
            {
                shortCut = default(CShortCut);
                return false;
            }

            shortCut = new CShortCut(key, modifiers);
            return true;
        }

        public bool Equals(CShortCut other)
        {
            return key == other.key && modifiers == other.modifiers;
        }

        public bool HasModifier(CModifiers modifier)
        {
            return (modifiers & modifier) != 0;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            if (this.IsCommand) result.Append("cmd+");
            if (this.IsControl) result.Append("ctrl+");
            if (this.IsAlt) result.Append("alt+");
            if (this.IsShift) result.Append("shift+");

            result.Append(CBindings.KeyName(this.key));

            return result.ToString();
        }

        #region Properties

        public bool IsControl
        {
            get { return HasModifier(CModifiers.Control); }
        }

        public bool IsAlt
        {
            get { return HasModifier(CModifiers.Alt); }
        }

        public bool IsShift
        {
            get { return HasModifier(CModifiers.Shift); }
        }

        public bool IsCommand
        {
            get { return HasModifier(CModifiers.Command); }
        }

        public bool HasModifiers
        {
            get { return modifiers != 0; }
        }

        #endregion
    }

    struct CBinding
    {
        private const int kShiftModifier   = 1 << 0;
        private const int kControlModifier = 1 << 1;
        private const int kAltModifier     = 1 << 2;
        private const int kCommandModifier = 1 << 3;

        private CShortCut m_shortCut;
        private string m_cmdKeyDown;
        private string m_cmdKeyUp;

        public CBinding(CShortCut shortCut, string cmdKeyUp, string cmdKeyDown)
        {
            m_shortCut = shortCut;
            m_cmdKeyDown = cmdKeyUp;
            m_cmdKeyUp = cmdKeyDown;
        }

        #region Modifiers

        public bool HasModifiers
        {
            get { return m_shortCut.HasModifiers; }
        }

        public bool IsCommand
        {
            get { return m_shortCut.HasModifier(CModifiers.Command); }
        }

        #endregion

        #region Properties

        public CShortCut shortCut
        {
            get { return m_shortCut; }
        }

        public KeyCode key
        {
            get { return m_shortCut.key; }
        }

        public string cmdKeyDown
        {
            get { return m_cmdKeyDown; }
            set { m_cmdKeyDown = value; }
        }

        public string cmdKeyUp
        {
            get { return m_cmdKeyUp; }
            set { m_cmdKeyUp = value; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} {1}", key, cmdKeyDown);
        }
    }

    delegate bool CCommandBindingsFilter(CBinding binding);

    class CBindings
    {
        private static List<CBinding> m_bindings = new List<CBinding>();

        private static IDictionary<string, KeyCode> s_keyCodeLookup;
        private static IDictionary<KeyCode, string> s_keyNameLookup;
        private static string[] s_names;

        public static void Bind(CShortCut shortCut, string cmdKeyDown, string cmdKeyUp)
        {
            if (cmdKeyDown == null)
            {
                throw new NullReferenceException("Command is null");
            }
             
            int index = IndexOf(shortCut);
            if (index != -1)
            {
                CBinding existing = m_bindings[index];
                existing.cmdKeyDown = cmdKeyDown;
                existing.cmdKeyUp = cmdKeyUp;
                m_bindings[index] = existing;
            }
            else
            {
                m_bindings.Add(new CBinding(shortCut, cmdKeyDown, cmdKeyUp));
            }
        }

        public static bool Unbind(CShortCut shortCut)
        {
            int index = IndexOf(shortCut);
            if (index != -1)
            {
                m_bindings.RemoveAt(index);
                return true;
            }

            return false;
        }

        public static IList<CBinding> List(string prefix = null) // TODO: unit tests
        {
            if (prefix != null)
            {
                return List(delegate(CBinding binding) {
                    return CStringUtils.StartsWithIgnoreCase(binding.shortCut.ToString(), prefix);
                });
            }

            return m_bindings;
        }

        public static IList<CBinding> List(CCommandBindingsFilter filter)
        {
            if (filter == null)
            {
                throw new NullReferenceException("Filter is null");
            }

            IList<CBinding> list = new List<CBinding>();
            for (int i = 0; i < m_bindings.Count; ++i)
            {
                if (filter(m_bindings[i]))
                {
                    list.Add(m_bindings[i]);
                }
            }

            return list;
        }

        public static IList<string> ListShortCuts(string prefix = null) // TODO: unit tests
        {
            IList<CBinding> bindings = CBindings.List(prefix);
            string[] shortcuts = new string[bindings.Count];

            int index = 0;
            foreach (CBinding b in bindings)
            {
                shortcuts[index++] = b.shortCut.ToString();
            }

            return shortcuts;
        }

        public static bool FindBinding(CShortCut shortCut, out CBinding result)
        {
            for (int i = 0; i < m_bindings.Count; ++i)
            {
                if (m_bindings[i].shortCut.Equals(shortCut))
                {
                    result = m_bindings[i];
                    return true;
                }
            }

            result = default(CBinding);
            return false;
        }

        public static void Clear()
        {
            m_bindings.Clear();
        }

        #region List operations

        // TODO: better lookup
        private static int IndexOf(CShortCut shortCut)
        {
            for (int i = 0; i < m_bindings.Count; ++i)
            {
                if (m_bindings[i].shortCut.Equals(shortCut))
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region Lookup

        private static IDictionary<string, KeyCode> CreateKeyCodeLookup()
        {
            IDictionary<string, KeyCode> lookup = new Dictionary<string, KeyCode>();

            lookup["backspace"] = KeyCode.Backspace;
            lookup["delete"] = KeyCode.Delete;
            lookup["tab"] = KeyCode.Tab;
            lookup["enter"] = KeyCode.Return;
            lookup["pause"] = KeyCode.Pause;
            lookup["escape"] = KeyCode.Escape;
            lookup["space"] = KeyCode.Space;

            lookup["num0"] = KeyCode.Keypad0;
            lookup["num1"] = KeyCode.Keypad1;
            lookup["num2"] = KeyCode.Keypad2;
            lookup["num3"] = KeyCode.Keypad3;
            lookup["num4"] = KeyCode.Keypad4;
            lookup["num5"] = KeyCode.Keypad5;
            lookup["num6"] = KeyCode.Keypad6;
            lookup["num7"] = KeyCode.Keypad7;
            lookup["num8"] = KeyCode.Keypad8;
            lookup["num9"] = KeyCode.Keypad9;

            lookup["0"] = KeyCode.Alpha0;
            lookup["1"] = KeyCode.Alpha1;
            lookup["2"] = KeyCode.Alpha2;
            lookup["3"] = KeyCode.Alpha3;
            lookup["4"] = KeyCode.Alpha4;
            lookup["5"] = KeyCode.Alpha5;
            lookup["6"] = KeyCode.Alpha6;
            lookup["7"] = KeyCode.Alpha7;
            lookup["8"] = KeyCode.Alpha8;
            lookup["9"] = KeyCode.Alpha9;

            lookup["kp_period"] = KeyCode.KeypadPeriod;
            lookup["kp_divide"] = KeyCode.KeypadDivide;
            lookup["kp_multiply"] = KeyCode.KeypadMultiply;
            lookup["kp_minus"] = KeyCode.KeypadMinus;
            lookup["kp_plus"] = KeyCode.KeypadPlus;
            lookup["kp_enter"] = KeyCode.KeypadEnter;
            lookup["kp_equals"] = KeyCode.KeypadEquals;

            lookup["up"] = KeyCode.UpArrow;
            lookup["down"] = KeyCode.DownArrow;
            lookup["right"] = KeyCode.RightArrow;
            lookup["left"] = KeyCode.LeftArrow;

            lookup["insert"] = KeyCode.Insert;
            lookup["home"] = KeyCode.Home;
            lookup["end"] = KeyCode.End;
            lookup["pageup"] = KeyCode.PageUp;
            lookup["pagedown"] = KeyCode.PageDown;

            lookup["f1"] = KeyCode.F1;
            lookup["f2"] = KeyCode.F2;
            lookup["f3"] = KeyCode.F3;
            lookup["f4"] = KeyCode.F4;
            lookup["f5"] = KeyCode.F5;
            lookup["f6"] = KeyCode.F6;
            lookup["f7"] = KeyCode.F7;
            lookup["f8"] = KeyCode.F8;
            lookup["f9"] = KeyCode.F9;
            lookup["f10"] = KeyCode.F10;
            lookup["f11"] = KeyCode.F11;
            lookup["f12"] = KeyCode.F12;
            lookup["f13"] = KeyCode.F13;
            lookup["f14"] = KeyCode.F14;
            lookup["f15"] = KeyCode.F15;

            // FIXME: add support for ' and "
            // lookup["doublequote"] = KeyCode.DoubleQuote;
            // lookup["quote"] = KeyCode.Quote;

            lookup[","] = KeyCode.Comma;
            lookup["-"] = KeyCode.Minus;
            lookup["="] = KeyCode.Equals;
            lookup["."] = KeyCode.Period;
            lookup["/"] = KeyCode.Slash;
            lookup[";"] = KeyCode.Semicolon;

            lookup["["] = KeyCode.LeftBracket;
            lookup["\\"] = KeyCode.Backslash;
            lookup["]"] = KeyCode.RightBracket;
            lookup["~"] = KeyCode.BackQuote;

            lookup["a"] = KeyCode.A;
            lookup["b"] = KeyCode.B;
            lookup["c"] = KeyCode.C;
            lookup["d"] = KeyCode.D;
            lookup["e"] = KeyCode.E;
            lookup["f"] = KeyCode.F;
            lookup["g"] = KeyCode.G;
            lookup["h"] = KeyCode.H;
            lookup["i"] = KeyCode.I;
            lookup["j"] = KeyCode.J;
            lookup["k"] = KeyCode.K;
            lookup["l"] = KeyCode.L;
            lookup["m"] = KeyCode.M;
            lookup["n"] = KeyCode.N;
            lookup["o"] = KeyCode.O;
            lookup["p"] = KeyCode.P;
            lookup["q"] = KeyCode.Q;
            lookup["r"] = KeyCode.R;
            lookup["s"] = KeyCode.S;
            lookup["t"] = KeyCode.T;
            lookup["u"] = KeyCode.U;
            lookup["v"] = KeyCode.V;
            lookup["w"] = KeyCode.W;
            lookup["x"] = KeyCode.X;
            lookup["y"] = KeyCode.Y;
            lookup["z"] = KeyCode.Z;

            lookup["numlock"] = KeyCode.Numlock;
            lookup["capslock"] = KeyCode.CapsLock;
            lookup["scrolllock"] = KeyCode.ScrollLock;
            lookup["rightshift"] = KeyCode.RightShift;
            lookup["leftshift"] = KeyCode.LeftShift;
            lookup["rightctrl"] = KeyCode.RightControl;
            lookup["leftctrl"] = KeyCode.LeftControl;
            lookup["rightalt"] = KeyCode.RightAlt;
            lookup["leftalt"] = KeyCode.LeftAlt;
            lookup["leftcmd"] = KeyCode.LeftCommand;
            lookup["rightcmd"] = KeyCode.RightCommand;
            lookup["print"] = KeyCode.Print;
            lookup["break"] = KeyCode.Break;

            lookup["mouse0"] = KeyCode.Mouse0;
            lookup["mouse1"] = KeyCode.Mouse1;
            lookup["mouse2"] = KeyCode.Mouse2;
            lookup["mouse3"] = KeyCode.Mouse3;
            lookup["mouse4"] = KeyCode.Mouse4;
            lookup["mouse5"] = KeyCode.Mouse5;
            lookup["mouse6"] = KeyCode.Mouse6;

            lookup["joy0"] = KeyCode.JoystickButton0;
            lookup["joy1"] = KeyCode.JoystickButton1;
            lookup["joy2"] = KeyCode.JoystickButton2;
            lookup["joy3"] = KeyCode.JoystickButton3;
            lookup["joy4"] = KeyCode.JoystickButton4;
            lookup["joy5"] = KeyCode.JoystickButton5;
            lookup["joy6"] = KeyCode.JoystickButton6;
            lookup["joy7"] = KeyCode.JoystickButton7;
            lookup["joy8"] = KeyCode.JoystickButton8;
            lookup["joy9"] = KeyCode.JoystickButton9;
            lookup["joy10"] = KeyCode.JoystickButton10;
            lookup["joy11"] = KeyCode.JoystickButton11;
            lookup["joy12"] = KeyCode.JoystickButton12;
            lookup["joy13"] = KeyCode.JoystickButton13;
            lookup["joy14"] = KeyCode.JoystickButton14;
            lookup["joy15"] = KeyCode.JoystickButton15;
            lookup["joy16"] = KeyCode.JoystickButton16;
            lookup["joy17"] = KeyCode.JoystickButton17;
            lookup["joy18"] = KeyCode.JoystickButton18;
            lookup["joy19"] = KeyCode.JoystickButton19;

            return lookup;
        }

        private static IDictionary<KeyCode, string> CreateKeyNameLookup()
        {
            IDictionary<KeyCode, string> lookup = new Dictionary<KeyCode, string>();

            foreach (KeyValuePair<string, KeyCode> e in keyCodeLookup)
            {
                lookup[e.Value] = e.Key;
            }

            return lookup;
        }

        public static bool TryParse(string name, out KeyCode code)
        {
            return keyCodeLookup.TryGetValue(name, out code);
        }

        public static string KeyName(KeyCode key)
        {
            string name;
            if (keyNameLookup.TryGetValue(key, out name))
            {
                return name;
            }

            return null;
        }

        #endregion

        #region Properties

        private static IDictionary<string, KeyCode> keyCodeLookup
        {
            get
            {
                if (s_keyCodeLookup == null)
                {
                    s_keyCodeLookup = CreateKeyCodeLookup();
                }
                return s_keyCodeLookup;
            }
        }

        private static IDictionary<KeyCode, string> keyNameLookup
        {
            get
            {
                if (s_keyNameLookup == null)
                {
                    s_keyNameLookup = CreateKeyNameLookup();
                }

                return s_keyNameLookup;
            }
        }

        internal static IList<CBinding> BindingsList
        {
            get { return m_bindings; }
        }

        internal static int Count
        {
            get { return m_bindings.Count; }
        }

        internal static string[] BindingsNames
        {
            get
            {
                if (s_names == null)
                {
                    ICollection<string> keys = keyCodeLookup.Keys;
                    s_names = new string[keys.Count];
                    keys.CopyTo(s_names, 0);
                    Array.Sort(s_names);
                }

                return s_names;
            }
        }

        #endregion
    }
}

