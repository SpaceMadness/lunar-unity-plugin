//
//  CSystemVars.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using LunarPlugin;

namespace LunarPluginInternal
{
    class CSystemVars
    {
        public static readonly CVar c_historySize = new CVar("c_historySize", 32768, CFlags.System);
        public static readonly CVar d_assertsEnabled = new CVar("d_assertsEnabled", CConfig.isDebugBuild, CFlags.System);

        #if LUNAR_DEVELOPMENT
        public static readonly CVar g_drawVisibleCells = new CVar("g_drawVisibleCells", false);
        #endif
    }
}
