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
        m_inputField = GetComponent<InputField>();
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            print(m_inputField.text);
            m_inputField.text = "";
        }
    }

    #endregion
}
