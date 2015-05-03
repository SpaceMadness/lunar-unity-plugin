using UnityEngine;
using System.Collections;

namespace LunarEditor
{
    delegate void ToggleChangeDelegate(Toggle toggle);

    class Toggle : View 
    {
        private GUIContent m_content;

        public Toggle(string title, bool isOn = false)
        {
            m_content = new GUIContent(title);
            IsOn = isOn;

            Vector2 size = Style.CalcSize(m_content);
            Width = size.x;
            Height = size.y;
        }

        public override void OnGUI()
        {
            bool oldFlag = IsOn;
            IsOn = GUI.Toggle(Frame, IsOn, m_content);
            if ((oldFlag ^ IsOn) && Delegate != null)
            {
                Delegate(this);
            }
        }

        protected override GUIStyle CreateGUIStyle()
        {
            return new GUIStyle("toggle");
        }

        #region Properties

        public bool IsOn { get; set; }
        public ToggleChangeDelegate Delegate { get; set; }

        #endregion
    }
}