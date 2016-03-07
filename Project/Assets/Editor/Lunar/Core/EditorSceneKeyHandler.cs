//
//  EditorSceneKeyHandler.cs
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

using UnityEngine;
using UnityEditor;
using LunarPluginInternal;

namespace LunarEditor
{
    static class EditorSceneKeyHandler
    {
        public delegate bool KeyHandler(KeyCode key, CModifiers modifiers);

        #pragma warning disable 0649
        public static KeyHandler keyDownHandler;
        public static KeyHandler keyUpHandler;
        #pragma warning restore 0649

        private static bool[] s_pressedKeyCodeFlags;

        static EditorSceneKeyHandler()
        {
            KeyCode lastKeyCode;
            #if UNITY_4_7 || UNITY_4_6 || UNITY_4_5 || UNITY_4_4 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4
            lastKeyCode = KeyCode.Joystick4Button19;
            #else
            lastKeyCode = KeyCode.Joystick8Button19;
            #endif

            int keysCount = (int)lastKeyCode + 1;
            s_pressedKeyCodeFlags = new bool[keysCount];

            SceneView.onSceneGUIDelegate += HandleOnSceneFunc;
        }

        private static void HandleOnSceneFunc(SceneView sceneView)
        {
            try
            {
                Event evt = Event.current;
                if (evt != null && evt.isKey)
                {
                    int keyIndex = (int)evt.keyCode;
                    if (keyIndex < 0 || keyIndex >= s_pressedKeyCodeFlags.Length)
                    {
                        return;
                    }

                    if (evt.type == EventType.KeyDown)
                    {
                        if (!s_pressedKeyCodeFlags[keyIndex])
                        {
                            s_pressedKeyCodeFlags[keyIndex] = true;
                            if (keyDownHandler != null && keyDownHandler(evt.keyCode, GetModifiers(evt)))
                            {
                                evt.Use();
                            }
                        }
                    }
                    else if (evt.type == EventType.KeyUp)
                    {
                        if (s_pressedKeyCodeFlags[keyIndex])
                        {
                            s_pressedKeyCodeFlags[keyIndex] = false;
                            if (keyUpHandler != null && keyUpHandler(evt.keyCode, GetModifiers(evt)))
                            {
                                evt.Use();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Exception while handling scene keys: " +  e.Message);
            }
        }

        private static CModifiers GetModifiers(Event evt)
        {
            CModifiers modifiers = 0;

            if (evt.alt)
                modifiers |= CModifiers.Alt;
            if (evt.shift)
                modifiers |= CModifiers.Shift;
            if (evt.control)
                modifiers |= CModifiers.Control;
            if (evt.command)
                modifiers |= CModifiers.Command;
            
            return modifiers;
        }
    }
}

