using System;
using System.Collections.Generic;

using LunarPlugin;

namespace LunarPluginInternal
{
    class COperationCommand : CCommand
    {
        const char kOpPlus = '+';
        const char kOpMinus = '-';

        public COperationCommand(char operation, string identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException("Identifier is null");
            }

            if (identifier.Length == 0)
            {
                throw new ArgumentException("Empty identifier");
            }

            this.Name = operation + identifier;
            this.Identifier = identifier;
            this.Operation = operation;
        }

        protected virtual bool Execute()
        {
            CVar cvar = FindCvar(this.Identifier);
            if (cvar != null)
            {
                return ExecuteCvar(cvar);
            }

            PrintError("Unknown identifier: {0}", this.Identifier);
            return false;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region CVars

        private bool ExecuteCvar(CVar cvar)
        {
            switch (this.Operation)
            {
                case kOpPlus:
                {
                    cvar.BoolValue = true;
                    return true;
                }

                case kOpMinus:
                {
                    cvar.BoolValue = false;
                    return true;
                }
            }

            PrintError("Unknown operation: '{0}'", this.Operation.ToString());
            return false;
        }

        private CVar FindCvar(string identifier)
        {
            CVar cvar = CRegistery.FindCvar(identifier);
            return cvar != null && cvar.IsBool ? cvar : null; // only boolean vars are acceptable
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public string Identifier { get; private set; }
        public char Operation { get; private set; }

        #endregion
    }
}

