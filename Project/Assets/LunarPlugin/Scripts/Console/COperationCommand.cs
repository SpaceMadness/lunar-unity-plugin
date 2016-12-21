//
//  COperationCommand.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

﻿using System;
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
                throw new ArgumentNullException("identifier");
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
            CVar cvar = ResolveCvar(this.Identifier);
            if (cvar != null)
            {
                return ExecuteCvar(cvar);
            }

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

        private CVar ResolveCvar(string identifier)
        {
            CVar cvar = CRegistery.FindCvar(identifier);
            if (cvar == null)
            {
                PrintError("Can't find boolean variable: '{0}'", identifier);
                return null;
            }

            if (!cvar.IsBool)
            {
                PrintError("Boolean variable expected: '{0}'", identifier);
                return null;
            }

            return cvar;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public string Identifier { get; private set; }
        public char Operation { get; private set; }

        #endregion
    }
}

