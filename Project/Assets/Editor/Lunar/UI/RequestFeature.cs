using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace LunarEditor
{
    class RequestFeature
    {
        internal static void Open()
        {
            Application.OpenURL("https://github.com/SpaceMadness/lunar-unity-plugin/issues/new");
        }
    }
}
