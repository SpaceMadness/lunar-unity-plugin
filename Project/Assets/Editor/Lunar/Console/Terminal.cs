using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    class Terminal : AbstractConsole, ICCommandDelegate
    {
        public Terminal(int capacity)
            : base(capacity)
        {
            History = new TerminalHistory(100);
        }

        public virtual void Add(string line)
        {
            Add(new ConsoleViewCellEntry(line));
        }
        
        public virtual void Add(string[] lines)
        {
            Add(new ConsoleViewCellEntry(lines));
        }

        public virtual void Add(Exception e, string message)
        {
            
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Auto Complete

        public string DoAutoComplete(string text, bool isDoubleTab)
        {
            // TODO: add unit tests

            ReusableList<string> tokensList = ReusableLists.NextAutoRecycleList<string>();
            CommandTokenizer.Tokenize(text, tokensList);

            if (tokensList.Count == 1)
            {
                string newText = DoAutoComplete(text, tokensList[0], isDoubleTab);
                if (newText != null && newText != text)
                {
                    return newText;
                }
            }

            if (tokensList.Count > 0)
            {
                return DoAutoComplete(text, tokensList, isDoubleTab);
            }

            return DoAutoCompleteNoToken(text, isDoubleTab);
        }

        private string DoAutoComplete(string text, string token, bool isDoubleTab)
        {
            IList<CCommand> suggestedCommands = CRegistery.ListCommands(token);
            if (suggestedCommands.Count == 1)
            {
                return suggestedCommands[0].Name + " ";
            }

            if (suggestedCommands.Count > 1)
            {
                if (isDoubleTab)
                {
                    string[] names = new string[suggestedCommands.Count];
                    for (int i = 0; i < suggestedCommands.Count; ++i)
                    {
                        CCommand cmd = suggestedCommands[i];
                        ColorCode color = cmd is CVarCommand ? ColorCode.TableVar : cmd.ColorCode;
                        names[i] = StringUtils.C(cmd.Name, color);
                    }

                    Array.Sort(names);
                    Add(CCommand.Prompt(token));
                    Add(names);
                }

                return GetSuggestedText(token, suggestedCommands);
            }

            return text;
        }

        private string DoAutoComplete(string text, IList<string> tokensList, bool isDoubleTab)
        {
            string name = tokensList[0];
            CCommand command = CRegistery.FindCommand(name);
            if (command != null)
            {
                command.Delegate = this; // FIXME: split ICCommandDelegate interface
                string newText = command.AutoComplete(text, tokensList, isDoubleTab);
                command.Delegate = null;
                if (newText != null)
                {
                    return newText;
                }
            }

            return text;
        }

        private string DoAutoCompleteNoToken(string text, bool isDoubleTab)
        {
            if (isDoubleTab)
            {
                IList<CCommand> commands = CRegistery.ListCommands();
                string[] names = new string[commands.Count];
                for (int i = 0; i < commands.Count; ++i)
                {
                    CCommand cmd = commands[i];
                    ColorCode color = cmd is CVarCommand ? ColorCode.TableVar : cmd.ColorCode;
                    names[i] = StringUtils.C(cmd.Name, color);
                }

                Add(names);
            }

            return text;
        }

        private static string GetSuggestedText(string token, IList<CCommand> commands)
        {
            string firstCommandName = commands[0].Name;

            if (firstCommandName.Length > token.Length)
            {
                StringBuilder suggestedToken = new StringBuilder(token);
                for (int charIndex = token.Length; charIndex < firstCommandName.Length; ++charIndex)
                {
                    char chr = firstCommandName[charIndex];
                    char chrLower = char.ToLower(chr);
                    for (int commandIndex = 1; commandIndex < commands.Count; ++commandIndex)
                    {
                        string otherCommandName = commands[commandIndex].Name;
                        if (char.ToLower(otherCommandName[charIndex]) != chrLower)
                        {
                            return suggestedToken.ToString();
                        }
                    }
                    suggestedToken.Append(chr);
                }

                return suggestedToken.ToString();
            }

            return token;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region ICCommandDelegate

        public void LogTerminal(string message)
        {
            Add(message);
        }

        public void LogTerminal(string[] table)
        {
            Add(table);
        }

        public void LogTerminal(Exception e, string message)
        {
            
        }

        public void ClearTerminal()
        {
            throw new NotImplementedException();
        }

        public bool ExecuteCommandLine(string commandLine, bool manual = false)
        {
            throw new NotImplementedException();
        }

        public void PostNotification(CCommand cmd, string name, params object[] data)
        {
            throw new NotImplementedException();
        }

        public bool IsPromptEnabled
        {
            get { return true; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public TerminalHistory History { get; private set; }

        #endregion
    }

    class FormattedTerminal : Terminal
    {
        public FormattedTerminal(int capacity)
            : base(capacity)
        {
        }

        #region Inheritance

        public override void Add(string line)
        {
            base.Add(FormatLine(line));
        }

        public override void Add(string[] lines)
        {
            base.Add(FormatLines(lines));
        }

        #endregion

        #region Lines format
        
        private string FormatLine(string line)
        {
            return EditorSkin.SetColors(line);
        }
        
        private string[] FormatLines(string[] lines)
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                lines[i] = EditorSkin.SetColors(lines[i]);
            }
            return lines;
        }

        #endregion
    }

    class TerminalHistory
    {
        private CycleArray<string> m_entries;
        private int m_currentIndex;

        public TerminalHistory(int capacity)
        {
            m_entries = new CycleArray<string>(capacity);
            m_currentIndex = -1;
        }

        public string this[int index]
        {
            get
            {
                int entryIndex = m_entries.HeadIndex + index;
                return m_entries[entryIndex];
            }
        }

        public void Push(string line)
        {
            if (m_entries.Length == 0 || m_entries[m_entries.Length - 1] != line)
            {
                m_entries.Add(line);
            }

            Reset();
        }

        public void Reset()
        {
            m_currentIndex = m_entries.Length;
        }

        public string Next()
        {
            int nextIndex = m_currentIndex + 1;
            if (nextIndex < m_entries.Length)
            {
                m_currentIndex = nextIndex;
                return m_entries[m_currentIndex];
            }

            return null;
        }

        public string Prev()
        {
            int prevIndex = m_currentIndex - 1;
            if (prevIndex >= m_entries.HeadIndex)
            {
                m_currentIndex = prevIndex;
                return m_entries[m_currentIndex];
            }
            
            return null;
        }

        public void Clear()
        {
            m_currentIndex = -1;
            m_entries.Clear();
        }

        public int Count
        {
            get { return m_entries.RealLength; }
        }
    }
}