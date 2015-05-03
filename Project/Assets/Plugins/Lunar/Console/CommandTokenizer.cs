using System;
using System.Text;

using System.Collections.Generic;

namespace LunarPluginInternal
{
    class CommandTokenizer
    {
        private const char DoubleQuote  = '"';
        private const char SingleQuote  = '\'';
        private const char EscapeSymbol = '\\';

        public static IList<string> Tokenize(string str)
        {
            IList<string> list = ReusableLists.NextAutoRecycleList<string>();
            Tokenize(str, list);
            return list;
        }

        public static void Tokenize(string str, IList<string> tokens)
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

            if (insideDoubleQuotes)
            {
                throw new TokenizeException("Missing closing double quote: " + str);
            }

            if (insideSingleQuotes)
            {
                throw new TokenizeException("Missing closing single quote: " + str);
            }

            AddToken(tokenBuffer, tokens);
        }

        private static void AddToken(StringBuilder buffer, IList<string> list, bool addEmpty = false)
        {
            if (buffer.Length > 0 || addEmpty)
            {
                list.Add(buffer.ToString());
                buffer.Length = 0;
            }
        }
    }
}

