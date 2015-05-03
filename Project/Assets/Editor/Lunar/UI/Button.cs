using UnityEngine;
using System.Collections;

namespace LunarEditor
{
    delegate void ButtonDelegate(Button button);

    class Button : AbstractButton
    {
        public Button(string title, ButtonDelegate buttonDelegate)
            : base(title)
        {
            ButtonDelegate = buttonDelegate;
        }

        public override void OnGUI()
        {
            if (GUI.Button(Frame, Content))
            {
                if (ButtonDelegate != null)
                    ButtonDelegate(this);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public ButtonDelegate ButtonDelegate { get; private set; }

        #endregion
    }
}