using UnityEngine;
using UnityEditor;

using System;
using System.Text;
using System.Collections.Generic;

using LunarPluginInternal;

namespace LunarEditor
{
    class TerminalWindow : Window, ITerminalCompositeViewDelegate
    {
        public TerminalWindow()
            : base("Terminal")
        {
            this.minSize = new Vector2(320, 240);
        }

        //////////////////////////////////////////////////////////////////////////////

        protected override void CreateUI()
        {
            TerminalCompositeView terminalView = new TerminalCompositeView(this, this.Width, this.Height);
            AddSubview(terminalView);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region ITerminalCompositeViewDelegate

        public void ExecCommand(string commandLine)
        {
            EditorApp.ExecCommand(commandLine, true);
        }

        public Terminal Terminal
        {
            get { return EditorApp.Terminal; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
        
        #region Menu item
        
        internal static void ShowWindow()
        {
            EditorWindow.GetWindow<TerminalWindow>();
        }

        #endregion
    }
}