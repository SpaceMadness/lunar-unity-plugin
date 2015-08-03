using UnityEngine;
using System.Collections;

namespace LunarPluginInternal
{
    class MonospaceTextMeasure : ITextMeasure
    {
        private readonly int m_charWidth;
        private readonly int m_lineHeight;

        public MonospaceTextMeasure(int charWidth, int lineHeight)
        {
            m_charWidth = charWidth;
            m_lineHeight = lineHeight;
        }

        public Vector2 CalcSize(string text)
        {
            int charsCount = text.Length;
            for (int i = text.Length - 1; i >= 0 && char.IsWhiteSpace(text[i]); --i)
            {
                --charsCount;
            }

            return new Vector2(charsCount * m_charWidth, m_lineHeight);
        }

        public float CalcHeight(string text, float width)
        {
            throw new System.NotImplementedException();
        }

        public float LineHeight
        {
            get { return m_lineHeight; }
        }
    }
}
