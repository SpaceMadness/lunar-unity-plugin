using UnityEngine;
using System.Collections;

namespace LunarPluginInternal
{
    public class TerminalCanvas : MonoBehaviour, ITerminalView, ICommandInputFieldDelegate
    {
        [SerializeField]
        private CommandInputField input; // FIXME

        void OnEnable()
        {
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
            string line = this.Terminal.DoAutoComplete(commandLine, index, doubleTab);
            if (line != null)
            {
                input.Text = line;
            }
        }

        #endregion

        #region Properties

        private Terminal Terminal
        {
            get { return App.Terminal; }
        }

        #endregion
    }
}