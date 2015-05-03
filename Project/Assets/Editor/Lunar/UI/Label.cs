using UnityEngine;

using System.Collections;
using System.Text;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class Label : View
    {
        private GUIContent m_content;

        public Label(string text = "", GUIStyle style = null)
        {
            if (style != null)
            {
                this.Style = style;
            }

            m_content = new GUIContent(StringUtils.NonNullOrEmpty(text));
            Vector2 size = CalcTextSize();
            this.Frame = new Rect(0, 0, size.x, size.y);
        }

        public Label(string text, float width, GUIStyle style = null)
        {
            if (style != null)
            {
                this.Style = style;
            }

            m_content = new GUIContent(StringUtils.NonNullOrEmpty(text));
            Vector2 size = WordWrap(m_content, width);
            this.Frame = new Rect(0, 0, width, size.y);
        }

        //////////////////////////////////////////////////////////////////////////////

        public override void OnGUI()
        {
            GUI.Label(Frame, m_content, Style);
        }

        //////////////////////////////////////////////////////////////////////////////

        public float CalcTextWidth()
        {
            return Style.CalcSize(m_content).x;
        }

        public Vector2 CalcTextSize()
        {
            return Style.CalcSize(m_content);
        }

        //////////////////////////////////////////////////////////////////////////////

        private Vector2 WordWrap(GUIContent content, float maxWidth)
        {
            string[] words = content.text.Split(' ');
            string result = "";
            Vector2 size = new Vector2();

            for (int i = 0; i < words.Length; ++i)
            {
                content.text = result + words [i] + ' ';
                size = Style.CalcSize(content);
                if (size.x > maxWidth)
                {
                    result += ('\n' + words [i] + ' ');
                } else
                {
                    result = content.text;
                }
            }

            return size;
        }

        //////////////////////////////////////////////////////////////////////////////
        
        #region Inheritance
        
        protected override GUIStyle CreateGUIStyle()
        {
            GUIStyle style = new GUIStyle("label");
            style.richText = true;
            return style;
        }
        
        protected override void DrawBackgroud()
        {
            // draw background with a label
        }
        
        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public string Text
        { 
            get { return m_content.text; } 
            set { m_content.text = value; } 
        }

        public Color TextColor
        {
            get { return Style.normal.textColor; }
            set { Style.normal.textColor = value; }
        }

        public FontStyle FontStyle
        {
            get { return Style.fontStyle; }
            set { Style.fontStyle = value; }
        }

        #endregion
    }
}