//
//  CEditorSkin.cs
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
    static class CEditorSkin
    {
        private static Colors colors;

        static CEditorSkin()
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

        public static Color GetColor(CColorCode code)
        {
            return colors.GetColor(code);
        }

        internal static void SetColor(CColorCode code, Color color)
        {
            colors.SetColor(code, color);
        }

        public static string SetColors(string line)
        {
            return line != null ? CStringUtils.SetColors(line, colors.m_lookup) : null;
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
                int lookupSize = Enum.GetNames(typeof(CColorCode)).Length;

                m_lookup = new Color[lookupSize];

                m_lookup[(int)CColorCode.Clear] = CreateClear();
                m_lookup[(int)CColorCode.Plain] = CreatePlain();
                m_lookup[(int)CColorCode.TableCommand] = CreateTableCommand();
                m_lookup[(int)CColorCode.TableCommandDisabled] = CreateTableCommandDisabled();
                m_lookup[(int)CColorCode.TableVar] = CreateTableVar();
                m_lookup[(int)CColorCode.Error] = CreateError();
                m_lookup[(int)CColorCode.ErrorUnknownCommand] = CreateErrorUnknownCommand();
                m_lookup[(int)CColorCode.LevelCritical] = CreateLevelCritical();
                m_lookup[(int)CColorCode.LevelError] = CreateLevelError();
                m_lookup[(int)CColorCode.LevelWarning] = CreateLevelWarning();
                m_lookup[(int)CColorCode.LevelInfo] = CreateLevelInfo();
                m_lookup[(int)CColorCode.LevelDebug] = CreateLevelDebug();
                m_lookup[(int)CColorCode.LevelVerbose] = CreateLevelVerbose();
                m_lookup[(int)CColorCode.Link] = CreateLink();
                m_lookup[(int)CColorCode.LinkInnactive] = CreateLinkInnactive();
            }

            public Color GetColor(CColorCode code)
            {
                int index = (int)code;
                if (index >= 0 && index < m_lookup.Length)
                {
                    return m_lookup[index];
                }

                return m_lookup[(int)CColorCode.Clear];
            }

            internal void SetColor(CColorCode code, Color color)
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
            protected override Color CreateClear() { return CColorUtils.FromRGB(0xc2c2c2); }
            protected override Color CreatePlain() { return CColorUtils.FromRGB(0x243e57); }
            protected override Color CreateTableCommand() { return CColorUtils.FromRGB(0x512352); }
            protected override Color CreateTableCommandDisabled() { return CColorUtils.FromRGB(0x3e413f); }
            protected override Color CreateTableVar() { return CColorUtils.FromRGB(0x4e4f2f); }
            protected override Color CreateError() { return CColorUtils.FromRGB(0xbe2323); }
            protected override Color CreateErrorUnknownCommand() { return CColorUtils.FromRGB(0xbe2323); }
            protected override Color CreateLevelCritical() { return CColorUtils.FromRGB(0xbe2323); }
            protected override Color CreateLevelError() { return CColorUtils.FromRGB(0xbe2323); }
            protected override Color CreateLevelWarning() { return CColorUtils.FromRGB(0xa7551d); }
            protected override Color CreateLevelInfo() { return CColorUtils.FromRGB(0x475480); }
            protected override Color CreateLevelDebug() { return CColorUtils.FromRGB(0x243e57); }
            protected override Color CreateLevelVerbose() { return CColorUtils.FromRGB(0x3e413f); }
            protected override Color CreateLink() { return CColorUtils.FromRGB(0x193562); }
            protected override Color CreateLinkInnactive() { return Color.gray; }
        }

        class DarkColors : Colors
        {
            protected override Color CreateClear() { return CColorUtils.FromRGB(0x383838); }
            protected override Color CreatePlain() { return CColorUtils.FromRGB(0xb8c4d0); }
            protected override Color CreateTableCommand() { return CColorUtils.FromRGB(0xffcf85); }
            protected override Color CreateTableCommandDisabled() { return CColorUtils.FromRGB(0xb8c4d0); }
            protected override Color CreateTableVar() { return CColorUtils.FromRGB(0xb4c974); }
            protected override Color CreateError() { return CColorUtils.FromRGB(0xe1614e); }
            protected override Color CreateErrorUnknownCommand() { return CColorUtils.FromRGB(0xe1614e); }
            protected override Color CreateLevelCritical() { return CColorUtils.FromRGB(0xff6f5d); }
            protected override Color CreateLevelError() { return CColorUtils.FromRGB(0xff6f5d); }
            protected override Color CreateLevelWarning() { return CColorUtils.FromRGB(0xffcf85); }
            protected override Color CreateLevelInfo() { return CColorUtils.FromRGB(0xb4c974); }
            protected override Color CreateLevelDebug() { return CColorUtils.FromRGB(0xdee4ed); }
            protected override Color CreateLevelVerbose() { return CColorUtils.FromRGB(0xb8c4d0); }
            protected override Color CreateLink() { return CColorUtils.FromRGB(0x6ba1ff); }
            protected override Color CreateLinkInnactive() { return Color.gray; }
        }
    }
}

