using UnityEngine;

using System;
using System.Collections.Generic;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    using Option = CCommand.Option;

    public static class TerminalAutocompletion
    {
        public static Result Autocomplete(string line)
        {
            return Autocomplete(line, line.Length);
        }

        public static Result Autocomplete(string line, int index)
        {
            string[] suggestions = CommandAutocompletion.getSuggestions(line, index);

            Result result;
            result.line = GetNewLine(line, suggestions);
            result.suggestions = suggestions.Length > 1 ? suggestions : null;

            return result;
        }

        private static string GetNewLine(string line, string[] suggestions)
        {
            if (suggestions.Length == 0)
            {
                return null;
            }

            string token = GetToken(line);
            if (token == null)
            {
                return null;
            }

            if (suggestions.Length == 1)
            {
                return ReplaceToken(line, token, StringUtils.RemoveRichTextTags(suggestions[0])) + " ";
            }

            string suggestion = StringUtils.GetSuggestedText(token, suggestions, true);
            if (suggestion == null || suggestion.Equals(token))
            {
                return null;
            }

            return ReplaceToken(line, token, suggestion);
        }

        private static string GetToken(string line)
        {
            if (line.Length == 0)
            {
                return string.Empty;
            }

            IList<string> tokens = CommandTokenizer.Tokenize(line, CommandTokenizer.OPTION_IGNORE_MISSING_QUOTES);
            return tokens.Count > 0 ? tokens[tokens.Count - 1] : null;
        }

        private static string ReplaceToken(string line, string token, string suggestion)
        {
            return line.Substring(0, line.Length - token.Length) + suggestion;
        }

        public struct Result
        {
            public static readonly Result Empty = new Result();

            public string line;
            public string[] suggestions;

            public bool IsEmpty
            {
                get { return line == null; }
            }
        }
    }
}

