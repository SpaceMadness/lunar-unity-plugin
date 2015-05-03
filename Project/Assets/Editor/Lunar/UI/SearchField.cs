using UnityEngine;
using System.Collections;

namespace LunarEditor
{
    class SearchField : TextField
    {
        protected override string CreateTextField(string text)
        {
            BeginGroup(Frame);
            {
                GUILayout.BeginHorizontal();
                {
                    text = GUILayout.TextField(text, Style);
                    if (GUILayout.Button(GUIContent.none, SharedStyles.toolbarSearchCancelButton))
                    {
                        // Remove focus if cleared
                        text = "";
                        GUI.FocusControl(null);
                    }
                }
                GUILayout.EndHorizontal();
            }
            EndGroup();

            return text;
        }

        protected override GUIStyle CreateGUIStyle()
        {
            return SharedStyles.searchField;
        }
    }
}