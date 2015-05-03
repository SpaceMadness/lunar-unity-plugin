using UnityEngine;
using System.Collections;

namespace LunarEditor
{
    delegate void ToggleButtonDelegate(ToggleButton button);
    
    class ToggleButton : AbstractButton
    {
        public ToggleButton(string title = "", ToggleButtonDelegate buttonDelegate = null)
            : base(title)
        {
            ButtonDelegate = buttonDelegate;
        }
        
        public override void OnGUI()
        {
            bool oldFlag = IsOn;
            IsOn = GUI.Toggle(Frame, IsOn, Content);
            if ((oldFlag ^ IsOn) && ButtonDelegate != null)
            {
                ButtonDelegate(this);
            }
        }

        protected override GUIStyle CreateGUIStyle()
        {
            return new GUIStyle("toggle");
        }
        
        //////////////////////////////////////////////////////////////////////////////
        
        #region Properties
        
        public ToggleButtonDelegate ButtonDelegate { get; set; }
        public bool IsOn { get; set; }
        
        #endregion
    }
}