//
//  ArrayList.cs
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
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LunarPluginInternal
{
    [DebuggerDisplay("Count={Count}")]
    [Serializable]
    class ArrayList<T> : IList<T>, IList, ICollection<T>, IEnumerable<T>, IEnumerable, ICollection
    {
        private static readonly T[] EmptyArray = new T[0];

        private T[] _items;
        private int _size;
        private int _version;

        public T[] Items
        {
            get { return _items; }
        }

        //
        // Properties
        //
        public int Capacity
        {
            get
            {
                return this._items.Length;
            }
            set
            {
                if (value < this._size)
                {
                    throw new ArgumentOutOfRangeException();
                }
                Array.Resize<T>(ref this._items, value);
            }
        }

        public int Count
        {
            get
            {
                return this._size;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        //
        // Indexer
        //
        public T this[int index]
        {
            [MethodImpl((MethodImplOptions)256)]
            get
            {
                if (index >= this._size)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return _items[index];
            }
            [MethodImpl((MethodImplOptions)256)]
            set
            {
                if (index >= this._size)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                this._items[index] = value;
                this._version++;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                try
                {
                    this[index] = (T)((object)value);
                    return;
                }
                catch (NullReferenceException)
                {
                }
                catch (InvalidCastException)
                {
                }
                throw new ArgumentException("value");
            }
        }

        //
        // Constructors
        //
        internal ArrayList(T[] data, int size)
        {
            this._items = data;
            this._size = size;
        }

        public ArrayList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }
            this._items = new T[capacity];
        }

        public ArrayList(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            ICollection<T> collection2 = collection as ICollection<T>;
            if (collection2 == null)
            {
                this._items = EmptyArray;
                this.AddEnumerable(collection);
            }
            else
            {
                this._size = collection2.Count;
                this._items = new T[this._size];
                collection2.CopyTo(this._items, 0);
            }
        }

        public ArrayList()
        {
            this._items = EmptyArray;
        }

        //
        // Static Methods
        //
        private static void CheckMatch(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
        }

        //
        // Methods
        //
        int IList.Add(object item)
        {
            try
            {
                this.Add((T)((object)item));
                return this._size - 1;
            }
            catch (NullReferenceException)
            {
            }
            catch (InvalidCastException)
            {
            }
            throw new ArgumentException("item");
        }

        public void Add(T item)
        {
            if (this._size == this._items.Length)
            {
                this.GrowIfNeeded(1);
            }
            this._items[this._size++] = item;
            this._version++;
        }

        private void AddCollection(ICollection<T> collection)
        {
            int count = collection.Count;
            if (count == 0)
            {
                return;
            }
            this.GrowIfNeeded(count);
            collection.CopyTo(this._items, this._size);
            this._size += count;
        }

        private void AddEnumerable(IEnumerable<T> enumerable)
        {
            foreach (T current in enumerable)
            {
                this.Add(current);
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            ICollection<T> collection2 = collection as ICollection<T>;
            if (collection2 != null)
            {
                this.AddCollection(collection2);
            }
            else
            {
                this.AddEnumerable(collection);
            }
            this._version++;
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return new ReadOnlyCollection<T>(this);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            this.CheckRange(index, count);
            return Array.BinarySearch<T>(this._items, index, count, item, comparer);
        }

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return Array.BinarySearch<T>(this._items, 0, this._size, item, comparer);
        }

        public int BinarySearch(T item)
        {
            return Array.BinarySearch<T>(this._items, 0, this._size, item);
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || index > this._size)
            {
                throw new ArgumentOutOfRangeException("index");
            }
        }

        private void CheckRange(int idx, int count)
        {
            if (idx < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (idx + count > this._size)
            {
                throw new ArgumentException("index and count exceed length of list");
            }
        }

        private void CheckRangeOutOfRange(int idx, int count)
        {
            if (idx < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (idx + count > this._size)
            {
                throw new ArgumentOutOfRangeException("index and count exceed length of list");
            }
        }

        private void CheckStartIndex(int index)
        {
            if (index < 0 || index > this._size)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
        }

        public void Clear()
        {
            Array.Clear(this._items, 0, this._items.Length);
            this._size = 0;
            this._version++;
        }

        bool IList.Contains(object item)
        {
            try
            {
                return this.Contains((T)((object)item));
            }
            catch (NullReferenceException)
            {
            }
            catch (InvalidCastException)
            {
            }
            return false;
        }

        public bool Contains(T item)
        {
            return Array.IndexOf<T>(this._items, item, 0, this._size) != -1;
        }

        public ArrayList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            ArrayList<TOutput> list = new ArrayList<TOutput>(this._size);
            for (int i = 0; i < this._size; i++)
            {
                list._items[i] = converter(this._items[i]);
            }
            list._size = this._size;
            return list;
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            this.CheckRange(index, count);
            Array.Copy(this._items, index, array, arrayIndex, count);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(this._items, 0, array, arrayIndex, this._size);
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Rank > 1 || array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException("Array must be zero based and single dimentional", "array");
            }
            Array.Copy(this._items, 0, array, arrayIndex, this._size);
        }

        public void CopyTo(T[] array)
        {
            Array.Copy(this._items, 0, array, 0, this._size);
        }

        public bool Exists(Predicate<T> match)
        {
            ArrayList<T>.CheckMatch(match);
            for (int i = 0; i < this._size; i++)
            {
                T obj = this._items[i];
                if (match(obj))
                {
                    return true;
                }
            }
            return false;
        }

        public T Find(Predicate<T> match)
        {
            ArrayList<T>.CheckMatch(match);
            for (int i = 0; i < this._size; i++)
            {
                T t = this._items[i];
                if (match(t))
                {
                    return t;
                }
            }
            return default(T);
        }

        private ArrayList<T> FindAllList(Predicate<T> match)
        {
            ArrayList<T> list = new ArrayList<T>();
            for (int i = 0; i < this._size; i++)
            {
                if (match(this._items[i]))
                {
                    list.Add(this._items[i]);
                }
            }
            return list;
        }

        public void ForEach(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            for (int i = 0; i < this._size; i++)
            {
                action(this._items[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public ArrayList<T>.Enumerator GetEnumerator()
        {
            return new ArrayList<T>.Enumerator(this);
        }

        public ArrayList<T> GetRange(int index, int count)
        {
            this.CheckRange(index, count);
            T[] array = new T[count];
            Array.Copy(this._items, index, array, 0, count);
            return new ArrayList<T>(array, count);
        }

        private void GrowIfNeeded(int newCount)
        {
            int num = this._size + newCount;
            if (num > this._items.Length)
            {
                this.Capacity = Math.Max(Math.Max(this.Capacity * 2, 4), num);
            }
        }

        int IList.IndexOf(object item)
        {
            try
            {
                return this.IndexOf((T)((object)item));
            }
            catch (NullReferenceException)
            {
            }
            catch (InvalidCastException)
            {
            }
            return -1;
        }

        public int IndexOf(T item, int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (index + count > this._size)
            {
                throw new ArgumentOutOfRangeException("index and count exceed length of list");
            }
            return Array.IndexOf<T>(this._items, item, index, count);
        }

        public int IndexOf(T item, int index)
        {
            this.CheckIndex(index);
            return Array.IndexOf<T>(this._items, item, index, this._size - index);
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf<T>(this._items, item, 0, this._size);
        }

        public void Insert(int index, T item)
        {
            this.CheckIndex(index);
            if (this._size == this._items.Length)
            {
                this.GrowIfNeeded(1);
            }
            this.Shift(index, 1);
            this._items[index] = item;
            this._version++;
        }

        void IList.Insert(int index, object item)
        {
            this.CheckIndex(index);
            try
            {
                this.Insert(index, (T)((object)item));
                return;
            }
            catch (NullReferenceException)
            {
            }
            catch (InvalidCastException)
            {
            }
            throw new ArgumentException("item");
        }

        private void InsertCollection(int index, ICollection<T> collection)
        {
            int count = collection.Count;
            this.GrowIfNeeded(count);
            this.Shift(index, count);
            collection.CopyTo(this._items, index);
        }

        private void InsertEnumeration(int index, IEnumerable<T> enumerable)
        {
            foreach (T current in enumerable)
            {
                this.Insert(index++, current);
            }
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            this.CheckIndex(index);
            if (collection == this)
            {
                T[] array = new T[this._size];
                this.CopyTo(array, 0);
                this.GrowIfNeeded(this._size);
                this.Shift(index, array.Length);
                Array.Copy(array, 0, this._items, index, array.Length);
            }
            else
            {
                ICollection<T> collection2 = collection as ICollection<T>;
                if (collection2 != null)
                {
                    this.InsertCollection(index, collection2);
                }
                else
                {
                    this.InsertEnumeration(index, collection);
                }
            }
            this._version++;
        }

        public int LastIndexOf(T item, int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "index is negative");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", count, "count is negative");
            }
            if (index - count + 1 < 0)
            {
                throw new ArgumentOutOfRangeException("cound", count, "count is too large");
            }
            return Array.LastIndexOf<T>(this._items, item, index, count);
        }

        public int LastIndexOf(T item, int index)
        {
            this.CheckIndex(index);
            return Array.LastIndexOf<T>(this._items, item, index, index + 1);
        }

        public int LastIndexOf(T item)
        {
            if (this._size == 0)
            {
                return -1;
            }
            return Array.LastIndexOf<T>(this._items, item, this._size - 1, this._size);
        }

        public bool Remove(T item)
        {
            int num = this.IndexOf(item);
            if (num != -1)
            {
                this.RemoveAt(num);
            }
            return num != -1;
        }

        void IList.Remove(object item)
        {
            try
            {
                this.Remove((T)((object)item));
            }
            catch (NullReferenceException)
            {
            }
            catch (InvalidCastException)
            {
            }
        }

        public int RemoveAll(Predicate<T> match)
        {
            ArrayList<T>.CheckMatch(match);
            int i;
            for (i = 0; i < this._size; i++)
            {
                if (match(this._items[i]))
                {
                    break;
                }
            }
            if (i == this._size)
            {
                return 0;
            }
            this._version++;
            int j;
            for (j = i + 1; j < this._size; j++)
            {
                if (!match(this._items[j]))
                {
                    this._items[i++] = this._items[j];
                }
            }
            if (j - i > 0)
            {
                Array.Clear(this._items, i, j - i);
            }
            this._size = i;
            return j - i;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this._size)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            this.Shift(index, -1);
            Array.Clear(this._items, this._size, 1);
            this._version++;
        }

        public void RemoveRange(int index, int count)
        {
            this.CheckRange(index, count);
            if (count > 0)
            {
                this.Shift(index, -count);
                Array.Clear(this._items, this._size, count);
                this._version++;
            }
        }

        public void Reverse(int index, int count)
        {
            this.CheckRange(index, count);
            Array.Reverse(this._items, index, count);
            this._version++;
        }

        public void Reverse()
        {
            Array.Reverse(this._items, 0, this._size);
            this._version++;
        }

        private void Shift(int start, int delta)
        {
            if (delta < 0)
            {
                start -= delta;
            }
            if (start < this._size)
            {
                Array.Copy(this._items, start, this._items, start + delta, this._size - start);
            }
            this._size += delta;
            if (delta < 0)
            {
                Array.Clear(this._items, this._size, -delta);
            }
        }

        public void Sort()
        {
            Array.Sort<T>(this._items, 0, this._size);
            this._version++;
        }

        public void Sort(IComparer<T> comparer)
        {
            Array.Sort<T>(this._items, 0, this._size, comparer);
            this._version++;
        }

        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }
            Array.Sort<T>(this._items, comparison);
            this._version++;
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            this.CheckRange(index, count);
            Array.Sort<T>(this._items, index, count, comparer);
            this._version++;
        }

        public T[] ToArray()
        {
            T[] array = new T[this._size];
            Array.Copy(this._items, array, this._size);
            return array;
        }

        public void TrimExcess()
        {
            this.Capacity = this._size;
        }

        public bool TrueForAll(Predicate<T> match)
        {
            ArrayList<T>.CheckMatch(match);
            for (int i = 0; i < this._size; i++)
            {
                if (!match(this._items[i]))
                {
                    return false;
                }
            }
            return true;
        }

        //
        // Nested Types
        //
        [Serializable]
        internal struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private ArrayList<T> l;

            private int ver;
            private int next;
            private T current;

            object IEnumerator.Current
            {
                get
                {
                    if (this.ver != this.l._version)
                    {
                        throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                    }
                    if (this.next <= 0)
                    {
                        throw new InvalidOperationException();
                    }
                    return this.current;
                }
            }

            public T Current
            {
                get
                {
                    return this.current;
                }
            }

            internal Enumerator(ArrayList<T> l)
            {
                this = default(ArrayList<T>.Enumerator);
                this.l = l;
                this.ver = l._version;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                ArrayList<T> list = this.l;
                if (this.next < list._size && this.ver == list._version)
                {
                    this.current = list._items[this.next++];
                    return true;
                }
                if (this.ver != this.l._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                this.next = -1;
                return false;
            }

            void IEnumerator.Reset()
            {
                if (this.ver != this.l._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                this.next = 0;
                this.current = default(T);
            }
        }
    }
}
