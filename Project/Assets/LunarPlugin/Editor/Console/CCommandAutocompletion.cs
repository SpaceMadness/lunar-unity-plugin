//
//  CCommandAutocompletion.cs
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

    public class CCommandAutocompletion
    {
        private static readonly string[] EMPTY_SUGGESTIONS = new string[0]; // TODO: rename
        private static readonly string[] SINGLE_SUGGESTION = new string[1]; // TODO: rename

        public static string[] getSuggestions(string line)
        {
            return getSuggestions(line, line.Length);
        }

        public static string[] getSuggestions(string line, int index)
        {
            if (line == null)
            {
                throw new NullReferenceException("Line is null");
            }

            if (index < 0 || index > line.Length)
            {
                throw new ArgumentOutOfRangeException("Index is out of range: " + index);
            }

            if (index != line.Length)
            {
                return EMPTY_SUGGESTIONS; // TODO: add middle line suggestions
            }

            IList<string> tokens = getTokens(line);
            if (tokens.Count == 0)
            {
                return getDefaultSuggestions();
            }

            if (tokens.Count == 1)
            {
                return getSuggestions(line, tokens[0]);
            }

            return getSuggestions(line, tokens);
        }

        private static string[] getSuggestions(string line, string token)
        {
            IList<CCommand> commands = CRegistery.ListCommands(token);

            if (commands.Count == 0) // no commands found
            {
                return EMPTY_SUGGESTIONS;
            }

            if (commands.Count == 1) // single command
            {
                CCommand cmd = commands[0];

                // check if line already contains suggested value (e.g. "cmdlist ")
                if (line.Length == cmd.Name.Length + 1 && line.EndsWith(" ") && line.StartsWith(cmd.Name))
                {
                    throw new NotImplementedException(); // FIXME
                }

                return singleSuggestion(toDisplayName(cmd));
            }

            return getSuggestions(commands); // multiple commands
        }

        private static string[] getSuggestions(string commandLine, IList<string> tokens)
        {
            CIterator<string> iter = new CIterator<string>(tokens);

            CCommand cmd = CRegistery.FindCommand(iter.Next());
            if (cmd == null)
            {
                return EMPTY_SUGGESTIONS; // no command found
            }

            while (iter.HasNext())
            {
                string token = iter.Next();
                int iterPos = iter.Position; // store position to revert for the case if option skip fails

                // first try to parse options
                if (token.StartsWith("--")) // long option
                {
                    string optionName = token.Substring(2);
                    if (SkipOption(iter, cmd, optionName)) continue;

                    iter.Position = iterPos;
                    return getSuggestedOptions(iter, cmd, optionName, "--");
                }
                else if (token.StartsWith("-") && !CStringUtils.IsNumeric(token)) // short option
                {
                    string optionName = token.Substring(1);
                    if (SkipOption(iter, cmd, optionName)) continue;

                    iter.Position = iterPos;
                    return getSuggestedOptions(iter, cmd, optionName, "-");
                }

                if (iter.HasNext())
                {
                    return EMPTY_SUGGESTIONS; // TODO: add multiple args suggestion support
                }

                return getSuggestedArgs(commandLine, cmd, token);
            }

            return getSuggestedArgs(commandLine, cmd, "");
        }

        private static string[] getDefaultSuggestions()
        {
            return getSuggestions(CRegistery.ListCommands());
        }

        private static string[] getSuggestions(IList<CCommand> commands)
        {
            string[] names = new string[commands.Count];
            int index = 0;

            foreach (CCommand cmd in commands)
            {
                names[index++] = toDisplayName(cmd);
            }

            return names;
        }

        //////////////////////////////////////////////////////////////////////////////
        // Options

        private static bool SkipOption(CIterator<string> iter, CCommand cmd, string name)
        {
            Option opt = cmd.FindOption(name);
            return opt != null && SkipOption(iter, opt) && iter.HasNext();
        }

        private static bool SkipOption(CIterator<string> iter, Option opt)
        {
            Type type = opt.Target.FieldType;

            if (type.IsArray)
            {
                Array arr = (Array) opt.Target.GetValue(opt);
                if (arr != null)
                {
                    int index;
                    int length = arr.Length;
                    for (index = 0; index < length && iter.HasNext(); ++index)
                    {
                        string value = iter.Next();
                        if (!IsValidOptionString(value))
                        {
                            return false;
                        }
                    }

                    return index == length;
                }

                return false;
            }

            if (type == typeof(int) || 
                type == typeof(float) || 
                type == typeof(string))
            {
                if (iter.HasNext())
                {
                    string value = iter.Next();
                    return IsValidOptionString(value);
                }

                return false;
            }

            if (type == typeof(bool))
            {
                return true;
            }

            return false;
        }

        private static bool IsValidOptionString(string value)
        {
            return Option.IsValidValue(typeof(string), value); // don't actually check types: just format
        }

        private static string[] getSuggestedOptions(CIterator<string> iter, CCommand cmd, string optNameToken, string prefix)
        {
            List<Option> optionsList = new List<Option>(); // TODO: reuse list

            // list options
            bool useShort = prefix.Equals("-");
            if (useShort)
            {
                cmd.ListShortOptions(optionsList, optNameToken);
                optionsList.Sort(delegate(Option op1, Option op2) {
                    return op1.ShortName.CompareTo(op2.ShortName);
                });
            }
            else
            {
                cmd.ListOptions(optionsList, optNameToken);
                optionsList.Sort(delegate(Option op1, Option op2) {
                    return op1.Name.CompareTo(op2.Name);
                });
            }

            if (optionsList.Count > 1) // multiple options available
            {
                return getSuggestedOptions(optionsList, useShort);
            }

            if (optionsList.Count == 1) // single option available
            {
                Option opt = optionsList[0];

                if (isOptionNameMatch(opt, optNameToken, useShort)) // option name already matched - try values
                {
                    if (opt.HasValues()) // option has predefined values?
                    {
                        if (iter.HasNext()) // has value token?
                        {
                            return opt.ListValues(iter.Next());
                        }

                        return opt.Values;
                    }

                    if (iter.HasNext())
                    {
                        return EMPTY_SUGGESTIONS; // don't suggest option value
                    }
                }

                return singleSuggestion(getSuggestedOption(opt, useShort)); // suggest option`s name
            }

            return EMPTY_SUGGESTIONS;
        }

        private static string[] getSuggestedOptions(List<Option> options, bool useShort)
        {
            if (options.Count > 0)
            {
                string[] names = new string[options.Count];
                for (int i = 0; i < options.Count; ++i)
                {
                    names[i] = getSuggestedOption(options[i], useShort);
                }

                return names;
            }

            return EMPTY_SUGGESTIONS;
        }

        private static string getSuggestedOption(Option opt, bool useShort)
        {
            if (useShort)
            {
                return CStringUtils.C("-" + opt.ShortName, CColorCode.TableVar);
            }

            return CStringUtils.C("--" + opt.Name, CColorCode.TableVar);
        }

        private static bool isOptionNameMatch(Option opt, string token, bool useShort)
        {
            if (useShort)
            {
                return opt.ShortName != null && CStringUtils.EqualsIgnoreCase(opt.ShortName, token);
            }

            return CStringUtils.EqualsIgnoreCase(opt.Name, token);
        }


        //////////////////////////////////////////////////////////////////////////////
        // Arguments

        private static string[] getSuggestedArgs(string commandLine, CCommand cmd, string token)
        {
            string[] values = cmd.Values;
            if (values != null && values.Length > 0)
            {
                return getSuggestedArgs(values, token);
            }

            IList<string> customValues = cmd.AutoCompleteCustomArgs(commandLine, token);
            if (customValues != null && customValues.Count > 0)
            {
                return getSuggestedArgs(customValues, token);
            }

            return EMPTY_SUGGESTIONS;
        }

        private static string[] getSuggestedArgs(IList<string> values, string token)
        {
            // we need to keep suggested values in a sorted order
            List<string> sortedValues = new List<string>(values.Count);
            for (int i = 0; i < values.Count; ++i)
            {
                if (token.Length == 0 || CStringUtils.StartsWithIgnoreCase(CStringUtils.RemoveRichTextTags(values[i]), token))
                {
                    sortedValues.Add(CStringUtils.RemoveRichTextTags(values[i]));
                }
            }

            if (sortedValues.Count == 0)
            {
                return EMPTY_SUGGESTIONS;
            }

            if (sortedValues.Count > 1)
            {
                sortedValues.Sort();
                return sortedValues.ToArray();
            }

            return singleSuggestion(sortedValues[0]);
        }

        //////////////////////////////////////////////////////////////////////////////
        // Helpers

        private static string[] singleSuggestion(string suggestion)
        {
            if (suggestion == null)
            {
                throw new NullReferenceException("Suggestion is null");
            }

            SINGLE_SUGGESTION[0] = suggestion;
            return SINGLE_SUGGESTION;
        }

        private static string toDisplayName(CCommand cmd)
        {
            CColorCode color = cmd is CVarCommand ? CColorCode.TableVar : cmd.ColorCode;
            return CStringUtils.C(cmd.Name, color);
        }

        private static IList<string> getTokens(string line)
        {
            IList<string> tokens = CCommandTokenizer.Tokenize(line, CCommandTokenizer.OPTION_IGNORE_MISSING_QUOTES);
            if (tokens.Count > 0 && line.EndsWith(" "))
            {
                tokens.Add(""); // treat last space as "empty" token
            }
            return tokens;
        }
    }
}

