using System;

using UnityEngine;

using NUnit.Framework;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

namespace LunarPlugin.Test
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class MonospaceTextMeasureTest
    {
        private readonly ITextMeasure m_textMeasure = new MonospaceTextMeasure(10, 1);

        [Test]
        public void TestTextSize()
        {
            string text = "Text";
            AssertSize(text, 40, 1);
        }

        private void AssertSize(string text, int width, int height)
        {
            Assert.AreEqual(new Vector2(width, height), m_textMeasure.CalcSize(text));
        }
    }
}

