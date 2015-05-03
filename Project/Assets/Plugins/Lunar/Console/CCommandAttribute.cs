using System;

using LunarPluginInternal;

namespace LunarPlugin
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CCommandAttribute : Attribute
    {
        public CCommandAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public string Description { get; set; }

        public string Values { get; set; }
        public CCommandFlags Flags { get; set; }

        public bool IsHidden { get { return HasFlag(CCommandFlags.Hidden); } }
        public bool IsDebug { get { return HasFlag(CCommandFlags.Debug); } }

        private bool HasFlag(CCommandFlags flag)
        {
            return (Flags & flag) != 0;
        }
    }
}