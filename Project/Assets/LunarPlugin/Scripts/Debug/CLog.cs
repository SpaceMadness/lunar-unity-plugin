//
//  CLog.cs
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
    public class CTag : IComparable<CTag>
    {
        private static Dictionary<string, CTag> m_registry;
        private static int m_nextOrdinal;

        public CTag(string name)
            : this(name, true)
        {
        }

        public CTag(string name, Color color)
            : this(name, color, Color.clear, true)
        {
        }

        public CTag(string name, bool enabled)
            : this(name, Color.clear, Color.clear, enabled)
        {
        }

        public CTag(string name, Color color, Color backColor)
            : this(name, color, backColor, true)
        {
        }

        public CTag(string name, Color color, Color backColor, bool enabled)
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

        public int CompareTo(CTag other)
        {
            return Name.CompareTo(other.Name);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public override bool Equals(object obj)
        {
            CTag tag = obj as CTag;
            return tag != null && tag.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Registry

        private static void Register(CTag tag)
        {
            if (m_registry == null)
            {
                m_registry = new Dictionary<string, CTag>();
            }
            m_registry[tag.Name] = tag;
        }

        internal static CTag Find(string name)
        {
            if (name != null && m_registry != null)
            {
                CTag tag;
                if (m_registry.TryGetValue(name, out tag))
                {
                    return tag;
                }
            }

            return null;
        }

        internal static IList<CTag> ListTags()
        {
            return ListTags(new List<CTag>());
        }

        internal static IList<CTag> ListTags(IList<CTag> outList)
        {
            foreach (CTag tag in m_registry.Values)
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

    public sealed class CLogLevel
    {
        public static readonly CLogLevel Verbose   = new CLogLevel(0, "verbose",   "V", CColorCode.LevelVerbose);
        public static readonly CLogLevel Debug     = new CLogLevel(1, "debug",     "D", CColorCode.LevelDebug);
        public static readonly CLogLevel Info      = new CLogLevel(2, "info",      "I", CColorCode.LevelInfo);
        public static readonly CLogLevel Warn      = new CLogLevel(3, "warning",   "W", CColorCode.LevelWarning);
        public static readonly CLogLevel Error     = new CLogLevel(4, "error",     "E", CColorCode.LevelError);
        public static readonly CLogLevel Exception = new CLogLevel(5, "exception", "C", CColorCode.LevelCritical);

        private static Dictionary<string, CLogLevel> m_lookup;

        private CLogLevel(int priority, string name, string shortName, CColorCode color = CColorCode.Clear, CColorCode backColor = CColorCode.Clear)
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

        internal CColorCode Color { get; private set; }

        internal CColorCode BackColor { get; private set; }

        public int Priority { get; private set; }

        private static void Register(CLogLevel level)
        {
            if (m_lookup == null)
            {
                m_lookup = new Dictionary<string, CLogLevel>();
            }
            m_lookup[level.ShortName] = level;
        }

        internal static CLogLevel Find(string name)
        {
            if (name != null && m_lookup != null)
            {
                CLogLevel level;
                if (m_lookup.TryGetValue(name, out level))
                {
                    return level;
                }
            }

            return null;
        }

        internal static IList<CLogLevel> ListLevels()
        {
            return ListLevels(new List<CLogLevel>());
        }

        internal static IList<CLogLevel> ListLevels(IList<CLogLevel> outList)
        {
            foreach (CLogLevel level in m_lookup.Values)
            {
                outList.Add(level);
            }

            return outList;
        }
    }

    struct CLogEntry
    {
        public CLogLevel level;
        public CTag tag;
        public string message;
        public string[] tokens;
    }

    public delegate void CLogDelegate(CLogLevel level, CTag tag, string message, string stackTrace);

    public static class CLog
    {
        private static List<CLogDelegate> logDelegates;
        private static Dictionary<LogType, CLogLevel> m_logLevelLookup;

        static CLog()
        {
            logDelegates = new List<CLogDelegate>(1);

            m_logLevelLookup = new Dictionary<LogType, CLogLevel>();
            m_logLevelLookup[LogType.Assert] = CLogLevel.Exception;
            m_logLevelLookup[LogType.Exception] = CLogLevel.Exception;
            m_logLevelLookup[LogType.Error] = CLogLevel.Error;
            m_logLevelLookup[LogType.Log] = CLogLevel.Debug;
            m_logLevelLookup[LogType.Warning] = CLogLevel.Warn;

            Level = CLogLevel.Verbose;
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
                string message = CStringUtils.TryFormat("{0}: {1}", e.GetType().Name, e.Message);
                LogMessage(null, CLogLevel.Exception, message, e.StackTrace);
            }
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void error(Exception e, string format, params object[] args)
        {
            string message = CStringUtils.TryFormat(format, args);

            if (e != null)
            {
                string fullMessage = CStringUtils.TryFormat("{0}: {1} ({2})", e.GetType().Name, e.Message, message);
                LogMessage(null, CLogLevel.Exception, fullMessage, e.StackTrace);
            }
            else
            {
                LogMessage(null, CLogLevel.Exception, message);
            }
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(object arg)
        {
            if (ShouldLogLevel(CLogLevel.Verbose))
                LogMessage(null, CLogLevel.Verbose, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(CTag tag, object arg)
        {
            if (ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(bool condition, CTag tag, object arg)
        {
            if (condition && ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0>(string format, A0 arg0)
        {
            if (ShouldLogLevel(CLogLevel.Verbose))
                LogMessage(null, CLogLevel.Verbose, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1>(string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(CLogLevel.Verbose))
                LogMessage(null, CLogLevel.Verbose, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1, A2>(string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(CLogLevel.Verbose))
                LogMessage(null, CLogLevel.Verbose, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(string format, params object[] args)
        {
            if (ShouldLogLevel(CLogLevel.Verbose))
                LogMessage(null, CLogLevel.Verbose, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0>(CTag tag, string format, A0 arg0)
        {
            if (ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1>(CTag tag, string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1, A2>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1, A2, A3>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
        {
            if (ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, CStringUtils.TryFormat(format, arg0, arg1, arg2, arg3));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1, A2, A3, A4>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            if (ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, CStringUtils.TryFormat(format, arg0, arg1, arg2, arg3, arg4));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(CTag tag, string format, params object[] args)
        {
            if (ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0>(bool condition, CTag tag, string format, A0 arg0)
        {
            if (condition && ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1>(bool condition, CTag tag, string format, A0 arg0, A1 arg1)
        {
            if (condition && ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v<A0, A1, A2>(bool condition, CTag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (condition && ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void v(bool condition, CTag tag, string format, params object[] args)
        {
            if (condition && ShouldLogLevel(CLogLevel.Verbose) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Verbose, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(object arg)
        {
            if (ShouldLogLevel(CLogLevel.Debug))
                LogMessage(null, CLogLevel.Debug, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(CTag tag, object arg)
        {
            if (ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(bool condition, CTag tag, object arg)
        {
            if (condition && ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0>(string format, A0 arg0)
        {
            if (ShouldLogLevel(CLogLevel.Debug))
                LogMessage(null, CLogLevel.Debug, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1>(string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(CLogLevel.Debug))
                LogMessage(null, CLogLevel.Debug, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1, A2>(string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(CLogLevel.Debug))
                LogMessage(null, CLogLevel.Debug, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(string format, params object[] args)
        {
            if (ShouldLogLevel(CLogLevel.Debug))
                LogMessage(null, CLogLevel.Debug, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0>(CTag tag, string format, A0 arg0)
        {
            if (ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1>(CTag tag, string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1, A2>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1, A2, A3>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
        {
            if (ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, CStringUtils.TryFormat(format, arg0, arg1, arg2, arg3));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1, A2, A3, A4>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            if (ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, CStringUtils.TryFormat(format, arg0, arg1, arg2, arg3, arg4));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(CTag tag, string format, params object[] args)
        {
            if (ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0>(bool condition, CTag tag, string format, A0 arg0)
        {
            if (condition && ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1>(bool condition, CTag tag, string format, A0 arg0, A1 arg1)
        {
            if (condition && ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d<A0, A1, A2>(bool condition, CTag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (condition && ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void d(bool condition, CTag tag, string format, params object[] args)
        {
            if (condition && ShouldLogLevel(CLogLevel.Debug) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Debug, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(object arg)
        {
            if (ShouldLogLevel(CLogLevel.Info))
                LogMessage(null, CLogLevel.Info, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(CTag tag, object arg)
        {
            if (ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(bool condition, CTag tag, object arg)
        {
            if (condition && ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0>(string format, A0 arg0)
        {
            if (ShouldLogLevel(CLogLevel.Info))
                LogMessage(null, CLogLevel.Info, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1>(string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(CLogLevel.Info))
                LogMessage(null, CLogLevel.Info, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1, A2>(string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(CLogLevel.Info))
                LogMessage(null, CLogLevel.Info, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(string format, params object[] args)
        {
            if (ShouldLogLevel(CLogLevel.Info))
                LogMessage(null, CLogLevel.Info, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0>(CTag tag, string format, A0 arg0)
        {
            if (ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1>(CTag tag, string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1, A2>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1, A2, A3>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
        {
            if (ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, CStringUtils.TryFormat(format, arg0, arg1, arg2, arg3));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1, A2, A3, A4>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            if (ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, CStringUtils.TryFormat(format, arg0, arg1, arg2, arg3, arg4));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(CTag tag, string format, params object[] args)
        {
            if (ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0>(bool condition, CTag tag, string format, A0 arg0)
        {
            if (condition && ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1>(bool condition, CTag tag, string format, A0 arg0, A1 arg1)
        {
            if (condition && ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i<A0, A1, A2>(bool condition, CTag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (condition && ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void i(bool condition, CTag tag, string format, params object[] args)
        {
            if (condition && ShouldLogLevel(CLogLevel.Info) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Info, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(object arg)
        {
            if (ShouldLogLevel(CLogLevel.Warn))
                LogMessage(null, CLogLevel.Warn, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(CTag tag, object arg)
        {
            if (ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(bool condition, CTag tag, object arg)
        {
            if (condition && ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0>(string format, A0 arg0)
        {
            if (ShouldLogLevel(CLogLevel.Warn))
                LogMessage(null, CLogLevel.Warn, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1>(string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(CLogLevel.Warn))
                LogMessage(null, CLogLevel.Warn, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1, A2>(string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(CLogLevel.Warn))
                LogMessage(null, CLogLevel.Warn, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(string format, params object[] args)
        {
            if (ShouldLogLevel(CLogLevel.Warn))
                LogMessage(null, CLogLevel.Warn, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0>(CTag tag, string format, A0 arg0)
        {
            if (ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1>(CTag tag, string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1, A2>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1, A2, A3>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
        {
            if (ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, CStringUtils.TryFormat(format, arg0, arg1, arg2, arg3));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1, A2, A3, A4>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            if (ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, CStringUtils.TryFormat(format, arg0, arg1, arg2, arg3, arg4));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(CTag tag, string format, params object[] args)
        {
            if (ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0>(bool condition, CTag tag, string format, A0 arg0)
        {
            if (condition && ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1>(bool condition, CTag tag, string format, A0 arg0, A1 arg1)
        {
            if (condition && ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w<A0, A1, A2>(bool condition, CTag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (condition && ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void w(bool condition, CTag tag, string format, params object[] args)
        {
            if (condition && ShouldLogLevel(CLogLevel.Warn) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Warn, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(object arg)
        {
            if (ShouldLogLevel(CLogLevel.Error))
                LogMessage(null, CLogLevel.Error, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(CTag tag, object arg)
        {
            if (ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(bool condition, CTag tag, object arg)
        {
            if (condition && ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, arg != null ? arg.ToString() : "");
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0>(string format, A0 arg0)
        {
            if (ShouldLogLevel(CLogLevel.Error))
                LogMessage(null, CLogLevel.Error, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1>(string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(CLogLevel.Error))
                LogMessage(null, CLogLevel.Error, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1, A2>(string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(CLogLevel.Error))
                LogMessage(null, CLogLevel.Error, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(string format, params object[] args)
        {
            if (ShouldLogLevel(CLogLevel.Error))
                LogMessage(null, CLogLevel.Error, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0>(CTag tag, string format, A0 arg0)
        {
            if (ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1>(CTag tag, string format, A0 arg0, A1 arg1)
        {
            if (ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1, A2>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1, A2, A3>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
        {
            if (ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, CStringUtils.TryFormat(format, arg0, arg1, arg2, arg3));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1, A2, A3, A4>(CTag tag, string format, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            if (ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, CStringUtils.TryFormat(format, arg0, arg1, arg2, arg3, arg4));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(CTag tag, string format, params object[] args)
        {
            if (ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, CStringUtils.TryFormat(format, args));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0>(bool condition, CTag tag, string format, A0 arg0)
        {
            if (condition && ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, CStringUtils.TryFormat(format, arg0));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1>(bool condition, CTag tag, string format, A0 arg0, A1 arg1)
        {
            if (condition && ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, CStringUtils.TryFormat(format, arg0, arg1));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e<A0, A1, A2>(bool condition, CTag tag, string format, A0 arg0, A1 arg1, A2 arg2)
        {
            if (condition && ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, CStringUtils.TryFormat(format, arg0, arg1, arg2));
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void e(bool condition, CTag tag, string format, params object[] args)
        {
            if (condition && ShouldLogLevel(CLogLevel.Error) && ShouldLogTag(tag))
                LogMessage(tag, CLogLevel.Error, CStringUtils.TryFormat(format, args));
        }

        #endregion

        internal static void LogMessage(CTag tag, CLogLevel level, string message)
        {
            LogMessage(tag, level, message, CStackTrace.ExtractStackTrace(3));
        }

        internal static void LogMessage(CTag tag, CLogLevel level, string message, string stackTrace)
        {
            for (int i = 0; i < logDelegates.Count; ++i)
            {
                logDelegates[i](level, tag, message, stackTrace);
            }
        }

        private static bool ShouldLogLevel(CLogLevel level)
        {
            return level == null || Level != null && level.Priority >= Level.Priority;
        }

        private static bool ShouldLogTag(CTag tag)
        {
            return tag == null || tag.Enabled;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Log delegates

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void AddLogDelegate(CLogDelegate logDelegate)
        {
            if (!logDelegates.Contains(logDelegate))
            {
                logDelegates.Add(logDelegate);
            }
        }

        [System.Diagnostics.Conditional("LUNAR_DEVELOPMENT")]
        public static void RemoveLogDelegate(CLogDelegate logDelegate)
        {
            logDelegates.Remove(logDelegate);
        }

        private static void UnityLogCallbackHandler(string logString, string stackTrace, LogType type)
        {
            LogMessage(null, ToLogLevel(type), logString, stackTrace);
        }

        private static CLogLevel ToLogLevel(LogType type)
        {
            CLogLevel level;
            if (m_logLevelLookup.TryGetValue(type, out level))
            {
                return level;
            }

            return CLogLevel.Debug;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public static CLogLevel Level { get; set; }

        #endregion
    }
}
