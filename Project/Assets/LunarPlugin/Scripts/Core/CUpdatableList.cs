//
//  CUpdatableList.cs
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

namespace LunarPluginInternal
{
    delegate void CUpdatableDelegate(float delta);

    class CUpdatableList : CBaseUpdatableList<ICUpdatable>, ICDestroyable
    {
        public static readonly CUpdatableList Null = new CNullUpdatableList();
        private static readonly ICUpdatable nullUpdatable = new CNullUpdatable();

        public CUpdatableList()
            : base(nullUpdatable) 
        {
        }

        public CUpdatableList(int capacity)
            : base(nullUpdatable, capacity)
        {
        }

        public void Add(CUpdatableDelegate del)
        {
            Add(new CUpdatableDelegateClass(del));
        }

        public void Remove(CUpdatableDelegate del)
        {
            for (int i = list.Count - 1; i >= 0; --i)
            {
                CUpdatableDelegateClass delegateClass = list[i] as CUpdatableDelegateClass;
                if (delegateClass != null && delegateClass.Delegate == del)
                {
                    RemoveAt(i);
                }
            }
        }

        protected CUpdatableList(List<ICUpdatable> list, ICUpdatable nullUpdatable)
            : base(list, nullUpdatable)
        {
        }

        public void Destroy()
        {
            if (Count > 0)
            {
                Clear();
            }
        }
    }

    internal sealed class CUpdatableDelegateClass : ICUpdatable
    {
        private CUpdatableDelegate m_delegate;

        public CUpdatableDelegateClass(CUpdatableDelegate del)
        {
            if (del == null)
            {
                throw new NullReferenceException("Delegate is null");
            }

            m_delegate = del;
        }

        #region IUpdatable implementation

        public void Update(float dt)
        {
            m_delegate(dt);
        }

        #endregion

        #region Properties

        public CUpdatableDelegate Delegate
        {
            get { return m_delegate; }
        }

        #endregion
    }

    internal sealed class CNullUpdatableList : CUpdatableList
    {
        public CNullUpdatableList()
            : base(null, null)
        {
        }

        public override void Update(float delta)
        {
        }

        public override bool Add(ICUpdatable updatable)
        {
            throw new InvalidOperationException("Can't add element to unmodifiable updatable list");
        }

        public override bool Remove(ICUpdatable updatable)
        {
            throw new InvalidOperationException("Can't remove element from unmodifiable updatable list");
        }

        public override void RemoveAt(int index)
        {
            throw new InvalidOperationException("Can't remove element from unmodifiable updatable list");
        }

        public override void Clear()
        {
            throw new InvalidOperationException("Can't clear unmodifiable updatable list");
        }

        public override bool Contains(ICUpdatable updatable)
        {
            return false;
        }

        public override int Count
        {
            get { return 0; }
        }
    }

    internal sealed class CNullUpdatable : ICUpdatable
    {
        public void Update(float delta)
        {
        }
    }
}
