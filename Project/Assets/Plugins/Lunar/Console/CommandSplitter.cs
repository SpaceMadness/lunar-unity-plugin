using System;
using System.Text;

using System.Collections.Generic;

namespace LunarPluginInternal
{
    class CommandSplitter
    {
        private const char Space        = ' ';
        private const char DoubleQuote  = '"';
        private const char SingleQuote  = '\'';
        private const char EscapeSymbol = '\\';

        public static IList<string> Split(string str)
        {
            IList<string> list = ReusableLists.NextAutoRecycleList<string>();
            Split(str, list);
            return list;
        }

        public static void Split(string str, IList<string> list)
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

            if (insideDoubleQuotes)
            {
                throw new TokenizeException("Missing closing double quote");
            }

            if (insideSingleQuotes)
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
    }
}

