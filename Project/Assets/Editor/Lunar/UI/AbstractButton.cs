using UnityEngine;
using System.Collections;

namespace LunarEditor
{
    abstract class AbstractButton : View
    {
        public AbstractButton(string title)
        {
            Content = new GUIContent(title);

            Vector2 size = Style.CalcSize(Content);
            Width = size.x + 2 * UISize.ButtonBorder;
            Height = UISize.ButtonHeight;
        }
        
        //////////////////////////////////////////////////////////////////////////////
        
        #region Properties
        
        public string Title 
        { 
            get { return Content.text; }
        }

        protected GUIContent Content { get; private set; }
        
        #endregion
    }
}