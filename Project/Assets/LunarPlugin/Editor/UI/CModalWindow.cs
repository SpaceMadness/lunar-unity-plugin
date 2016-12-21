//
//  CModalWindow.cs
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

namespace LunarEditor
{
    class CModalWindow
    {
        private static CView m_rootView;

        public CModalWindow()
        {
        }

        public void OnGUI()
        {
            float w = 2 * Screen.width / 3;
            float h = w / 1.6f;
            float x = 0.5f * (Screen.width - w);
            float y = 0.5f * (Screen.height - h);
            GUI.ModalWindow(0, new Rect(x, y, w, h), delegate(int id) {

                GUILayout.BeginVertical();
                {
                    GUILayout.Label("Select a server:");
                    GUILayout.Space(GUILayoutUtility.GetLastRect().height);

                    GUILayout.BeginScrollView(Vector2.zero);
                    {
                        m_rootView.OnGUI();
                    }
                    GUILayout.EndScrollView();

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Connect"))
                        {
                            Debug.Log("Connect");
                        }

                        if (GUILayout.Button("Cancel"))
                        {
                            Debug.Log("Cancel");
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }, "Test");
        }

        public static void Show(CView rootView)
        {
            if (rootView == null)
            {
                throw new NullReferenceException("Root view is null");
            }

            m_rootView = rootView;
        }

        public static void Hide()
        {
        }
    }
}

