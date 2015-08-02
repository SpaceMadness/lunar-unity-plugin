using UnityEngine;
using System.Collections;

namespace LunarPluginInternal
{
    interface ITerminalViewDelegate
    {
        void ExecCommand(string commandLine);
        Terminal Terminal { get; }
    }

    interface ITerminalView
    {
    }
}
