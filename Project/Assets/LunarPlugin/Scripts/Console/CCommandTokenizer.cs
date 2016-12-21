//
//  CCommandTokenizer.cs
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
    class CCommandTokenizer
    {
        public const int OPTION_IGNORE_MISSING_QUOTES = 1; // FIXME: use enum

        private const char DoubleQuote  = '"';
        private const char SingleQuote  = '\'';
        private const char EscapeSymbol = '\\';

        public static IList<string> Tokenize(string str, int options = 0)
        {
            IList<string> list = new List<string>();
            Tokenize(str, list, options);
            return list;
        }

        public static void Tokenize(string str, IList<string> tokens, int options = 0)
        {
            StringBuilder tokenBuffer = new StringBuilder();

            bool insideSingleQuotes = false;
            bool insideDoubleQuotes = false;
            bool shouldAddToken = true;

            char ch = (char) 0;
            char prevCh;

            for (int i = 0; i < str.Length;)
            {
                prevCh = ch;
                ch = str[i++];

                if (char.IsWhiteSpace(ch))
                {
                    if (!insideDoubleQuotes && !insideSingleQuotes && shouldAddToken)
                    {
                        AddToken(tokenBuffer, tokens);
                    }
                    else
                    {
                        tokenBuffer.Append(ch);
                    }
                }
                else if (ch == DoubleQuote)
                {
                    if (insideSingleQuotes)
                    {
                        tokenBuffer.Append(ch);
                    }
                    else
                    {
                        if (insideDoubleQuotes)
                        {
                            if (prevCh == EscapeSymbol)
                            {
                                tokenBuffer.Append(ch);
                            }
                            else
                            {
                                if (shouldAddToken)
                                {
                                    AddToken(tokenBuffer, tokens, true);
                                }
                                else
                                {
                                    tokenBuffer.Append(ch);
                                }

                                shouldAddToken = true;
                                insideDoubleQuotes = false;
                            }
                        }
                        else
                        {
                            shouldAddToken = char.IsWhiteSpace(prevCh) || prevCh == (char)0;
                            if (!shouldAddToken)
                            {
                                tokenBuffer.Append(ch);
                            }
                            insideDoubleQuotes = true;
                        }
                    }
                }
                else if (ch == SingleQuote)
                {
                    if (insideDoubleQuotes)
                    {
                        tokenBuffer.Append(ch);
                    }
                    else 
                    {
                        if (insideSingleQuotes)
                        {
                            if (prevCh == EscapeSymbol)
                            {
                                tokenBuffer.Append(ch);
                            }
                            else
                            {
                                if (shouldAddToken)
                                {
                                    AddToken(tokenBuffer, tokens, true);
                                }
                                else
                                {
                                    tokenBuffer.Append(ch);
                                }

                                shouldAddToken = true;
                                insideSingleQuotes = false;
                            }
                        }
                        else
                        {
                            shouldAddToken = char.IsWhiteSpace(prevCh) || prevCh == (char)0;
                            if (!shouldAddToken)
                            {
                                tokenBuffer.Append(ch);
                            }
                            insideSingleQuotes = true;
                        }
                    }
                }
                else
                {
                    tokenBuffer.Append(ch);
                }
            }
            
            if (insideDoubleQuotes && !HasOption(options, OPTION_IGNORE_MISSING_QUOTES))
            {
                throw new CCommandTokenizeException("Missing closing double quote: " + str);
            }
            
            if (insideSingleQuotes && !HasOption(options, OPTION_IGNORE_MISSING_QUOTES))
            {
                throw new CCommandTokenizeException("Missing closing single quote: " + str);
            }
            
            AddToken(tokenBuffer, tokens);
        }

        public static string GetAutoCompleteToken(string line)
        {
            int index = GetAutoCompleteTokenStartIndex(line);
            return line.Substring(index);
        }

        private static int GetAutoCompleteTokenStartIndex(string line)
        {
            int index = line.Length - 1;
            for (; index >= 0; --index)
            {
                char ch = line[index];
                if (char.IsWhiteSpace(ch))
                {
                    return index + 1;
                }
            }

            return 0;
        }

        private static void AddToken(StringBuilder buffer, IList<string> list, bool addEmpty = false)
        {
            if (buffer.Length > 0 || addEmpty)
            {
                list.Add(buffer.ToString());
                buffer.Length = 0;
            }
        }

        private static bool HasOption(int options, int option)
        {
            return (options & option) != 0;
        }
    }
}

