//
//  EditorSkin.cs
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

﻿using System;

using UnityEditor;
using UnityEngine;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    static class EditorSkin
    {
        private static Colors colors;

        static EditorSkin()
        {
            ResolveColors();
        }

        internal static void ResolveColors()
        {
            if (isProSkin)
            {
                colors = new DarkColors();
            }
            else
            {
                colors = new LightColors();
            }
        }

        public static Color GetColor(ColorCode code)
        {
            return colors.GetColor(code);
        }

        internal static void SetColor(ColorCode code, Color color)
        {
            colors.SetColor(code, color);
        }

        public static string SetColors(string line)
        {
            return line != null ? StringUtils.SetColors(line, colors.m_lookup) : null;
        }

        private static bool isProSkin
        {
            get
            {
                try
                {
                    return EditorGUIUtility.isProSkin;
                }
                catch (MissingMethodException)
                {
                    // running a unit test
                }

                return false;
            }
        }

        abstract class Colors
        {
            internal Color[] m_lookup;

            public Colors()
            {
                int lookupSize = Enum.GetNames(typeof(ColorCode)).Length;

                m_lookup = new Color[lookupSize];

                m_lookup[(int)ColorCode.Clear] = CreateClear();
                m_lookup[(int)ColorCode.Plain] = CreatePlain();
                m_lookup[(int)ColorCode.TableCommand] = CreateTableCommand();
                m_lookup[(int)ColorCode.TableCommandDisabled] = CreateTableCommandDisabled();
                m_lookup[(int)ColorCode.TableVar] = CreateTableVar();
                m_lookup[(int)ColorCode.Error] = CreateError();
                m_lookup[(int)ColorCode.ErrorUnknownCommand] = CreateErrorUnknownCommand();
                m_lookup[(int)ColorCode.LevelCritical] = CreateLevelCritical();
                m_lookup[(int)ColorCode.LevelError] = CreateLevelError();
                m_lookup[(int)ColorCode.LevelWarning] = CreateLevelWarning();
                m_lookup[(int)ColorCode.LevelInfo] = CreateLevelInfo();
                m_lookup[(int)ColorCode.LevelDebug] = CreateLevelDebug();
                m_lookup[(int)ColorCode.LevelVerbose] = CreateLevelVerbose();
                m_lookup[(int)ColorCode.Link] = CreateLink();
                m_lookup[(int)ColorCode.LinkInnactive] = CreateLinkInnactive();
            }

            public Color GetColor(ColorCode code)
            {
                int index = (int)code;
                if (index >= 0 && index < m_lookup.Length)
                {
                    return m_lookup[index];
                }

                return m_lookup[(int)ColorCode.Clear];
            }

            internal void SetColor(ColorCode code, Color color)
            {
                int index = (int)code;
                if (index < 0 || index >= m_lookup.Length)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                m_lookup[index] = color;
            }

            protected abstract Color CreateClear();
            protected abstract Color CreatePlain();
            protected abstract Color CreateTableCommand();
            protected abstract Color CreateTableCommandDisabled();
            protected abstract Color CreateTableVar();
            protected abstract Color CreateError();
            protected abstract Color CreateErrorUnknownCommand();
            protected abstract Color CreateLevelCritical();
            protected abstract Color CreateLevelError();
            protected abstract Color CreateLevelWarning();
            protected abstract Color CreateLevelInfo();
            protected abstract Color CreateLevelDebug();
            protected abstract Color CreateLevelVerbose();
            protected abstract Color CreateLink();
            protected abstract Color CreateLinkInnactive();
        }

        class LightColors : Colors
        {
            protected override Color CreateClear() { return ColorUtils.FromRGB(0xc2c2c2); }
            protected override Color CreatePlain() { return ColorUtils.FromRGB(0x243e57); }
            protected override Color CreateTableCommand() { return ColorUtils.FromRGB(0x512352); }
            protected override Color CreateTableCommandDisabled() { return ColorUtils.FromRGB(0x3e413f); }
            protected override Color CreateTableVar() { return ColorUtils.FromRGB(0x4e4f2f); }
            protected override Color CreateError() { return ColorUtils.FromRGB(0xbe2323); }
            protected override Color CreateErrorUnknownCommand() { return ColorUtils.FromRGB(0xbe2323); }
            protected override Color CreateLevelCritical() { return ColorUtils.FromRGB(0xbe2323); }
            protected override Color CreateLevelError() { return ColorUtils.FromRGB(0xbe2323); }
            protected override Color CreateLevelWarning() { return ColorUtils.FromRGB(0xa7551d); }
            protected override Color CreateLevelInfo() { return ColorUtils.FromRGB(0x475480); }
            protected override Color CreateLevelDebug() { return ColorUtils.FromRGB(0x243e57); }
            protected override Color CreateLevelVerbose() { return ColorUtils.FromRGB(0x3e413f); }
            protected override Color CreateLink() { return ColorUtils.FromRGB(0x193562); }
            protected override Color CreateLinkInnactive() { return Color.gray; }
        }

        class DarkColors : Colors
        {
            protected override Color CreateClear() { return ColorUtils.FromRGB(0x383838); }
            protected override Color CreatePlain() { return ColorUtils.FromRGB(0xb8c4d0); }
            protected override Color CreateTableCommand() { return ColorUtils.FromRGB(0xffcf85); }
            protected override Color CreateTableCommandDisabled() { return ColorUtils.FromRGB(0xb8c4d0); }
            protected override Color CreateTableVar() { return ColorUtils.FromRGB(0xb4c974); }
            protected override Color CreateError() { return ColorUtils.FromRGB(0xe1614e); }
            protected override Color CreateErrorUnknownCommand() { return ColorUtils.FromRGB(0xe1614e); }
            protected override Color CreateLevelCritical() { return ColorUtils.FromRGB(0xff6f5d); }
            protected override Color CreateLevelError() { return ColorUtils.FromRGB(0xff6f5d); }
            protected override Color CreateLevelWarning() { return ColorUtils.FromRGB(0xffcf85); }
            protected override Color CreateLevelInfo() { return ColorUtils.FromRGB(0xb4c974); }
            protected override Color CreateLevelDebug() { return ColorUtils.FromRGB(0xdee4ed); }
            protected override Color CreateLevelVerbose() { return ColorUtils.FromRGB(0xb8c4d0); }
            protected override Color CreateLink() { return ColorUtils.FromRGB(0x6ba1ff); }
            protected override Color CreateLinkInnactive() { return Color.gray; }
        }
    }
}

