using UnityEngine;

using System;

using LunarPlugin;

namespace LunarPluginInternal
{
    class PlatformDefault : PlatformImpl
    {
        public override void AssertMessage(string message, string stackTrace)
        {
            Debug.LogError("Assertion failed: " + message + "\n" + stackTrace);
        }
    }
}

