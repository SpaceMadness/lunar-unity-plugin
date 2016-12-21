using System;

using LunarPluginInternal;
using UnityEngine;
using System.Collections.Generic;

namespace LunarPlugin.Test
{
    public class TestInput : ICUpdatable
    {
        private readonly State m_oldState;
        private readonly State m_state;

        public TestInput()
        {
            m_state = new State();
            m_oldState = new State();
        }

        #region IUpdatable

        public void Update(float dt)
        {
            m_oldState.CopyFrom(m_state);
            m_state.ReleaseNonHoldKeys();
        }

        #endregion

        public void PressKey(KeyCode key, bool hold)
        {
            m_state.PressKey(key, hold);
        }

        public void ReleaseKey(KeyCode key)
        {
            m_state.ReleaseKey(key);
        }

        public bool GetKeyDown(KeyCode key)
        {
            return m_state.IsKeyPressed(key) && !m_oldState.IsKeyPressed(key);
        }

        public bool GetKeyUp(KeyCode key)
        {
            return !m_state.IsKeyPressed(key) && m_oldState.IsKeyPressed(key);
        }

        public bool GetKey(KeyCode key)
        {
            return m_state.IsKeyPressed(key);
        }

        private class State
        {
            private List<KeyEntry> m_keys;

            public State()
            {
                m_keys = new List<KeyEntry>();
            }

            public void PressKey(KeyCode key, bool hold)
            {
                if (!IsKeyPressed(key))
                {
                    m_keys.Add(new KeyEntry(key, hold));
                }
            }

            public void ReleaseKey(KeyCode key)
            {
                int index = IndexOf(key);
                if (index != -1)
                {
                    m_keys.RemoveAt(index);
                }
            }

            public bool IsKeyPressed(KeyCode key)
            {
                return IndexOf(key) != -1;
            }

            public void CopyFrom(State other)
            {
                m_keys.Clear();
                foreach (KeyEntry e in other.m_keys)
                {
                    if (e.hold)
                    {
                        m_keys.Add(e);
                    }
                }
            }

            public void ReleaseNonHoldKeys()
            {
                for (int i = m_keys.Count - 1; i >= 0; --i)
                {
                    if (!m_keys[i].hold)
                    {
                        m_keys.RemoveAt(i);
                    }
                }
            }

            private int IndexOf(KeyCode key)
            {
                for (int i = 0; i < m_keys.Count; ++i)
                {
                    if (m_keys[i].code == key)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        private struct KeyEntry
        {
            public readonly KeyCode code;
            public readonly bool hold;

            public KeyEntry(KeyCode code, bool hold)
            {
                this.code = code;
                this.hold = hold;
            }
        }
    }
}

