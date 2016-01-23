//
//  CVarCommand.cs
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
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

using LunarPlugin;

namespace LunarPluginInternal
{
    class CVarCommand : CCommand
    {
        private static Regex colorRegex;

        public readonly CVar cvar;

        public CVarCommand(CVar cvar)
        {   
            this.cvar = cvar;
            this.Name = cvar.Name;

            this.IsDebug = cvar.IsDebug;
            this.IsHidden = cvar.IsHidden;
            this.IsSystem = cvar.IsSystem;
        }

        bool Execute(string[] args)
        {
            if (args.Length == 0)
            {
                PrintIndent("{0} is:\"{1}\" default:\"{2}\"", StringUtils.C(cvar.Name, ColorCode.TableVar), cvar.Value, cvar.DefaultValue);
                return false;
            }

            switch (cvar.Type)
            {
                case CVarType.Boolean:
                {
                    if (args.Length != 1)
                    {
                        PrintError("Unexpected args count");
                        return false;
                    }

                    int value;
                    if (int.TryParse(args[0], out value) && (value == 0 || value == 1))
                    {
                        SetValue(value != 0);
                        return true;
                    }

                    PrintError("Invalid value: only '0' and '1' are permitted", args[0]);
                    return false;
                }

                case CVarType.Integer:
                {
                    if (args.Length != 1)
                    {
                        PrintError("Unexpected args count");
                        return false;
                    }

                    int value;
                    if (int.TryParse(args[0], out value))
                    {
                        SetValue(value);
                        return true;
                    }

                    PrintError("Invalid value");
                    return false;
                }

                case CVarType.Float:
                {
                    if (args.Length != 1)
                    {
                        PrintError("Unexpected args count");
                        return false;
                    }

                    float value;
                    if (float.TryParse(args[0], out value))
                    {
                        SetValue(value);
                        return true;
                    }

                    PrintError("Invalid value");
                    return false;
                }

                case CVarType.String:
                {
                    if (args.Length != 1)
                    {
                        PrintError("Unexpected args count");
                        return false;
                    }

                    SetValue(args[0]);
                    return true;
                }

                case CVarType.Color:
                {
                    if (args.Length == 3 || args.Length == 4)
                    {
                        float[] values = StringUtils.ParseFloats(args);
                        if (values == null)
                        {
                            PrintError("Invalid values");
                            return false;
                        }

                        for (int i = 0; i < values.Length; ++i)
                        {
                            if (values[i] < 0.0f || values[i] > 1.0f)
                            {
                                PrintError("Invalid values");
                                return false;
                            }
                        }

                        Color color = args.Length == 3 ? 
                            new Color(values[0], values[1], values[2]) :
                            new Color(values[0], values[1], values[2], values[3]);

                        SetValue(ref color);
                        return true;
                    }

                    if (args.Length == 1)
                    {
                        if (colorRegex == null)
                        {
                            colorRegex = new Regex("^0[xX]([\\dabcdefABCDEF]{1,8})$");
                        }

                        Match match = colorRegex.Match(args[0]);
                        if (!match.Success)
                        {
                            PrintError("Invalid color value: expected 0xRGB[A]");
                            return false;
                        }

                        string value = match.Groups[1].Value;
                        uint colorValue = Convert.ToUInt32(value, 16);

                        if (value.Length == 8)
                        {
                            Color color = ColorUtils.FromRGBA(colorValue);
                            SetValue(ref color);
                        }
                        else
                        {
                            Color color = ColorUtils.FromRGB(colorValue);
                            SetValue(ref color);
                        }

                        return true;
                    }

                    PrintError("Unexpected args count: expected R G B A or R G B or 0xRGB[A]");
                    return false;
                }

                case CVarType.Rect:
                {
                    if (args.Length != 4)
                    {
                        PrintError("Unexpected args count: expected X Y W H");
                        return false;
                    }

                    float[] values = StringUtils.ParseFloats(args);
                    if (values == null)
                    {
                        PrintError("Invalid values");
                        return false;
                    }

                    Rect rect = new Rect(values[0], values[1], values[2], values[3]);
                    SetValue(ref rect);

                    return true;
                }

                case CVarType.Vector2:
                {
                    if (args.Length != 2)
                    {
                        PrintError("Unexpected args count: expected X Y");
                        return false;
                    }

                    float[] values = StringUtils.ParseFloats(args);
                    if (values == null)
                    {
                        PrintError("Invalid values");
                        return false;
                    }

                    Vector2 vector = new Vector2(values[0], values[1]);
                    SetValue(ref vector);

                    return true;
                }

                case CVarType.Vector3:
                {
                    if (args.Length != 2 && args.Length != 3)
                    {
                        PrintError("Unexpected args count: expected X Y or X Y Z");
                        return false;
                    }

                    float[] values = StringUtils.ParseFloats(args);
                    if (values == null)
                    {
                        PrintError("Invalid values");
                        return false;
                    }

                    if (values.Length == 2)
                    {
                        Vector3 vector = new Vector3(values[0], values[1]);
                        SetValue(ref vector);
                    }
                    else
                    {
                        Vector3 vector = new Vector3(values[0], values[1], values[2]);
                        SetValue(ref vector);
                    }

                    return true;
                }

                case CVarType.Vector4:
                {
                    if (args.Length < 2 || args.Length > 4)
                    {
                        PrintError("Unexpected args count: expected X Y or X Y Z or X Y Z W");
                        return false;
                    }

                    float[] values = StringUtils.ParseFloats(args);
                    if (values == null)
                    {
                        PrintError("Invalid values");
                        return false;
                    }

                    if (values.Length == 2)
                    {
                        Vector4 vector = new Vector4(values[0], values[1]);
                        SetValue(ref vector);
                    }
                    else if (values.Length == 3)
                    {
                        Vector4 vector = new Vector4(values[0], values[1], values[2]);
                        SetValue(ref vector);
                    }
                    else
                    {
                        Vector4 vector = new Vector4(values[0], values[1], values[2], values[3]);
                        SetValue(ref vector);
                    }

                    return true;
                }
            }

            if (args.Length != 1)
            {
                Print("usage ");
                return false;
            }

            if (cvar.IsFloat)
            {
                float value;
                if (float.TryParse(args[0], out value))
                {
                    SetValue(value);
                    return true;
                }

                PrintError("Invalid float \"{0}\"", args[0]);
                return false;
            }

            if (cvar.IsInt)
            {
                int value;
                if (int.TryParse(args[0], out value))
                {
                    if (cvar.IsBool && value != 0 && value != 1)
                    {
                        PrintError("Invalid value \"{0}\" only \"0\" and \"1\" are permitted", args[0]);
                        return false;
                    }

                    SetValue(value);
                    return true;
                }

                PrintError("Invalid int \"{0}\"", args[0]);
                return false;
            }

            string str = args[0];
            SetValue(str);
            return true;
        }

        public void SetValue(float value)
        {
            if (cvar.FloatValue != value)
            {
                cvar.FloatValue = value;
                OnValueChanged();
            }
        }

        public void SetValue(bool value)
        {
            if (cvar.BoolValue != value)
            {
                cvar.BoolValue = value;
                OnValueChanged();
            }
        }

        public void SetValue(int value)
        {
            if (cvar.IntValue != value)
            {
                cvar.IntValue = value;
                OnValueChanged();
            }
        }

        public void SetValue(string value)
        {
            if (cvar.Value != value)
            {
                cvar.Value = value;
                OnValueChanged();
            }
        }

        public void SetValue(ref Color value)
        {
            if (cvar.ColorValue != value)
            {
                cvar.ColorValue = value;
                OnValueChanged();
            }
        }

        public void SetValue(ref Rect value)
        {
            if (cvar.RectValue != value)
            {
                cvar.RectValue = value;
                OnValueChanged();
            }
        }

        public void SetValue(ref Vector2 value)
        {
            if (cvar.Vector2Value != value)
            {
                cvar.Vector2Value = value;
                OnValueChanged();
            }
        }

        public void SetValue(ref Vector3 value)
        {
            if (cvar.Vector3Value != value)
            {
                cvar.Vector3Value = value;
                OnValueChanged();
            }
        }

        public void SetValue(ref Vector4 value)
        {
            if (cvar.Vector4Value != value)
            {
                cvar.Vector4Value = value;
                OnValueChanged();
            }
        }

        public void SetDefault()
        {
            if (!cvar.IsDefault)
            {
                cvar.IsDefault = true;
                OnValueChanged();
            }
        }

        private void OnValueChanged()
        {
            PostNotification(CCommandNotifications.CVarValueChanged,
                CCommandNotifications.CVarValueChangedKeyVar, cvar,
                CCommandNotifications.KeyManualMode, this.IsManualMode
            );
        }

        public string Value
        {
            get { return cvar.Value; }
        }

        public string DefaultValue
        {
            get { return cvar.DefaultValue; }
        }

        public int IntValue
        {
            get { return cvar.IntValue; }
        }

        public float FloatValue
        {
            get { return cvar.FloatValue; }
        }

        public bool BoolValue
        {
            get { return cvar.BoolValue; }
        }

        public bool IsString
        {
            get { return cvar.IsString; }
        }

        public bool IsInt
        {
            get { return cvar.IsInt; }
        }

        public bool IsFloat
        {
            get { return cvar.IsFloat; }
        }

        public bool IsBool
        {
            get { return cvar.IsBool; }
        }

        public bool IsDefault
        {
            get { return cvar.IsDefault; }
        }

        public bool HasFlag(CFlags flag)
        {
            return cvar.HasFlag(flag);
        }
    }
}
