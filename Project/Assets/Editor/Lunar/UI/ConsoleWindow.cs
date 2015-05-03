using UnityEngine;
using UnityEditor;

using System;
using System.Text;
using System.Collections.Generic;

using LunarPlugin;

using LunarPluginInternal;

using Event = LunarEditor.Event;
using Console = LunarEditor.Console;

namespace LunarEditor
{
    class ConsoleWindow : Window, IConsoleCompositeViewDelegate
    {
        public ConsoleWindow()
            : base("Console")
        {
            this.minSize = new Vector2(586, 366);
        }

        #region UI

        protected override void CreateUI()
        {
            ConsoleCompositeView consoleView = new ConsoleCompositeView(this, this.Width, this.Height);
            AddSubview(consoleView);
        }

        #endregion

        #region IConsoleCompositeViewDelegate implementation

        public Console Console
        {
            get { return EditorApp.Console; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Menu item

        internal static void ShowWindow()
        {
            EditorWindow.GetWindow<ConsoleWindow>();
        }

        #endregion
    }
}
