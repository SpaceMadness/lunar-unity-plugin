using UnityEngine;

using System.Collections.Generic;

namespace LunarPluginInternal
{
    public class LunarRuntimeBehaviour : MonoBehaviour
    {
        static LunarRuntimeBehaviour s_instance;

        //////////////////////////////////////////////////////////////////////////////

        #region Callbacks

        void Awake()
        {
            InitInstance();
        }

        void OnEnable()
        {
            InitInstance();
        }

        void InitInstance()
        {
            if (s_instance == null)
            {
                s_instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (s_instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            UpdateBindings();
        }

        void OnDestroy()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Bindings

        private void UpdateBindings()
        {
            App.UpdateKeyBindings();
        }

        #endregion
    }
}

