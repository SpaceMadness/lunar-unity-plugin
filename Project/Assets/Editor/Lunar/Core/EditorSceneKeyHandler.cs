using System;

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
            int keysCount = (int)KeyCode.Joystick8Button19 + 1;
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

