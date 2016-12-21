//
//  CTestingPlatform.cs
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

﻿using System;

using LunarPlugin;

namespace LunarPluginInternal
{
    delegate void CTestingPlatformAssertDelegate(string message, string stackTrace);

    class CTestingPlatform : CPlatformImpl
    {
        private static CTestingPlatformAssertDelegate s_assertDelegate = DefaultAssertDelegate;

        public override void AssertMessage(string message, string stackTrace)
        {
            s_assertDelegate(message, stackTrace);
        }

        private static void DefaultAssertDelegate(string message, string stackTrace)
        {
            throw new Exception("Assertion failed: " + message);
        }

        public static CTestingPlatformAssertDelegate AssertDelegate
        {
            get { return s_assertDelegate; }
            set { s_assertDelegate = value != null ? value : DefaultAssertDelegate; }
        }
    }
}

