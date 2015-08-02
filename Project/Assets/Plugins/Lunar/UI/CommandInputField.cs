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
    }

    #endregion

    #region Properties

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
