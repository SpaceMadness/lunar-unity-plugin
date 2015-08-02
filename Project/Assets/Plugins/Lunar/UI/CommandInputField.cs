using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

[RequireComponent(typeof(InputField))]
class CommandInputField : MonoBehaviour
{
    private InputField m_inputField;

    #region MonoBehaviour callbacks

    void OnEnable()
    {
        if (m_inputField == null)
        {
            m_inputField = GetComponent<InputField>();

            // disable tab characters
            m_inputField.onValidateInput += delegate(string text, int charIndex, char addedChar)
            {
                if (addedChar == '\t')
                {
                    return '\0';
                }

                return addedChar;
            };
        }
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
        print(m_inputField.text);
        m_inputField.text = "";
    }

    void AutoComplete()
    {
    }

    #endregion
}
