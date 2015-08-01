using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

namespace LunarPluginInternal
{
    /*
     * This is a dirty hack to fix input field caret alignment issue when using custom fonts.
     * Last checked with Unity 5.1.2f1
     */

    [RequireComponent(typeof(InputField))]
    class InputFieldCaretFix : MonoBehaviour, ISelectHandler
    {
        public float offset = 0;

        public void OnSelect(BaseEventData eventData)
        {
            if (offset != 0)
            {
                InputField inputField = gameObject.GetComponent<InputField>();
                RectTransform caretTransform = (RectTransform) inputField.transform.Find(gameObject.name + " Input Caret");
                if (caretTransform != null)
                {
                    MoveCaret(caretTransform);
                }
                else
                {
                    // need to skip 1 frame
                    StartCoroutine(MoveCaretRoutine(inputField));
                }
            }
        }

        IEnumerator MoveCaretRoutine(InputField inputField)
        {
            yield return null; // skip a frame

            string caretName = gameObject.name + " Input Caret";

            RectTransform caretTransform = (RectTransform) inputField.transform.Find(caretName);
            if (caretTransform != null)
            {
                MoveCaret(caretTransform);
            }
            else
            {
                Debug.LogWarning("Can't find RectTransform of '" + caretName + "': caret might be misaligned!");
            }
        }

        void MoveCaret(RectTransform caretTransform)
        {
            Vector3 localPosition = caretTransform.localPosition;
            localPosition.y += offset;
            caretTransform.localPosition = localPosition;
        }
    }
}