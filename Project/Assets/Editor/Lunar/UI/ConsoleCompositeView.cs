using System;
using System.Text;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using LunarPlugin;
using LunarPluginInternal;

using Event = LunarEditor.Event;

namespace LunarEditor
{
    interface IConsoleCompositeViewDelegate
    {
        Console Console { get; }
    }

    class ConsoleCompositeView : View, ITableViewScrollDelegate
    {
        private IConsoleCompositeViewDelegate m_delegate;

        private ToolBarToggle m_scrollLockButton;
        private ConsoleView m_consoleView;

        public ConsoleCompositeView(IConsoleCompositeViewDelegate del, float width, float height)
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

            ToolBar toolbar = CreateToolbar();

            m_consoleView = new ConsoleView(this.Console, Width, Height-toolbar.Height);
            m_consoleView.OverridenContentWidth = 2880;
            m_consoleView.IsScrollLocked = m_scrollLockButton.IsOn;
            m_consoleView.ScrollDelegate = this;
            m_consoleView.AutoresizeMask = ViewAutoresizing.FlexibleWidth | ViewAutoresizing.FlexibleHeight;

            AddSubview(toolbar);
            AddSubview(m_consoleView);

            ArrangeVert();

            // TODO: I don't like this part and can't remember why I did it :(
            TimerManager.ScheduleTimer(delegate()
            {
                View rootView = this.RootView;
                if (rootView != null)
                {
                    rootView.KeyDown = KeyDownHandler;
                    rootView.KeyUp = KeyUpHandler;
                }
            });
        }

        private ToolBar CreateToolbar()
        {
            ToolBar toolbar = new ToolBar(this.Width);

            // clear console
            toolbar.AddButton("Clear", delegate(Button button)
            {
                m_consoleView.Clear();
            });

            toolbar.AddSpace();

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

            toolbar.AddSpace();

            // console scroll lock
            m_scrollLockButton = toolbar.AddToggle("Scroll lock", delegate(ToggleButton button)
            {
                m_consoleView.IsScrollLocked = button.IsOn;
                if (button.IsOn)
                {
                    m_consoleView.ScrollToBottom();
                }
            });
            m_scrollLockButton.IsOn = true;

            toolbar.AddSpace();

            // log level
            IList<LogLevel> levels = LogLevel.ListLevels();
            string[] names = new string[levels.Count];
            for (int i = 0; i < names.Length; ++i)
            {
                names[i] = levels[i].Name;
            }

            toolbar.AddDropList(names);

            toolbar.AddFlexibleSpace();

            // filter field
            SearchField filterField = new SearchField();
            filterField.TextChangedDelegate = delegate(TextField field)
            {
                m_consoleView.SetFilterText(field.Text);
            };
            filterField.Width = Width;
            toolbar.AddSubview(filterField);

            toolbar.AddButton("Terminal", delegate(Button button)
            {
                TerminalWindow.ShowWindow();
            });

            return toolbar;
        }

        private bool KeyDownHandler(View view, Event evt)
        {
            return false;
        }

        private bool KeyUpHandler(View view, Event evt)
        {
            return false;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public Console Console
        { 
            get { return m_delegate.Console; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private string GetText()
        {
            return m_consoleView.GetText(); // TODO: handle selected lines
        }

        //////////////////////////////////////////////////////////////////////////////

        #region ITableViewScrollDelegate implementation

        public void OnTableScroll(TableView table, float oldPos)
        {
            if (table.ScrollPosTop < oldPos)
            {
                table.IsScrollLocked = false;
                m_scrollLockButton.IsOn = false;
            }
        }

        #endregion
    }
}

