using UnityEngine;
using System.Collections;

namespace LunarPluginInternal
{
    public class TerminalCanvas : MonoBehaviour, ITerminalView, ICommandInputFieldDelegate
    {
        [SerializeField]
        CommandInputField input; // FIXME

        private Terminal m_terminal;

        void OnEnable()
        {
            if (m_terminal == null)
                m_terminal = new FormattedTerminal(1024); // FIXME

            if (input == null)
            {
                Debug.LogError("Missing command input");
                return;
            }

            input.Delegate = this;
        }

        #region ICommandInputFieldDelegate

        public void ExecuteCommand(CommandInputField input, string commandLine)
        {
            App.ExecCommand(commandLine, true);
        }

        public void AutoComplete(CommandInputField input, string commandLine, int index, bool doubleTab)
        {
            string line = m_terminal.DoAutoComplete(commandLine, index, doubleTab);
            if (line != null)
            {
                input.Text = line;
            }
        }

        #endregion
    }
}