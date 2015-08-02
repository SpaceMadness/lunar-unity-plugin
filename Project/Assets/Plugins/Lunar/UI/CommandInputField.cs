using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

namespace LunarPluginInternal
{
    public interface ICommandInputFieldDelegate
    {
        void ExecuteCommand(CommandInputField input, string commandLine);
        void AutoComplete(CommandInputField input, string commandLine, int index, bool doubleTab);
    }

    [RequireComponent(typeof(InputField))]
    public class CommandInputField : MonoBehaviour
    {
        private InputField m_inputField;

        #region MonoBehaviour callbacks

        void OnEnable()
        {
            // disable tab characters
            this.InputField.onValidateInput = delegate(string text, int charIndex, char addedChar)
            {
                if (addedChar == '\t')
                {
                    return '\0';
                }

                return addedChar;
            };
        }

        void Update ()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Submit();
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                AutoComplete();
            }
        }

        #endregion

        #region Input

        void Submit()
        {
            print(this.InputField.text);
            this.InputField.text = "";
        }

        void AutoComplete()
        {
            if (this.Delegate != null)
            {
                this.Delegate.AutoComplete(this, this.Text, this.Text.Length, false);
            }
        }

        #endregion

        #region Properties

        public ICommandInputFieldDelegate Delegate
        {
            get; set;
        }

        public string Text
        {
            get { return this.InputField.text; }
            set
            {
                this.InputField.text = StringUtils.NonNullOrEmpty(value);
                this.InputField.MoveTextEnd(false);
            }
        }

        private int CaretPos
        {
            get { return this.InputField.caretPosition; }
            set { this.InputField.caretPosition = Mathf.Clamp(value, 0, this.Text.Length); }
        }

        private InputField InputField
        {
            get
            {
                if (m_inputField == null)
                    m_inputField = GetComponent<InputField>();

                return m_inputField;
            }
        }

        #endregion
    }
}
