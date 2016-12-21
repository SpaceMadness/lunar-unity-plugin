//
//  CCycleArray.cs
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

﻿using UnityEngine;

using System;
using System.Collections;

namespace LunarPluginInternal
{
    class CCycleArray<E>
    {
        private E[] m_internalArray;

        public CCycleArray(int capacity)
        {
            m_internalArray = new E[capacity];
        }

        public E Add(E e)
        {
            int arrayIndex = ToArrayIndex(Length);
            E oldItem = m_internalArray[arrayIndex];
            m_internalArray[arrayIndex] = e;
            ++Length;

            if (Length - HeadIndex > m_internalArray.Length)
            {
                ++HeadIndex;
                return oldItem;
            }
            
            return default(E); // no items were destroyed
        }

        public void Clear()
        {
            if (!typeof(E).IsValueType && !typeof(E).IsPrimitive)
            {
                for (int i = 0; i < m_internalArray.Length; ++i)
                {
                    m_internalArray[i] = default(E);
                }
            }
            Length = 0;
            HeadIndex = 0;
        }

        public void TrimLength(int trimSize)
        {
            TrimToLength(Length - trimSize);
        }

        public void TrimToLength(int trimmedLength)
        {
            if (trimmedLength < HeadIndex || trimmedLength > Length)
            {
                throw new ArgumentException("Trimmed length " + trimmedLength + 
                    " should be between head index " + HeadIndex + 
                    " and length " + Length);
            }

            Length = trimmedLength;
        }

        public void TrimHeadIndex(int trimSize)
        {
            TrimToHeadIndex(HeadIndex + trimSize);
        }

        public void TrimToHeadIndex(int trimmedHeadIndex)
        {
            if (trimmedHeadIndex < HeadIndex || trimmedHeadIndex > Length)
            {
                throw new ArgumentException("Trimmed head index " + trimmedHeadIndex + 
                    " should be between head index " + HeadIndex + 
                    " and length " + Length);
            }

            HeadIndex = trimmedHeadIndex;
        }

        public E this[int index]
        {
            get 
            {
                int arrayIndex = ToArrayIndex(index);
                return m_internalArray[arrayIndex];
            }

            set
            {
                int arrayIndex = ToArrayIndex(index);
                m_internalArray[arrayIndex] = value;
            }
        }

        public int ToArrayIndex(int i)
        {
            return i % m_internalArray.Length;
        }

        public bool IsValidIndex(int index)
        {
            return index >= HeadIndex && index < Length;
        }

        private static int ToArrayIndex(Array array, int i)
        {
            return i % array.Length;
        }

        #region Properties

        public int HeadIndex { get; private set; }
        public int Length { get; private set; }
        public int Capacity
        {
            get { return m_internalArray.Length; } 
            set
            {
                if (value > Capacity)
                {
                    E[] data = new E[value];

                    int totalCopyLength = RealLength;

                    int fromIndex = ToArrayIndex(m_internalArray, HeadIndex);
                    int toIndex = ToArrayIndex(data, HeadIndex);

                    while (totalCopyLength > 0)
                    {
                        int copyLength = Math.Min(totalCopyLength, Math.Min(m_internalArray.Length - fromIndex, data.Length - toIndex));

                        Array.Copy(m_internalArray, fromIndex, data, toIndex, copyLength);
                        totalCopyLength -= copyLength;
                        fromIndex = ToArrayIndex(m_internalArray, fromIndex + copyLength);
                        toIndex = ToArrayIndex(data, toIndex + copyLength);
                    }

                    m_internalArray = data;
                }
                else if (value < Capacity)
                {
                    throw new NotImplementedException();
                }
            }
        }
        public int RealLength { get { return Length - HeadIndex; }}
        public E[] InternalArray { get { return m_internalArray; } }

        #endregion
    }
}