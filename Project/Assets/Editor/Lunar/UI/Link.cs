using System;

using UnityEngine;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class Link : View
    {
        private GUIContent m_content;
        private string m_href;

        public Link(string text, string href)
        {
            if (text == null)
            {
                throw new ArgumentNullException("Text is null");
            }

            if (href == null)
            {
                throw new ArgumentNullException("Href is null");
            }

            m_content = new GUIContent(StringUtils.NonNullOrEmpty(text));
            m_href = href;

            Vector2 size = this.Style.CalcSize(m_content);
            this.Frame = new Rect(0, 0, size.x, size.y);
        }

        protected override GUIStyle CreateGUIStyle()
        {
            return SharedStyles.linkTextStyle;
        }

        protected override void DrawGUI()
        {
            BeginGroup(Frame);
            {
                if (GUI.Button(this.Bounds, m_content, this.Style))
                {
                    EditorApp.HandleURL(m_href);
                }
            }
            EndGroup();

            Rect linkFrame = this.Frame;
            linkFrame.height = this.Style.font.ascent;
            UIHelper.DrawUnderLine(linkFrame, this.Style.normal.textColor);
        }
    }
}

