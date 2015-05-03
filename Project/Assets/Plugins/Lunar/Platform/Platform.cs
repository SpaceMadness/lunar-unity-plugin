using System;
using System.Reflection;

using UnityEngine;

using LunarPlugin;

namespace LunarPluginInternal
{
    internal static class Platform
    {
        static PlatformImpl s_impl;

        static Platform()
        {
            s_impl = CreateImpl();
        }

        static PlatformImpl CreateImpl()
        {
            try
            {
                if (Application.isEditor)
                {
                    string typeName = "LunarEditor.EditorPlatform";
                    Type type = ClassUtils.TypeForName(typeName);
                    if (type != null)
                    {
                        return ClassUtils.CreateInstance<PlatformImpl>(type);
                    }
                    else
                    {
                        Debug.LogError("Can't find " + typeName + " type");
                    }
                }

                return new PlatformDefault();
            }
            catch (MissingMethodException) // FIXME: I don't like this
            {
                // unit test running
                Type type = ClassUtils.TypeForName("LunarPluginInternal.TestingPlatform");
                return ClassUtils.CreateInstance<PlatformImpl>(type);
            }
        }

        internal static void AssertMessage(string message, string stackTrace)
        {
            s_impl.AssertMessage(message, stackTrace);
        }
    }

    abstract class PlatformImpl
    {
        public abstract void AssertMessage(string message, string stackTrace);
    }
}

