using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

namespace LunarPluginInternal
{
    class SelectOnAwake : MonoBehaviour
    {
        #pragma warning disable 0649
        public InputField selectable;
        #pragma warning restore 0649

        void OnEnable()
        {
            StartCoroutine(Select());
        }

        IEnumerator Select()
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }
    }
}
