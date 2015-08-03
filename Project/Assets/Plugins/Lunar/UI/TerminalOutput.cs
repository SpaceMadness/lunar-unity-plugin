using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

namespace LunarPluginInternal
{
    [RequireComponent(typeof(RectTransform))]
    public class TerminalOutput : MonoBehaviour
    {
        private RectTransform m_rectTransform;

        private bool scheduled;

        private IList<Text> entries = new List<Text>();

        [SerializeField]
        private Text entryPrefab;

        public void Add(string text)
        {
            RectTransform transform = this.RectTransform;

            Text entry = Instantiate(entryPrefab.gameObject).GetComponent<Text>();
            entry.text = text;
            RectTransform entryTransform = entry.GetComponent<RectTransform>();

            entryTransform.SetParent(transform);
            entries.Add(entry);

            if (!scheduled)
            {
                scheduled = true;
                StartCoroutine(CalcSize());
            }
        }

        private IEnumerator CalcSize()
        {
            yield return null; // skip a frame

            foreach (Text text in entries)
            {
                print(text.rectTransform.rect.size);
            }
            scheduled = false;
        }

        private RectTransform RectTransform
        {
            get
            {
                if (m_rectTransform == null)
                    m_rectTransform = GetComponent<RectTransform>();

                return m_rectTransform;
            }
        }
    }
}
