using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using LunarPlugin;

namespace LunarPluginInternal
{
    class CVars
    {
        public static readonly CVar c_historySize = new CVar("c_historySize", 32768, CFlags.System);
        public static readonly CVar d_assertsEnabled = new CVar("d_assertsEnabled", Config.isDebugBuild, CFlags.System);

        #if LUNAR_DEVELOPMENT
        public static readonly CVar g_drawVisibleCells = new CVar("g_drawVisibleCells", false);
        #endif
    }
}
