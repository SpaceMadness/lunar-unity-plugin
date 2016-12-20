//
//  ObjectsPool.cs
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LunarPlugin;

namespace LunarPluginInternal
{
    internal interface IObjectsPool
    {
        void Recycle(ObjectsPoolEntry entry);
    }

    class ObjectsPool<T> : FastList<ObjectsPoolEntry>, IObjectsPool, IDestroyable
        where T : ObjectsPoolEntry, new()
    {
        public ObjectsPool()
        {
        }

        public T NextAutoRecycleObject()
        {
            return (T)NextObject().AutoRecycle();
        }

        public virtual T NextObject()
        {
            ObjectsPoolEntry first = RemoveFirstItem();
            if (first == null)
            {
                first = CreateObject();
            }

            first.pool = this;
            first.recycled = false;

            return (T)first;
        }

        public virtual void Recycle(ObjectsPoolEntry e)
        {
            Assert.IsInstanceOfType<T>(e);
            Assert.AreSame(this, e.pool);

            AddLastItem(e);
        }

        protected virtual T CreateObject()
        {
            return new T();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Destroyable

        public virtual void Destroy()
        {
            Clear();
        }

        #endregion
    }
    
    class ObjectsPoolConcurrent<T> : ObjectsPool<T>
        where T : ObjectsPoolEntry, new()
    {
        public override T NextObject()
        {
            lock (this)
            {
                return base.NextObject();
            }
        }

        public override void Recycle(ObjectsPoolEntry e)
        {
            lock (this)
            {
                base.Recycle(e);
            }
        }

        public override void Destroy()
        {
            lock (this)
            {
                base.Destroy();
            }
        }
    }

    class ObjectsPoolEntry : FastListNode
    {
        internal IObjectsPool pool;
        internal bool recycled;

        public ObjectsPoolEntry AutoRecycle()
        {
            TimerManager.ScheduleTimer(Recycle);
            return this;
        }

        public void Recycle()
        {
            if (pool != null)
            {
                Assert.IsFalse(recycled);
                recycled = true;

                pool.Recycle(this);
            }
            
            OnRecycleObject();
        }

        protected virtual void OnRecycleObject()
        {
        }
    }

    class ReusableList<T> : ObjectsPoolEntry, IList<T>
    {
        private List<T> m_innerList;

        public ReusableList()
        {
            m_innerList = new List<T>();
        }

        protected override void OnRecycleObject()
        {
            m_innerList.Clear();

            base.OnRecycleObject();
        }

        public T[] ToArray()
        {
            return m_innerList.ToArray();
        }

        public void Sort()
        {
            m_innerList.Sort();
        }

        public void Sort(Comparison<T> comparison)
        {
            m_innerList.Sort(comparison);
        }

        #region IList implementation

        public int IndexOf(T item)
        {
            return m_innerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            m_innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            m_innerList.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return m_innerList[index]; }
            set { m_innerList[index] = value; }
        }

        #endregion

        #region ICollection implementation

        public void Add(T item)
        {
            m_innerList.Add(item);
        }

        public void Clear()
        {
            m_innerList.Clear();
        }

        public bool Contains(T item)
        {
            return m_innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return m_innerList.Remove(item);
        }

        public int Count
        {
            get { return m_innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<T> GetEnumerator()
        {
            return m_innerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_innerList.GetEnumerator();
        }

        #endregion
    }

    static class ReusableLists
    {
        private static IDictionary<Type, object> m_poolLookup;

        public static ReusableList<T> NextAutoRecycleList<T>()
        {
            return NextList<T>().AutoRecycle() as ReusableList<T>;
        }

        public static ReusableList<T> NextList<T>()
        {
            if (m_poolLookup == null)
            {
                m_poolLookup = new Dictionary<Type, object>();
            }

            Type type = typeof(T);

            object poolObject;
            if (!m_poolLookup.TryGetValue(type, out poolObject))
            {
                poolObject = new ObjectsPoolConcurrent<ReusableList<T>>();
                m_poolLookup[type] = poolObject;
            }

            return ((ObjectsPool<ReusableList<T>>) poolObject).NextObject();
        }

        public static void Clear()
        {
            m_poolLookup.Clear();
        }
    }

    class RecyclableStringBuilder : ObjectsPoolEntry
    {
        private StringBuilder m_stringBuilder;

        public RecyclableStringBuilder()
        {
            m_stringBuilder = new StringBuilder();
        }

        protected override void OnRecycleObject()
        {
            m_stringBuilder.Length = 0;
            base.OnRecycleObject();
        }

        //
        // Properties
        //
        public int Capacity
        {
            get { return m_stringBuilder.Capacity; }
            set { m_stringBuilder.Capacity = value; }
        }
        
        public int Length
        {
            get { return m_stringBuilder.Length; }
            set { m_stringBuilder.Length = value; }
        }
        
        public int MaxCapacity
        {
            get { return m_stringBuilder.MaxCapacity; }
        }

        internal StringBuilder InnerStringBuilder
        {
            get { return m_stringBuilder; }
        }
        
        //
        // Indexer
        //
        public char this[int index]
        {
            get { return m_stringBuilder[index]; }
            set { m_stringBuilder[index] = value; }
        }
        
        //
        // Constructors
        //

        public RecyclableStringBuilder Init(int capacity)
        {
            m_stringBuilder.Capacity = capacity;
            
            return this;
        }

        public RecyclableStringBuilder Init(string value)
        {
            m_stringBuilder.Append(value);
            
            return this;
        }

        public RecyclableStringBuilder Init(string value, int capacity)
        {
            m_stringBuilder.Capacity = capacity;
            m_stringBuilder.Append(value);

            return this;
        }
        
        public RecyclableStringBuilder Init(string value, int startIndex, int length, int capacity)
        {
            m_stringBuilder.Capacity = capacity;
            m_stringBuilder.Append(value, startIndex, length);

            return this;
        }
        
        //
        // Methods
        //
        public RecyclableStringBuilder Append(float value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(sbyte value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(object value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(long value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(int value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(short value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(ushort value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(string value, int startIndex, int count) { m_stringBuilder.Append(value, startIndex, count); return this; }
        public RecyclableStringBuilder Append(char[] value, int startIndex, int charCount) { m_stringBuilder.Append(value, startIndex, charCount); return this; }
        public RecyclableStringBuilder Append(char value, int repeatCount) { m_stringBuilder.Append(value, repeatCount); return this; }
        public RecyclableStringBuilder Append(char value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(ulong value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(uint value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(double value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(char[] value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(string value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(bool value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(byte value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder Append(decimal value) { m_stringBuilder.Append(value); return this; }
        public RecyclableStringBuilder AppendFormat(string format, params object[] args) { m_stringBuilder.AppendFormat(format, args); return this; }
        public RecyclableStringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args) { m_stringBuilder.AppendFormat(provider, format, args); return this; }
        public RecyclableStringBuilder AppendFormat(string format, object arg0) { m_stringBuilder.AppendFormat(format, arg0); return this; }
        public RecyclableStringBuilder AppendFormat(string format, object arg0, object arg1) { m_stringBuilder.AppendFormat(format, arg0, arg1); return this; }
        public RecyclableStringBuilder AppendFormat(string format, object arg0, object arg1, object arg2) { m_stringBuilder.AppendFormat(format, arg0, arg1, arg2); return this; }
        public RecyclableStringBuilder AppendLine() { m_stringBuilder.AppendLine(); return this; }
        public RecyclableStringBuilder AppendLine(string value) { m_stringBuilder.AppendLine(value); return this; }
        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) { m_stringBuilder.CopyTo(sourceIndex, destination, destinationIndex, count); }
        public int EnsureCapacity(int capacity) { return m_stringBuilder.EnsureCapacity(capacity); }
        public bool Equals(StringBuilder sb) { return m_stringBuilder.Equals(sb); }
        public RecyclableStringBuilder Insert(int index, short value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, int value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, long value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, object value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, sbyte value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, float value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, ushort value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, uint value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, ulong value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, string value, int count) { m_stringBuilder.Insert(index, value, count); return this; }
        public RecyclableStringBuilder Insert(int index, char[] value, int startIndex, int charCount) { m_stringBuilder.Insert(index, value, startIndex, charCount); return this; }
        public RecyclableStringBuilder Insert(int index, char value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, char[] value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, string value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, bool value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, byte value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, decimal value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Insert(int index, double value) { m_stringBuilder.Insert(index, value); return this; }
        public RecyclableStringBuilder Remove(int startIndex, int length) { m_stringBuilder.Remove(startIndex, length); return this; }
        public RecyclableStringBuilder Replace(string oldValue, string newValue, int startIndex, int count) { m_stringBuilder.Replace(oldValue, newValue, startIndex, count); return this; }
        public RecyclableStringBuilder Replace(char oldChar, char newChar, int startIndex, int count) { m_stringBuilder.Replace(oldChar, newChar, startIndex, count); return this; }
        public RecyclableStringBuilder Replace(char oldChar, char newChar) { m_stringBuilder.Replace(oldChar, newChar); return this; }
        public RecyclableStringBuilder Replace(string oldValue, string newValue) { m_stringBuilder.Replace(oldValue, newValue); return this; }
        public override string ToString() { return m_stringBuilder.ToString(); }
        public string ToString(int startIndex, int length) { return m_stringBuilder.ToString(startIndex, length); }
    }

    static class StringBuilderPool
    {
        private static ObjectsPool<RecyclableStringBuilder> m_pool;

        static StringBuilderPool()
        {
            m_pool = new ObjectsPoolConcurrent<RecyclableStringBuilder>();
        }

        public static RecyclableStringBuilder NextBuilder()
        {
            return m_pool.NextObject();
        }

        public static StringBuilder NextAutoRecycleBuilder()
        {
            return m_pool.NextAutoRecycleObject().InnerStringBuilder;
        }
    }
}
