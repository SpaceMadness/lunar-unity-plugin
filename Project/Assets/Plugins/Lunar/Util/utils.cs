using UnityEngine;
using System.Collections;

namespace LunarPluginInternal
{
    static class ColorUtils
    {
        private const float kMultiplier = 1.0f / 255.0f;

        public static Color FromRGBA(uint value)
        {
            float r = ((value >> 24) & 0xff) * kMultiplier;
            float g = ((value >> 16) & 0xff) * kMultiplier;
            float b = ((value >> 8) & 0xff) * kMultiplier;
            float a = (value & 0xff) * kMultiplier;

            return new Color(r, g, b, a);
        }

        public static Color FromRGB(uint value)
        {
            float r = ((value >> 16) & 0xff) * kMultiplier;
            float g = ((value >> 8) & 0xff) * kMultiplier;
            float b = (value & 0xff) * kMultiplier;
            float a = 1.0f;

            return new Color(r, g, b, a);
        }

        public static uint ToRGBA(ref Color value)
        {
            uint r = (uint)(value.r * 255.0f) & 0xff;
            uint g = (uint)(value.g * 255.0f) & 0xff;
            uint b = (uint)(value.b * 255.0f) & 0xff;
            uint a = (uint)(value.a * 255.0f) & 0xff;

            return (r << 24) | (g << 16) | (b << 8) | a;
        }
    }
}

