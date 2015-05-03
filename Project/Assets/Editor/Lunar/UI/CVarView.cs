using System;

using UnityEngine;
using UnityEditor;

using LunarPlugin;

namespace LunarEditor
{
    class CVarView : View
    {
        public static readonly float kItemHeight = 16;

        private delegate void CVarGUIDelegate(CVar cvar, ref Rect rect);

        private CVar m_cvar;
        private CVarGUIDelegate m_guiDelegate;

        public CVarView(CVar cvar, float width)
        {
            if (cvar == null)
            {
                throw new ArgumentNullException("CVar is null");
            }

            this.Width = width;
            this.Height = kItemHeight;

            m_cvar = cvar;
            m_guiDelegate = GetGUIDelegate(cvar);
        }

        protected override void DrawGUI()
        {
            BeginGroup(Frame);
            {
                Rect cvarRect = this.Frame;
                m_guiDelegate(m_cvar, ref cvarRect);
            }
            EndGroup();
        }

        #region GUI delegates

        private static CVarGUIDelegate GetGUIDelegate(CVar cvar)
        {
            switch (cvar.Type)
            {
                case CVarType.Boolean: return BoolGUIDelegate;
                case CVarType.Integer: return IntGUIDelegate;
                case CVarType.Float:   return FloatGUIDelegate;
            }

            return StringGUIDelegate;
        }

        private static void StringGUIDelegate(CVar cvar, ref Rect rect)
        {
            string oldValue = cvar.Value;
            string newValue = EditorGUI.TextField(rect, cvar.Name, oldValue);
            if (oldValue != newValue)
            {
                cvar.Value = newValue;
            }
        }

        private static void FloatGUIDelegate(CVar cvar, ref Rect rect)
        {
            float oldValue = cvar.FloatValue;
            float newValue = EditorGUI.FloatField(rect, cvar.Name, oldValue);
            if (oldValue != newValue)
            {
                cvar.FloatValue = newValue;
            }
        }

        private static void IntGUIDelegate(CVar cvar, ref Rect rect)
        {
            int oldValue = cvar.IntValue;
            int newValue = EditorGUI.IntField(rect, cvar.Name, oldValue);
            if (oldValue != newValue)
            {
                cvar.IntValue = newValue;
            }
        }

        private static void BoolGUIDelegate(CVar cvar, ref Rect rect)
        {
            bool oldValue = cvar.BoolValue;
            bool newValue = EditorGUI.Toggle(rect, cvar.Name, oldValue);
            if (oldValue != newValue)
            {
                cvar.BoolValue = newValue;
            }
        }

        #endregion
    }
}

