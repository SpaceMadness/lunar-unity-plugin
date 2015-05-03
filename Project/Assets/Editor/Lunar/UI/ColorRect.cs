using UnityEngine;
using System.Collections;

namespace LunarEditor
{
    class ColorRect : View
    {
        private Color m_color;
        private GUIStyle m_style;

        public ColorRect(Rect frame, Color color)
            : base(frame)
        {
            m_style = new GUIStyle();
            this.Color = color;
        }

        public override void OnGUI()
        {
            GUI.Box(Frame, GUIContent.none, m_style);
        }

        public Color Color
        {
            get { return m_color; }
            set
            {
                if (m_color != value)
                {
                    m_color = value;
                    m_style.normal.background = UIHelper.Create1x1ColorTexture(m_color);
                }
            }
        }
    }
}
