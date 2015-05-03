using System;

namespace LunarPlugin
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CCommandOptionAttribute : Attribute
    {
        public CCommandOptionAttribute()
        {
            Required = false;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public string Name { get; set; }
        public string Description { get; set; }

        public string ShortName { get; set; }
        public bool Required { get; set; }

        public string Values { get; set; }

        #endregion
    }
}