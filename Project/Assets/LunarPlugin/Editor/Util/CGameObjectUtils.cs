//
//  GameObjectUtils.cs
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

using System.Collections.Generic;

using LunarPlugin;
using LunarPluginInternal;

public delegate bool GameObjectFilter(GameObject o);
public delegate bool ComponentFilter(Component c);

public static class GameObjectUtils
{
    public static IList<GameObject> ListGameObjects(GameObjectFilter filter)
    {
        IList<GameObject> list = new List<GameObject>();

        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in objects)
        {
            if (filter(obj))
            {
                list.Add(obj);
            }
        }

        return list;
    }

    #if !(UNITY_4_7 || UNITY_4_6 || UNITY_4_5 || UNITY_4_4 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4)
    public static IList<GameObject> ListChildren(string name, GameObjectFilter filter)
    {
        GameObject obj = GameObject.Find(name);
        return obj != null ? ListChildren(obj, filter) : null;
    }

    public static IList<GameObject> ListChildren(GameObject obj, GameObjectFilter filter)
    {
        IList<GameObject> list = new List<GameObject>();

        GameObject[] children = obj.GetComponents<GameObject>();
        foreach (GameObject child in children)
        {
            if (filter(child))
            {
                list.Add(child);
            }
        }

        return list;
    }
    #endif // !(UNITY_4_7 || UNITY_4_6 || UNITY_4_5 || UNITY_4_4 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4)

    public static bool ShouldListObject(GameObject obj, string prefix)
    {
        return prefix == null || StringUtils.StartsWithIgnoreCase(obj.name, prefix);
    }
}
