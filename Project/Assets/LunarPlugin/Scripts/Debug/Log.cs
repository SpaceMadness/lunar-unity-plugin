//
//  Log.cs
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

using LunarPluginInternal;

namespace LunarPlugin
{
    public class Tag : IComparable<Tag>
    {
        private static Dictionary<string, Tag> m_registry;
        private static int m_nextOrdinal;

        public Tag(string name)
            : this(name, true)
        {
        }

        public Tag(string name, Color color)
            : this(name, color, Color.clear, true)
        {
        }

        public Tag(string name, bool enabled)
            : this(name, Color.clear, Color.clear, enabled)
        {
        }

        public Tag(string name, Color color, Color backColor)
            : this(name, color, backColor, true)
        {
        }

        public Tag(string name, Color color, Color backColor, bool enabled)
        {
            if (name == null)
            {
                throw new NullReferenceException("Name is null");
            }

            Name = name;
            Enabled = enabled;
            Color = color;
            BackColor = backColor;
            Ordinal = m_nextOrdinal++;

            Register(this);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IComparable

        public int CompareTo(Tag other)
        {
            return Name.CompareTo(other.Name);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public override bool Equals(object obj)
        {
            Tag tag = obj as Tag;
            return tag != null && tag.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Registry

        private static void Register(Tag tag)
        {
            if (m_registry == null)
            {
                m_registry = new Dictionary<string, Tag>();
            }
            m_registry[tag.Name] = tag;
        }

        internal static Tag Find(string name)
        {
            if (name != null && m_registry != null)
            {
                Tag tag;
                if (m_registry.TryGetValue(name, out tag))
                {
                    return tag;
                }
            }

            return null;
        }

        internal static IList<Tag> ListTags()
        {
            return ListTags(ReusableLists.NextAutoRecycleList<Tag>());
        }

        internal static IList<Tag> ListTags(IList<Tag> outList)
        {
            foreach (Tag tag in m_registry.Values)
            {
                outList.Add(tag);
            }

            return outList;
        }

        internal static int TotalCount
        {
            get { return m_nextOrdinal; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public string Name { get; private set; }

        public bool Enabled { get; set; }

        public Color Color { get; set; }

        public Color BackColor { get; set; }

        public int Ordinal { get; private set; }

        #endregion
    }

    public sealed class LogLevel
    {
        public static readonly LogLevel Verbose   = new LogLevel(0, "verbose",   "V", ColorCode.LevelVerbose);
        public static readonly LogLevel Debug     = new LogLevel(1, "debug",     "D", ColorCode.LevelDebug);
        public static readonly LogLevel Info      = new LogLevel(2, "info",      "I", ColorCode.LevelInfo);
        public static readonly LogLevel Warn      = new LogLevel(3, "warning",   "W", ColorCode.LevelWarning);
        public static readonly LogLevel Error     = new LogLevel(4, "error",     "E", ColorCode.LevelError);
        public static readonly LogLevel Exception = new LogLevel(5, "exception", "C", ColorCode.LevelCritical);

        private static Dictionary<string, LogLevel> m_lookup;

        private LogLevel(int priority, string name, string shortName, ColorCode color = ColorCode.Clear, ColorCode backColor = ColorCode.Clear)
        {
            this.Priority = priority;
            this.Name = name;
            this.ShortName = shortName;
            this.Color = color;
            this.BackColor = backColor;

            Register(this);
        }

        public string Name { get; private set; }

        public string ShortName { get; private set; }

        internal ColorCode Color { get; private set; }

        internal ColorCode BackColor { get; private set; }

        public int Priority { get; private set; }

        private static void Register(LogLevel level)
        {
            if (m_lookup == null)
            {
                m_lookup = new Dictionary<string, LogLevel>();
            }
            m_lookup[level.ShortName] = level;
        }

        internal static LogLevel Find(string name)
        {
            if (name != null && m_lookup != null)
            {
                LogLevel level;
                if (m_lookup.TryGetValue(name, out level))
                {
                    return level;
                }
            }

            return null;
        }

        internal static IList<LogLevel> ListLevels()
        {
            return ListLevels(ReusableLists.NextAutoRecycleList<LogLevel>());
        }

        internal static IList<LogLevel> ListLevels(IList<LogLevel> outList)
        {
            foreach (LogLevel level in m_lookup.Values)
            {
                outList.Add(level);
            }

            return outList;
        }
    }

    struct LogEntry
    {
        public LogLevel level;
        public Tag tag;
        public string message;
        public string[] tokens;
    }

    public delegate void LogDelegate(LogLevel level, Tag tag, string message, string stackTrace);

    public static class Log
    {
        private static List<LogDelegate> logDelegates;
        private static Dictionary<LogType, LogLevel> m_logLevelLookup;

        static Log()
        {
            logDelegates = new List<LogDelegate>(1);

            m_logLevelLookup = new Dictionary<LogType, LogLevel>();
            m_logLevelLookup[LogType.Assert] = LogLevel.Exception;
            m_logLevelLookup[LogType.Exception] = LogLevel.Exception;
            m_logLevelLookup[LogType.Error] = LogLevel.Error;
            m_logLevelLookup[LogType.Log] = LogLevel.Debug;
            m_logLevelLookup[LogType.Warning] = LogLevel.Warn;

            Level = LogLevel.Verbose;
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void Initialize()
        {
            #if UNITY_4_7 || UNITY_4_6 || UNITY_4_5 || UNITY_4_4 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4
            UnityEngine.Application.RegisterLogCallback(UnityLogCallbackHandler);
            #else
            UnityEngine.Application.logMessageReceived += UnityLogCallbackHandler;
            #endif
        }

        #region Levels

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void error(Exception e)
        {
            if (e != null)
            {
                string message = StringUtils.TryFormat("{0}: {1}", e.GetType().Name, e.Message);
                LogMessage(null, LogLevel.Exception, message, e.StackTrace);
            }
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void error(Exception e, string format, params object[] args)
        {
            string message = StringUtils.TryFormat(format, args);

            if (e != null)
            {
                string fullMessage = StringUtils.TryFormat("{0}: {1} ({2})", e.GetType().Name, e.Message, message);
                LogMessage(null, LogLevel.Exception, fullMessage, e.StackTrace);
            }
            else
            {
                LogMessage(null, LogLevel.Exception, message);
            }
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(object arg)
        {
            if (ShouldLogLevel(LogLevel.Verbose))
                LogMessage(null, LogLevel.Verbose, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(Tag tag, object arg)
        {
            if (ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(bool condition, Tag tag, object arg)
        {
            if (condition && ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0>(string format, A0 arg0)
        {
            if (ShouldLogLevel(LogLevel.Verbose))
                LogMessage(null, LogLevel.Verbose, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1>(string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(LogLevel.Verbose))
                LogMessage(null, LogLevel.Verbose, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1, A2>(string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(LogLevel.Verbose))
                LogMessage(null, LogLevel.Verbose, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(string format, params object[] args)
        {
            if (ShouldLogLevel(LogLevel.Verbose))
                LogMessage(null, LogLevel.Verbose, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0>(Tag tag, string format, A0 arg0)
        {
            if (ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1>(Tag tag, string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1, A2>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1, A2, A3>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
        {
            if (ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, StringUtils.TryFormat(format, arg0, arg1, arg2, arg3));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1, A2, A3, A4>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            if (ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, StringUtils.TryFormat(format, arg0, arg1, arg2, arg3, arg4));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(Tag tag, string format, params object[] args)
        {
            if (ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0>(bool condition, Tag tag, string format, A0 arg0)
        {
            if (condition && ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1>(bool condition, Tag tag, string format, A0 arg0, A1 arg1)
        {
            if (condition && ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1, A2>(bool condition, Tag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (condition && ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(bool condition, Tag tag, string format, params object[] args)
        {
            if (condition && ShouldLogLevel(LogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Verbose, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(object arg)
        {
            if (ShouldLogLevel(LogLevel.Debug))
                LogMessage(null, LogLevel.Debug, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(Tag tag, object arg)
        {
            if (ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(bool condition, Tag tag, object arg)
        {
            if (condition && ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0>(string format, A0 arg0)
        {
            if (ShouldLogLevel(LogLevel.Debug))
                LogMessage(null, LogLevel.Debug, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1>(string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(LogLevel.Debug))
                LogMessage(null, LogLevel.Debug, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1, A2>(string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(LogLevel.Debug))
                LogMessage(null, LogLevel.Debug, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(string format, params object[] args)
        {
            if (ShouldLogLevel(LogLevel.Debug))
                LogMessage(null, LogLevel.Debug, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0>(Tag tag, string format, A0 arg0)
        {
            if (ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1>(Tag tag, string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1, A2>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1, A2, A3>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
        {
            if (ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, StringUtils.TryFormat(format, arg0, arg1, arg2, arg3));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1, A2, A3, A4>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            if (ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, StringUtils.TryFormat(format, arg0, arg1, arg2, arg3, arg4));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(Tag tag, string format, params object[] args)
        {
            if (ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0>(bool condition, Tag tag, string format, A0 arg0)
        {
            if (condition && ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1>(bool condition, Tag tag, string format, A0 arg0, A1 arg1)
        {
            if (condition && ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1, A2>(bool condition, Tag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (condition && ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(bool condition, Tag tag, string format, params object[] args)
        {
            if (condition && ShouldLogLevel(LogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Debug, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(object arg)
        {
            if (ShouldLogLevel(LogLevel.Info))
                LogMessage(null, LogLevel.Info, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(Tag tag, object arg)
        {
            if (ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(bool condition, Tag tag, object arg)
        {
            if (condition && ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0>(string format, A0 arg0)
        {
            if (ShouldLogLevel(LogLevel.Info))
                LogMessage(null, LogLevel.Info, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1>(string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(LogLevel.Info))
                LogMessage(null, LogLevel.Info, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1, A2>(string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(LogLevel.Info))
                LogMessage(null, LogLevel.Info, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(string format, params object[] args)
        {
            if (ShouldLogLevel(LogLevel.Info))
                LogMessage(null, LogLevel.Info, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0>(Tag tag, string format, A0 arg0)
        {
            if (ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1>(Tag tag, string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1, A2>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1, A2, A3>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
        {
            if (ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, StringUtils.TryFormat(format, arg0, arg1, arg2, arg3));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1, A2, A3, A4>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            if (ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, StringUtils.TryFormat(format, arg0, arg1, arg2, arg3, arg4));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(Tag tag, string format, params object[] args)
        {
            if (ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0>(bool condition, Tag tag, string format, A0 arg0)
        {
            if (condition && ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1>(bool condition, Tag tag, string format, A0 arg0, A1 arg1)
        {
            if (condition && ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1, A2>(bool condition, Tag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (condition && ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(bool condition, Tag tag, string format, params object[] args)
        {
            if (condition && ShouldLogLevel(LogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Info, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(object arg)
        {
            if (ShouldLogLevel(LogLevel.Warn))
                LogMessage(null, LogLevel.Warn, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(Tag tag, object arg)
        {
            if (ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(bool condition, Tag tag, object arg)
        {
            if (condition && ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0>(string format, A0 arg0)
        {
            if (ShouldLogLevel(LogLevel.Warn))
                LogMessage(null, LogLevel.Warn, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1>(string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(LogLevel.Warn))
                LogMessage(null, LogLevel.Warn, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1, A2>(string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(LogLevel.Warn))
                LogMessage(null, LogLevel.Warn, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(string format, params object[] args)
        {
            if (ShouldLogLevel(LogLevel.Warn))
                LogMessage(null, LogLevel.Warn, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0>(Tag tag, string format, A0 arg0)
        {
            if (ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1>(Tag tag, string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1, A2>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1, A2, A3>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
        {
            if (ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, StringUtils.TryFormat(format, arg0, arg1, arg2, arg3));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1, A2, A3, A4>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            if (ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, StringUtils.TryFormat(format, arg0, arg1, arg2, arg3, arg4));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(Tag tag, string format, params object[] args)
        {
            if (ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0>(bool condition, Tag tag, string format, A0 arg0)
        {
            if (condition && ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1>(bool condition, Tag tag, string format, A0 arg0, A1 arg1)
        {
            if (condition && ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1, A2>(bool condition, Tag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (condition && ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(bool condition, Tag tag, string format, params object[] args)
        {
            if (condition && ShouldLogLevel(LogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Warn, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(object arg)
        {
            if (ShouldLogLevel(LogLevel.Error))
                LogMessage(null, LogLevel.Error, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(Tag tag, object arg)
        {
            if (ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(bool condition, Tag tag, object arg)
        {
            if (condition && ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0>(string format, A0 arg0)
        {
            if (ShouldLogLevel(LogLevel.Error))
                LogMessage(null, LogLevel.Error, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1>(string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(LogLevel.Error))
                LogMessage(null, LogLevel.Error, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1, A2>(string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(LogLevel.Error))
                LogMessage(null, LogLevel.Error, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(string format, params object[] args)
        {
            if (ShouldLogLevel(LogLevel.Error))
                LogMessage(null, LogLevel.Error, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0>(Tag tag, string format, A0 arg0)
        {
            if (ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1>(Tag tag, string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1, A2>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1, A2, A3>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
        {
            if (ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, StringUtils.TryFormat(format, arg0, arg1, arg2, arg3));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1, A2, A3, A4>(Tag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            if (ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, StringUtils.TryFormat(format, arg0, arg1, arg2, arg3, arg4));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(Tag tag, string format, params object[] args)
        {
            if (ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, StringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0>(bool condition, Tag tag, string format, A0 arg0)
        {
            if (condition && ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, StringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1>(bool condition, Tag tag, string format, A0 arg0, A1 arg1)
        {
            if (condition && ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, StringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1, A2>(bool condition, Tag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (condition && ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, StringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(bool condition, Tag tag, string format, params object[] args)
        {
            if (condition && ShouldLogLevel(LogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, LogLevel.Error, StringUtils.TryFormat(format, args));
        }

        #endregion

        internal static void LogMessage(Tag tag, LogLevel level, string message)
        {
            LogMessage(tag, level, message, StackTrace.ExtractStackTrace(3));
        }

        internal static void LogMessage(Tag tag, LogLevel level, string message, string stackTrace)
        {
            for (int i = 0; i < logDelegates.Count; ++i)
            {
                logDelegates[i](level, tag, message, stackTrace);
            }
        }

        private static bool ShouldLogLevel(LogLevel level)
        {
            return level == null || Level != null && level.Priority >= Level.Priority;
        }

        private static bool ShouldLogTag(Tag tag)
        {
            return tag == null || tag.Enabled;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Log delegates

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void AddLogDelegate(LogDelegate logDelegate)
        {
            if (!logDelegates.Contains(logDelegate))
            {
                logDelegates.Add(logDelegate);
            }
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void RemoveLogDelegate(LogDelegate logDelegate)
        {
            logDelegates.Remove(logDelegate);
        }

        private static void UnityLogCallbackHandler(string logString, string stackTrace, LogType type)
        {
            LogMessage(null, ToLogLevel(type), logString, stackTrace);
        }

        private static LogLevel ToLogLevel(LogType type)
        {
            LogLevel level;
            if (m_logLevelLookup.TryGetValue(type, out level))
            {
                return level;
            }

            return LogLevel.Debug;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public static LogLevel Level { get; set; }

        #endregion
    }
}
