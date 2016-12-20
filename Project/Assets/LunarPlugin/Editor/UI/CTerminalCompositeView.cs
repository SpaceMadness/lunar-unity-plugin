//
//  TerminalCompositeView.cs
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
using UnityEditor;

using System;
using System.Text;
using System.Collections.Generic;

using LunarPluginInternal;

namespace LunarEditor
{
    interface ITerminalCompositeViewDelegate
    {
        void ExecCommand(string commandLine);
        Terminal Terminal { get; }
    }

    class TerminalCompositeView : View
    {
        private ConsoleView m_consoleView;
        private TextField m_commandField;
        private ToolBarLabel m_infoLabel;

        private ITerminalCompositeViewDelegate m_delegate;

        private float m_lastTabTimestamp;
        private string m_lastUserInput;

        public TerminalCompositeView(ITerminalCompositeViewDelegate del, float width, float height)
            : base(width, height)
        {
            if (del == null)
            {
                throw new NullReferenceException("Delegate is null");
            }

            m_delegate = del;

            CreateUI();
        }

        private void CreateUI()
        {
            this.AutoresizeMask = ViewAutoresizing.FlexibleWidth | ViewAutoresizing.FlexibleHeight;

            ToolBar toolbar = new ToolBar(this.Width);
            toolbar.Width = this.Width;

            toolbar.AddButton("Clear", delegate(Button button)
            {
                Terminal.Clear();
            });

            // copy to clipboard
            toolbar.AddButton("Copy", delegate(Button button)
                {
                    string text = GetText();
                    Editor.CopyToClipboard(text);
                });

            // save to file
            toolbar.AddButton("Save", delegate(Button button)
                {
                    string title = "Console log";
                    string directory = FileUtils.DataPath;
                    string defaultName = string.Format("console");
                    string filename = Editor.SaveFilePanel(title, directory, defaultName, "log");
                    if (!string.IsNullOrEmpty(filename))
                    {
                        string text = GetText();
                        FileUtils.Write(filename, text);
                    }
                });

            m_infoLabel = toolbar.AddLabel("");

            toolbar.AddFlexibleSpace();

            AddSubview(toolbar);

            m_commandField = new TextField();
            m_commandField.Width = Width;
            AddSubview(m_commandField);
            m_commandField.AlignX(View.AlignCenter);
            m_commandField.AlignBottom(0);
            m_commandField.AutoresizeMask = ViewAutoresizing.FlexibleTopMargin | ViewAutoresizing.FlexibleWidth;

            m_commandField.TextKeyDelegate = delegate(TextField tf, KeyCode code, bool pressed)
            {
                if (pressed)
                {
                    switch (code)
                    {
                        case KeyCode.Return:
                        case KeyCode.KeypadEnter:
                        {
                            string commandLine = tf.Text.Trim();
                            if (commandLine.Length > 0)
                            {
                                HistoryPush(commandLine);
                                ExecCommand(commandLine);
                            }
                            tf.Text = "";
                            HistoryReset();

                            return true;
                        }

                        case KeyCode.Tab:
                        {
                            string line = Terminal.DoAutoComplete(tf.Text, tf.CaretPos, IsDoubleTab());
                            if (line != null)
                            {
                                tf.Text = line;
                            }
                            m_lastTabTimestamp = Time.realtimeSinceStartup;
                            return true;
                        }

                        case KeyCode.Escape:
                        {
                            tf.Text = "";
                            HistoryReset();
                            return true;
                        }

                        case KeyCode.C:
                        {
                            if (tf.IsCtrlPressed)
                            {
                                tf.Text = "";
                                HistoryReset();
                                return true;
                            }
                            break;
                        }

                        case KeyCode.K:
                        {
                            if (tf.IsCtrlPressed)
                            {
                                Terminal.Clear();
                                return true;
                            }
                            break;
                        }

                        case KeyCode.DownArrow:
                        {
                            if (HistoryNext(tf))
                            {
                                return true;
                            }

                            if (m_lastUserInput != null)
                            {
                                tf.Text = m_lastUserInput;
                                HistoryReset();
                                return true;
                            }

                            return true;
                        }

                        case KeyCode.UpArrow:
                        {
                            // keep user input to restore it 
                            if (m_lastUserInput == null)
                            {
                                m_lastUserInput = tf.Text;
                            }

                            if (HistoryPrev(tf))
                            {
                                return true;
                            }

                            return true;
                        }
                    }
                }

                return false;
            };

            m_commandField.TextChangedDelegate = delegate(TextField field) {
                HistoryReset();
            };

            m_consoleView = new ConsoleView(Terminal, m_commandField.Width, this.Height - (toolbar.Height + m_commandField.Height));
            m_consoleView.Y = toolbar.Bottom;
            m_consoleView.IsScrollLocked = true;
            m_consoleView.AutoresizeMask = ViewAutoresizing.FlexibleWidth | ViewAutoresizing.FlexibleHeight;
            AddSubview(m_consoleView);

            m_lastUserInput = null;
        }

        private string GetText()
        {
            return m_consoleView.GetText();
        }

        private bool IsDoubleTab()
        {
            return Time.realtimeSinceStartup - m_lastTabTimestamp < 0.5f;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region History

        private void HistoryPush(string commandLine)
        {
            History.Push(commandLine);
        }

        private bool HistoryNext(TextField tf)
        {
            string line = History.Next();
            if (line != null)
            {
                tf.Text = line;
                return true;
            }

            return false;
        }

        private bool HistoryPrev(TextField tf)
        {
            string line = History.Prev();
            if (line != null)
            {
                tf.Text = line;
                return true;
            }

            return false;
        }

        private void HistoryReset()
        {
            History.Reset();
            m_lastUserInput = null;
        }

        #endregion

        public void SetInputEnabled(bool flag)
        {
            if (m_commandField != null)
            {
                m_commandField.IsEnabled = flag;
                Repaint();
            }
        }

        private void ExecCommand(string commandLine)
        {
            m_delegate.ExecCommand(commandLine);
        }

        #region Properties

        private Terminal Terminal
        {
            get { return m_delegate.Terminal; }
        }

        private TerminalHistory History
        {
            get { return Terminal.History; }
        }

        public string InfoString
        {
            get { return m_infoLabel != null ? m_infoLabel.Text : null; }
            set
            {
                if (m_infoLabel != null)
                {
                    m_infoLabel.Text = value;
                }
            }
        }

        #endregion
    }
}

