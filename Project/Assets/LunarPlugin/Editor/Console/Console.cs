//
//  Console.cs
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
using System.Text;

using UnityEngine;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class Console : AbstractConsole
    {
        public Console(int capacity)
            : base(capacity)
        {
        }

        //////////////////////////////////////////////////////////////////////////////

        public virtual void Add(string text)
        {
            Add(new ConsoleViewCellEntry(text));
        }

        public virtual void Add(LogLevel level, Tag tag, string text, string stackTrace)
        {
            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(text);
            entry.level = level;
            entry.tag = tag;
            entry.stackTrace = stackTrace;
            Add(entry);
        }
        
        public virtual void Add(LogLevel level, Tag tag, string[] table, string stackTrace)
        {
            ConsoleViewCellEntry entry = new ConsoleViewCellEntry(table);
            entry.level = level;
            entry.tag = tag;
            entry.stackTrace = stackTrace;
            Add(entry);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Log delegate

        public void RegisterLogDelegate()
        {
            Log.AddLogDelegate(OnLogMessage);
        }
        
        private void OnLogMessage(LogLevel level, Tag tag, string message, string stackTrace)
        {
            Add(level, tag, message, stackTrace);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IDestroyable

        public override void Destroy()
        {
            Log.RemoveLogDelegate(OnLogMessage);
            base.Destroy();
        }

        #endregion
    }

    internal class FormattedConsole : Console
    {
        public FormattedConsole(int capacity)
            : base(capacity)
        {
        }

        public override void Add(string text)
        {
            base.Add(FormatLine(text));
        }

        public override void Add(LogLevel level, Tag tag, string text, string stackTrace)
        {
            string formattedText = FormatLine(text, level, tag, stackTrace);
            base.Add(level, tag, formattedText, stackTrace);
        }

        public override void Add(LogLevel level, Tag tag, string[] table, string stackTrace)
        {
            string[] formattedTable = FormatTable(table, level, tag, stackTrace);
            base.Add(level, tag, formattedTable, stackTrace);
        }

        #region Format
        
        private string FormatLine(string line, LogLevel level, Tag tag, string stackTrace)
        {
            RecyclableStringBuilder lineBuffer = StringBuilderPool.NextBuilder();

            string coloredLine = EditorSkin.SetColors(line);
            
            string filename = EditorStackTrace.ExtractFileName(stackTrace);
            if (level != null)
            {
                lineBuffer.Append("[");
                lineBuffer.Append(level.ShortName);
                lineBuffer.Append("]: ");
            }
            
            if (filename != null)
            {
                lineBuffer.Append(StringUtils.C("[" + filename + "]: ", EditorSkin.GetColor(ColorCode.Plain)));
            }
            
            if (tag != null)
            {
                lineBuffer.Append("[");
                lineBuffer.Append(tag.Name);
                lineBuffer.Append("]: ");
            }
            
            lineBuffer.Append(coloredLine);
            
            string result = lineBuffer.ToString();
            lineBuffer.Recycle(); // don't trash timer manager with this call

            return result;
        }
        
        private string FormatLine(string line)
        {
            return EditorSkin.SetColors(line);
        }
        
        private string[] FormatTable(string[] table, LogLevel level, Tag tag, string stackTrace)
        {
            for (int i = 0; i < table.Length; ++i)
            {
                table[i] = EditorSkin.SetColors(table[i]);
            }
            return table;
        }
        
        #endregion
    }
}