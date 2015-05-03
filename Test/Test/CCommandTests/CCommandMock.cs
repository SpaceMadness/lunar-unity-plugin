using System;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

namespace CCommandTests
{
    public class CCommandMock : CCommand
    {
        public CCommandMock(string name, bool succeed = true)
            : base(name)
        {
            this.IsSucceed = succeed;
        }

        bool Execute(string[] args)
        {
            return this.IsSucceed;
        }

        public bool IsSucceed { get; set; }
    }
}

