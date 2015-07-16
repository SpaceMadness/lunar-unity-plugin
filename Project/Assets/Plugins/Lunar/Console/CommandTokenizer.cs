using System;
using System.Text;

using System.Collections.Generic;

namespace LunarPluginInternal
{
    class CommandTokenizer
    {
        public const int OPTION_IGNORE_MISSING_QUOTES = 1; // FIXME: use enum

        private const char DoubleQuote  = '"';
        private const char SingleQuote  = '\'';
        private const char EscapeSymbol = '\\';

        public static IList<string> Tokenize(string str, int options = 0)
        {
            IList<string> list = ReusableLists.NextAutoRecycleList<string>();
            Tokenize(str, list, options);
            return list;
        }

        public static void Tokenize(string str, IList<string> tokens, int options = 0)
        {
            StringBuilder tokenBuffer = StringBuilderPool.NextAutoRecycleBuilder();

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
                throw new TokenizeException("Missing closing double quote: " + str);
            }
            
            if (insideSingleQuotes && !HasOption(options, OPTION_IGNORE_MISSING_QUOTES))
            {
                throw new TokenizeException("Missing closing single quote: " + str);
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
            bool insideSingleQuotes = false;
            bool insideDoubleQuotes = false;

            int index = line.Length - 1;
            for (; index >= 0; --index)
            {
                char ch = line[index];
                if (char.IsWhiteSpace(ch))
                {
                    if (!insideDoubleQuotes && !insideSingleQuotes)
                    {
                        return index + 1;
                    }
                }
                else if (ch == DoubleQuote)
                {

                }
                else if (ch == SingleQuote)
                {
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

