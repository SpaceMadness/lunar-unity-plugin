//
//  CommandSplitter.cs
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
using System.Text;

using System.Collections.Generic;

namespace LunarPluginInternal
{
    class CommandSplitter
    {
        public const int OPTION_IGNORE_MISSING_QUOTES = 1; // TODO: use enum
    
        private const char Space        = ' ';
        private const char DoubleQuote  = '"';
        private const char SingleQuote  = '\'';
        private const char EscapeSymbol = '\\';

        public static IList<string> Split(string str, int options = 0)
        {
            IList<string> list = ReusableLists.NextAutoRecycleList<string>();
            Split(str, list, options);
            return list;
        }

        public static void Split(string str, IList<string> list, int options = 0)
        {
            StringBuilder commandBuffer = StringBuilderPool.NextAutoRecycleBuilder();

            bool insideDoubleQuotes = false;
            bool insideSingleQuotes = false;

            char ch = (char) 0;
            char prevCh;
            for (int i = 0; i < str.Length;)
            {
                prevCh = ch;
                ch = str[i++];

                if (ch == '&' && i < str.Length && str[i] == '&') // found command separator
                {
                    if (!insideDoubleQuotes && !insideSingleQuotes)
                    {
                        AddCommand(commandBuffer, list);
                        ++i; // skip second separator's char
                    }
                    else
                    {
                        commandBuffer.Append(ch);
                    }
                }
                else if (ch == DoubleQuote)
                {
                    commandBuffer.Append(ch);

                    if (insideDoubleQuotes)
                    {
                        insideDoubleQuotes = prevCh == EscapeSymbol;
                    }
                    else
                    {
                        insideDoubleQuotes = prevCh != EscapeSymbol;
                    }
                }
                else if (ch == SingleQuote)
                {
                    commandBuffer.Append(ch);
                    if (insideSingleQuotes)
                    {
                        insideSingleQuotes = prevCh == EscapeSymbol;
                    }
                    else
                    {
                        insideSingleQuotes = prevCh != EscapeSymbol;
                    }
                }
                else
                {
                    commandBuffer.Append(ch);
                }
            }

            if (insideDoubleQuotes && !HasOption(options, OPTION_IGNORE_MISSING_QUOTES))
            {
                throw new TokenizeException("Missing closing double quote");
            }

            if (insideSingleQuotes && !HasOption(options, OPTION_IGNORE_MISSING_QUOTES))
            {
                throw new TokenizeException("Missing closing single quote");
            }

            if (commandBuffer.Length > 0)
            {
                AddCommand(commandBuffer, list);
            }
        }

        private static void AddCommand(StringBuilder buffer, IList<string> list)
        {
            string command = buffer.ToString().Trim();
            if (command.Length == 0)
            {
                throw new TokenizeException("Can't add empty command");
            }

            list.Add(command);
            buffer.Length = 0;
        }
        
        private static bool HasOption(int options, int option)
        {
            return (options & option) != 0;
        }
    }
}

