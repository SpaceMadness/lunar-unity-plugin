using System;

using LunarPlugin;

namespace LunarPluginInternal
{
    class CAliasCommand : CCommand
    {
        public CAliasCommand(string name, string alias)
            : base(name)
        {
            if (alias == null)
            {
                throw new NullReferenceException("Alias is null");
            }

            this.Alias = alias;
        }

        bool Execute()
        {
            return ExecCommand(this.Alias, true);
        }

        public string Alias { get; internal set; }
    }
}

