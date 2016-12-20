//
//  TerminalAutoCompletion.cs
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

﻿using UnityEngine;

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
            return CommandTokenizer.GetAutoCompleteToken(line);
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

