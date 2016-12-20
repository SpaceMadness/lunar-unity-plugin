//
//  FastList.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

﻿using LunarPlugin;

namespace LunarPluginInternal
{
    internal interface IFastList
    {
    }

    class FastList<T> : IFastList where T : FastListNode
    {
        internal FastListNode m_listFirst;
        internal FastListNode m_listLast;

        private int m_size;

        public void AddFirstItem(T item)
        {
            InsertItem(item, null, m_listFirst);
        }

        public void AddLastItem(T item)
        {
            InsertItem(item, m_listLast, null);
        }

        public void InsertBeforeItem(T node, T item)
        {
            InsertItem(item, node != null ? node.m_listPrev : null, node);
        }

        public void InsertAfterItem(T node, T item)
        {
            InsertItem(item, node, node != null ? node.m_listNext : null);
        }

        public virtual void RemoveItem(T item)
        {
            Assert.Greater(m_size, 0);
            Assert.AreSame(this, item.m_list);

            FastListNode prev = item.m_listPrev;
            FastListNode next = item.m_listNext;

            if (prev != null)
            {
                prev.m_listNext = next;
            }
            else
            {
                m_listFirst = next;
            }

            if (next != null)
            {
                next.m_listPrev = prev;
            }
            else
            {
                m_listLast = prev;
            }

            item.m_listNext = item.m_listPrev = null;
            item.m_list = null;
            --m_size;
        }

        public virtual T RemoveFirstItem()
        {
            T node = ListFirst;
            if (node != null)
            {
                RemoveItem(node);
            }

            return node;
        }

        public virtual T RemoveLastItem()
        {
            T node = ListLast;
            if (node != null)
            {
                RemoveItem(node);
            }

            return node;
        }

        public virtual bool ContainsItem(FastListNode item)
        {
            if (item.m_list != this)
            {
                return false;
            }

            for (FastListNode t = m_listFirst; t != null; t = t.m_listNext)
            {
                if (t == item)
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual void InsertItem(FastListNode item, FastListNode prev, FastListNode next)
        {
            Assert.IsNull(item.m_list);

            if (next != null)
            {
                next.m_listPrev = item;
            }
            else
            {
                m_listLast = item;
            }

            if (prev != null)
            {
                prev.m_listNext = item;
            }
            else
            {
                m_listFirst = item;
            }

            item.m_listPrev = prev;
            item.m_listNext = next;
            item.m_list = this;
            ++m_size;
        }

        public virtual void Clear()
        {
            for (FastListNode t = m_listFirst; t != null; )
            {
                FastListNode next = t.m_listNext;
                t.m_listPrev = t.m_listNext = null;
                t.m_list = null;
                t = next;
            }

            m_listFirst = m_listLast = null;
            m_size = 0;
        }

        public int Count
        {
            get { return m_size; }
        }

        public T ListFirst
        {
            get { return (T)m_listFirst; }
            protected set { m_listFirst = value; }
        }

        public T ListLast
        {
            get { return (T)m_listLast; }
            protected set { m_listLast = value; }
        }
    }

    class FastConcurrentList<T> : FastList<T> where T : FastListNode
    {
        public override void RemoveItem(T item)
        {
            lock (this)
            {
                base.RemoveItem(item);
            }
        }

        public override T RemoveFirstItem()
        {
            lock (this)
            {
                return base.RemoveFirstItem();
            }
        }

        public override T RemoveLastItem()
        {
            lock (this)
            {
                return base.RemoveLastItem();
            }
        }

        public override bool ContainsItem(FastListNode item)
        {
            lock (this)
            {
                return base.ContainsItem(item);
            }
        }

        protected override void InsertItem(FastListNode item, FastListNode prev, FastListNode next)
        {
            lock (this)
            {
                base.InsertItem(item, prev, next);
            }
        }

        public override void Clear()
        {
            lock (this)
            {
                base.Clear();
            }
        }
    }

    class FastListNode
    {
        internal FastListNode m_listPrev;
        internal FastListNode m_listNext;
        internal IFastList m_list;

        protected FastListNode ListNodePrev
        {
            get { return m_listPrev; }
        }

        protected FastListNode ListNodeNext
        {
            get { return m_listNext; }
        }

        #if LUNAR_DEVELOPMENT

        /* For Unit testing */
        public void DetachFromList()
        {
            m_list = null;
            m_listPrev = m_listNext = null;
        }

        #endif // LUNAR_DEVELOPMENT
    }

    class FastListNode<T> : FastListNode where T : FastListNode
    {
        public new T ListNodePrev
        {
            get { return (T)m_listPrev; }
        }

        public new T ListNodeNext
        {
            get { return (T)m_listNext; }
        }
    }
}
