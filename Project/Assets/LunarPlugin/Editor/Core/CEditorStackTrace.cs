//
//  EditorStackTrace.cs
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
using System.Text.RegularExpressions;

using UnityEngine;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    struct SourcePathEntry
    {
        public static readonly SourcePathEntry Invalid = new SourcePathEntry(null, 0);

        public readonly string sourcePath;
        public readonly int lineNumber;

        public SourcePathEntry(string sourcePath, int lineNumber)
        {
            this.sourcePath = sourcePath;
            this.lineNumber = lineNumber;
        }

        public bool IsValid { get { return sourcePath != null; } }
    }

    struct StackTraceLine
    {
        public static readonly StackTraceLine[] kEmptyLinesArray = new StackTraceLine[0];

        public string line;
        public readonly string sourcePath;
        public readonly bool sourcePathExists;

        public readonly int lineNumber;
        public readonly int sourcePathStart;
        public readonly int sourcePathEnd;

        public Rect frame;
        public Rect sourceFrame;

        public StackTraceLine(string line, string sourcePath = null, int lineNumber = -1, int lineNumberLength = 0)
        {
            this.line = line;
            this.sourcePath = sourcePath;
            this.lineNumber = lineNumber;
            this.frame = default(Rect);
            this.sourceFrame = default(Rect);

            if (!string.IsNullOrEmpty(sourcePath))
            {
                sourcePathExists = EditorFileUtils.PathExists(sourcePath);
                sourcePathStart = line.IndexOf(sourcePath);
                sourcePathEnd = sourcePathStart + sourcePath.Length + 1 + lineNumberLength;
            }
            else
            {
                sourcePathExists = false;
                sourcePathStart = sourcePathEnd = -1;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is StackTraceLine)
            {
                StackTraceLine other = (StackTraceLine) obj;
                return string.Equals(other.line, line) &&
                    string.Equals(other.sourcePath, sourcePath) &&
                    other.lineNumber == lineNumber;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return line.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[StackTraceLine] line='{0}' sourcePath='{1}' lineNumber={2}", line, sourcePath, lineNumber);
        }

        public bool IsClickable
        {
            get { return sourcePathStart != -1 && sourcePathEnd != -1; }
        }
    }

    class EditorStackTrace
    {
        private static Regex m_patterFilename;
        private static Regex m_patternSystemStackTrace;
        private static Regex m_patternUnityStackTrace;
        private static Regex m_patternMethodArg;
        private static Regex m_patternCompilerMessageSourcePath;

        public static string ExtractFileName(string stackTrace)
        {
            Match match = PatternFilename.Match(stackTrace);
            if (match.Success)
            {
                return match.Groups[3].ToString();
            }

            return null;
        }

        public static StackTraceLine[] ParseStackTrace(string stackTrace)
        {
            if (stackTrace != null)
            {
                ReusableList<StackTraceLine> list = ReusableLists.NextList<StackTraceLine>();

                int lineStart = 0;
                int lineEnd;

                while (lineStart < stackTrace.Length)
                {
                    // extract next line
                    lineEnd = StringUtils.EndOfLineIndex(stackTrace, lineStart);
                    string line = stackTrace.Substring(lineStart, lineEnd - lineStart);
                    lineStart = lineEnd + 1;

                    // extract data
                    Match m;
                    if ((m = PatternUnityStackTrace.Match(line)).Success)
                    {
                        GroupCollection groups = m.Groups;
                        string sourcePath = groups[2].Value;
                        string lineNumberStr = groups[3].Value;
                        int lineNumber = StringUtils.ParseInt(lineNumberStr, -1);

                        list.Add(new StackTraceLine(line, string.IsNullOrEmpty(sourcePath) ? null : sourcePath, lineNumber, lineNumberStr.Length));
                    }
                    else if ((m = PatternSystemStackTrace.Match(line)).Success)
                    {
                        GroupCollection groups = m.Groups;

                        string method = groups[1].Value;
                        string args = OptimizeArgs(groups[2].Value);
                        string path = OptimizePath(groups[4].Value);
                        string lineNumberStr = groups[5].Value;
                        int lineNumber = StringUtils.ParseInt(lineNumberStr, -1);

                        if (!string.IsNullOrEmpty(path) && lineNumber != -1)
                        {
                            line = StringUtils.TryFormat("{0}({1}) (at {2}:{3})", method, args, path, lineNumberStr);
                        }
                        else
                        {
                            line = StringUtils.TryFormat("{0}({1})", method, args);
                        }

                        list.Add(new StackTraceLine(line, path, lineNumber, lineNumberStr.Length));
                    }
                    else
                    {
                        list.Add(new StackTraceLine(line));
                    }
                }

                StackTraceLine[] lines = list.ToArray();
                list.Recycle();
                return lines;
            }

            return StackTraceLine.kEmptyLinesArray;
        }

        public static bool TryParseCompilerMessage(string line, out SourcePathEntry entry)
        {
            if (line != null)
            {
                Match m = PatternCompilerMessageSourcePath.Match(line);
                if (m.Success)
                {
                    string path = m.Groups[1].Value;
                    int lineNumber = StringUtils.ParseInt(m.Groups[2].Value, -1);
                    entry = new SourcePathEntry(path, lineNumber);
                    return true;
                }
            }

            entry = SourcePathEntry.Invalid;
            return false;
        }

        private static string OptimizePath(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            int index = value.IndexOf("/Assets/");
            return index == -1 ? value : value.Substring(index + 1);
        }

        private static string OptimizeArgs(string args)
        {
            return string.IsNullOrEmpty(args) ? string.Empty : PatternMethodArg.Replace(args, "$2");
        }

        private static Regex PatternFilename
        {
            get
            {
                if (m_patterFilename == null)
                {
                    m_patterFilename = new Regex(@"\(at (.*/)?((.*?):[\d]+)\)");
                }

                return m_patterFilename;
            }
        }

        private static Regex PatternSystemStackTrace
        {
            get
            {
                if (m_patternSystemStackTrace == null)
                    m_patternSystemStackTrace = new Regex(@"  at (.*?) \((.*?)\)( \[.*?\] in (.*?):(\d+))?");

                return m_patternSystemStackTrace;
            }
        }

        private static Regex PatternUnityStackTrace
        {
            get
            {
                if (m_patternUnityStackTrace == null)
                    m_patternUnityStackTrace = new Regex(@"(.*?\(.*?\)) \(at (.*?):(\d+)\)");

                return m_patternUnityStackTrace;
            }
        }

        private static Regex PatternCompilerMessageSourcePath
        {
            get
            {
                if (m_patternCompilerMessageSourcePath == null)
                    m_patternCompilerMessageSourcePath = new Regex(@"\s+(.*?)\((\d+),\d+\):");
                return m_patternCompilerMessageSourcePath;
            }
        }

        private static Regex PatternMethodArg
        {
            get
            {
                if (m_patternMethodArg == null)
                {
                    m_patternMethodArg = new Regex(@"([^\s,]+\.)?([^\s,]+) ([^\s,]*)");
                }

                return m_patternMethodArg;
            }
        }
    }

    static class UnityStackTraceParser
    {
        private static Regex m_pattern;

        public static bool TryParse(string line, out SourcePathEntry element)
        {
            Match match = Pattern.Match(line);
            if (match.Success)
            {
                string path = match.Groups[1].ToString();
                string lineVal = match.Groups[2].ToString();

                int lineNumber;
                if (int.TryParse(lineVal, out lineNumber))
                {
                    element = new SourcePathEntry(path, lineNumber);
                    return true;
                }
            }

            element = SourcePathEntry.Invalid;
            return false;
        }

        private static Regex Pattern
        {
            get
            {
                if (m_pattern == null)
                {
                    m_pattern = new Regex(@"\(at (.*?):([\d]+)\)");
                }

                return m_pattern;
            }
        }
    }
}

