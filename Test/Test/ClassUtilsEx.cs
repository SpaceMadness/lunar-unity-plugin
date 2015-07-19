using System;
using System.Reflection;

namespace LunarPlugin.Test
{
    static class ClassUtilsEx
    {
        public static void SetField(Type type, string name, object value)
        {
            SetField(type, null, name, value);
        }

        public static void SetField(Type type, object target, string name, object value)
        {
            FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }
    }
}

