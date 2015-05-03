using System;
using UnityEngine;

namespace LunarEditor
{
    class ModalWindow
    {
        private static View m_rootView;

        public ModalWindow()
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

        public static void Show(View rootView)
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

