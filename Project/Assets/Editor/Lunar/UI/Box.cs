using System;
using UnityEngine;

namespace LunarEditor
{
    class Box : View
    {
        public Box(float width, float height)
            : this(width, height, GUI.skin.box)
        {
        }

        public Box(float width, float height, GUIStyle style)
            : base(width, height)
        {
            this.Style = style;
        }

        public override void OnGUI()
        {
            GUI.Box(this.Frame, GUIContent.none, this.Style);
        }
    }
}

