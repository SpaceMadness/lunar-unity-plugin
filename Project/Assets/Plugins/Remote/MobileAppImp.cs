using UnityEngine;
using System;
using LunarPlugin;

namespace LunarPluginInternal
{
    class MobileAppImp : DefaultAppImp
    {
        public override void LogTerminal(string line)
        {
            MobileAppObject.LogTerminal(line);
        }

        public override void LogTerminal(string[] table)
        {
            MobileAppObject.LogTerminal(table);
        }

        public override void LogTerminal(Exception e, string message)
        {
            MobileAppObject.LogTerminal(e, message);
        }

        public override bool IsPromptEnabled
        {
            get { return true; }
        }
    }
}
