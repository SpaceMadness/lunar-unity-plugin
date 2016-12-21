//
//  CCollection.cs
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

﻿using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;

namespace LunarPluginInternal
{
    static class CCollection
    {
        public static void Each<T>(IList<T> list, CEachDelegate<T> each)
        {
            foreach (T element in list)
            {
                each(element);
            }
        }

        public static void Each<T>(IList<T> list, CEachIndexDelegate<T> each)
        {
            int index = 0;
            foreach (T element in list)
            {
                each(element, index++);
            }
        }

        public static OUT[] Map<IN, OUT>(IList<IN> list, CMapDelegate<IN, OUT> map)
        {
            OUT[] result = new OUT[list.Count];

            int index = 0;
            foreach (IN element in list)
            {
                result[index++] = map(element);
            }
            return result;
        }

        public static OUT[] Map<IN, OUT>(IList<IN> list, CMapIndexDelegate<IN, OUT> map)
        {
            OUT[] result = new OUT[list.Count];

            int index = 0;
            foreach (IN element in list)
            {
                result[index] = map(element, index);
                ++index;
            }
            return result;
        }
    }

    delegate void CEachDelegate<T>(T element);
    delegate void CEachIndexDelegate<T>(T element, int index);

    delegate OUT CMapDelegate<IN, OUT>(IN element);
    delegate OUT CMapIndexDelegate<IN, OUT>(IN element, int index);
}
