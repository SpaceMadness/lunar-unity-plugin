using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using UnityEngine;

using LunarPlugin;

namespace LunarPluginInternal
{
    class CommandAutoCompleteException : Exception
    {
        public CommandAutoCompleteException(Exception e)
            : base("Custom command autocomplete exception", e)
        {
        }
    }
}
