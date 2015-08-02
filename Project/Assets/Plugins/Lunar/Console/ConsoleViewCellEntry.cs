using UnityEngine;

using System;
using System.Text;
using System.Collections.Generic;

using LunarPlugin;

namespace LunarPluginInternal
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

    public struct ConsoleViewCellEntry
    {
        internal enum Flags : byte
        {
            Plain     = 1 << 1,
            Table     = 1 << 2
        }
        
        private static readonly string kColSeparator = "  ";
        
        public string value;
        public object data;
        
        public float width;
        public float height;
        
        public Tag tag;
        public LogLevel level;
        public string stackTrace;
        
        internal Flags flags;
        
        public ConsoleViewCellEntry(string line, float width = 0, float height = 0)
        {
            this.value = line;
            this.width = width;
            this.height = height;
            this.flags = Flags.Plain;
            
            this.data = null;
            this.tag = null;
            this.level = null;
            this.stackTrace = null;
        }
        
        public ConsoleViewCellEntry(string[] lines, float width = 0, float height = 0)
        {
            if (lines.Length == 1)
            {
                this.value = lines[0];
                this.data = null;
                this.flags = Flags.Plain;
            }
            else
            {
                this.data = lines;
                this.value = null;
                this.flags = Flags.Table;
            }
            this.width = width;
            this.height = height;
            
            this.tag = null;
            this.level = null;
            this.stackTrace = null;
        }
        
        public ConsoleViewCellEntry(Exception e, string message, float width = 0, float height = 0)
        {
            this.value = message != null ? StringUtils.TryFormat("{0} ({1})", message, e.Message) : e.Message;
            this.width = width;
            this.height = height;
            this.flags = Flags.Plain;
            
            this.data = null;
            this.tag = null;
            this.level = LogLevel.Exception;
            this.stackTrace = e.StackTrace;
        }
        
        internal void Layout(ITextMeasure measure, float maxWidth)
        {
            Layout(measure, maxWidth, maxWidth);
        }
        
        internal void Layout(ITextMeasure measure, float contentWidth, float maxWidth)
        {
            if (this.IsPlain)
            {
                if (this.level == LogLevel.Exception)
                {
                    StackTraceLine[] stackLines = this.data as StackTraceLine[];
                    if (stackLines == null && this.stackTrace != null)
                    {
                        /*
                        stackLines = EditorStackTrace.ParseStackTrace(this.stackTrace);
                        this.data = stackLines;
                        */
                        this.data = new string[0]; // FIXME
                    }
                    
                    LayoutException(measure, stackLines, maxWidth);
                }
                else
                {
                    LayoutPlain(measure, maxWidth);
                }
            }
            else if (this.IsTable)
            {
                string[] table = this.Table;
                Assert.IsNotNull(table);
                
                LayoutTable(measure, table, contentWidth, maxWidth);
            }
            else
            {
                throw new NotImplementedException("Unexpected entry type");
            }
            
        }
        
        private void LayoutPlain(ITextMeasure measure, float maxWidth)
        {
            this.width = maxWidth;
            this.height = measure.CalcHeight(value, maxWidth);
        }
        
        private void LayoutTable(ITextMeasure measure, string[] table, float contentWidth, float maxWidth)
        {
            int maxLength = 0;
            string longestString = null;
            
            for (int i = 0; i < table.Length; ++i)
            {
                string str = table[i];
                int len = StringUtils.Strlen(str);
                if (len > maxLength)
                {
                    maxLength = len;
                    longestString = str;
                }
            }
            
            int colLength = maxLength + kColSeparator.Length;
            
            Vector2 colSize = measure.CalcSize(kColSeparator + longestString); 
            float colWidth = colSize.x;
            
            int numCols = Mathf.Max(1, (int)(contentWidth / colWidth));
            int numRows = table.Length / numCols + (table.Length % numCols != 0 ? 1 : 0);
            
            StringBuilder buffer = new StringBuilder(colLength * table.Length);
            
            for (int row = 0; row < numRows; ++row)
            {
                int appendSpacing = 0;
                for (int col = 0; col < numCols; ++col)
                {
                    int index = col * numRows + row;
                    
                    if (index > table.Length - 1)
                    {
                        break;
                    }
                    
                    buffer.Append(' ', appendSpacing);
                    if (col > 0)
                    {
                        buffer.Append(kColSeparator);
                    }
                    
                    string str = table[index];
                    if (str != null)
                    {
                        buffer.Append(str);
                        appendSpacing = maxLength - str.Length;
                    }
                    else
                    {
                        appendSpacing = maxLength;
                    }
                }
                
                if (row < numRows - 1)
                {
                    buffer.Append('\n');
                }
            }
            
            this.value = buffer.ToString();
            
            Vector2 size = measure.CalcSize(this.value);
            this.width = maxWidth;
            this.height = size.y;
        }
        
        private void LayoutException(ITextMeasure measure, StackTraceLine[] stackLines, float maxWidth)
        {
            float nextX = 0.0f;
            float totalHeight = measure.CalcHeight(value, maxWidth);
            
            if (stackLines != null)
            {
                for (int i = 0; i < stackLines.Length; ++i)
                {
                    float lineHeight = measure.CalcHeight(stackLines[i].line, maxWidth);
                    stackLines[i].frame = new Rect(nextX, totalHeight, maxWidth, lineHeight);
                    
                    if (stackLines[i].sourcePathStart != -1)
                    {
                        /*
                        FIXME:
                        GUIStyleTextMeasure styleMeasure = measure as GUIStyleTextMeasure;
                        if (styleMeasure != null)
                        {
                            ResolveSourceLink(styleMeasure, ref stackLines[i]);
                        }
                        */
                    }
                    
                    totalHeight += lineHeight;
                }
            }
            
            this.width = maxWidth;
            this.height = totalHeight;
        }
        
        private static void ResolveSourceLink(ITextMeasure measure, ref StackTraceLine stackLine)
        {
            /*
            FIXME
            Color color = SkinColors.GetColor(stackLine.sourcePathExists ? ColorCode.Link : ColorCode.LinkInnactive);
            
            int sourceStart = stackLine.sourcePathStart;
            int sourceEnd = stackLine.sourcePathEnd;
            
            GUIStyle style = measure.Style;
            GUIContent content = new GUIContent(stackLine.line);
            
            float startPosX = style.GetCursorPixelPosition(stackLine.frame, content, sourceStart).x - 1;
            float endPosX = style.GetCursorPixelPosition(stackLine.frame, content, sourceEnd).x + 1;
            
            stackLine.sourceFrame = new Rect(startPosX, stackLine.frame.y, endPosX - startPosX, stackLine.frame.height);
            stackLine.line = StringUtils.C(stackLine.line, color, sourceStart, sourceEnd);
            */
        }
        
        private bool HasFlag(Flags flag)
        {
            return (flags & flag) != 0;
        }
        
        public bool IsPlain
        {
            get { return HasFlag(Flags.Plain); }
        }
        
        public bool IsTable
        {
            get { return HasFlag(Flags.Table); }
        }
        
        public string[] Table
        {
            get { return data as string[]; }
        }
    }
}
