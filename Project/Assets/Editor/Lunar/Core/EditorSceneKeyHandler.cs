using System;

using UnityEngine;
using UnityEditor;

namespace LunarEditor
{
    static class EditorSceneKeyHandler
    {
        public delegate void KeyHandler(KeyCode key);

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
                            if (keyDownHandler != null)
                            {
                                keyDownHandler(evt.keyCode);
                            }
                        }
                    }
                    else if (evt.type == EventType.KeyUp)
                    {
                        if (s_pressedKeyCodeFlags[keyIndex])
                        {
                            s_pressedKeyCodeFlags[keyIndex] = false;
                            if (keyUpHandler != null)
                            {
                                keyUpHandler(evt.keyCode);
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
    }
}

