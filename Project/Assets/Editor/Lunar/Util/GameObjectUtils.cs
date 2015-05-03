using UnityEngine;

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

    public static bool ShouldListObject(GameObject obj, string prefix)
    {
        return prefix == null || StringUtils.StartsWithIgnoreCase(obj.name, prefix);
    }
}
