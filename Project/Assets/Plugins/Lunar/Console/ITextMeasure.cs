using UnityEngine;
using System.Collections;

namespace LunarPluginInternal
{
    interface ITextMeasure
    {
        Vector2 CalcSize(string text);
        float CalcHeight(string text, float width);
        float LineHeight { get; }
    }
}