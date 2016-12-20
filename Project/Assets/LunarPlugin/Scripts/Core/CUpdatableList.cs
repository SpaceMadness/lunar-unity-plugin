//
//  UpdatableList.cs
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
    delegate void UpdatableDelegate(float delta);

    class UpdatableList : BaseUpdatableList<IUpdatable>, IDestroyable
    {
        public static readonly UpdatableList Null = new NullUpdatableList();
        private static readonly IUpdatable nullUpdatable = new NullUpdatable();

        public UpdatableList()
            : base(nullUpdatable) 
        {
        }

        public UpdatableList(int capacity)
            : base(nullUpdatable, capacity)
        {
        }

        public void Add(UpdatableDelegate del)
        {
            Add(new UpdatableDelegateClass(del));
        }

        public void Remove(UpdatableDelegate del)
        {
            for (int i = list.Count - 1; i >= 0; --i)
            {
                UpdatableDelegateClass delegateClass = list[i] as UpdatableDelegateClass;
                if (delegateClass != null && delegateClass.Delegate == del)
                {
                    RemoveAt(i);
                }
            }
        }

        protected UpdatableList(List<IUpdatable> list, IUpdatable nullUpdatable)
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

    internal sealed class UpdatableDelegateClass : IUpdatable
    {
        private UpdatableDelegate m_delegate;

        public UpdatableDelegateClass(UpdatableDelegate del)
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

        public UpdatableDelegate Delegate
        {
            get { return m_delegate; }
        }

        #endregion
    }

    internal sealed class NullUpdatableList : UpdatableList
    {
        public NullUpdatableList()
            : base(null, null)
        {
        }

        public override void Update(float delta)
        {
        }

        public override bool Add(IUpdatable updatable)
        {
            throw new InvalidOperationException("Can't add element to unmodifiable updatable list");
        }

        public override bool Remove(IUpdatable updatable)
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

        public override bool Contains(IUpdatable updatable)
        {
            return false;
        }

        public override int Count
        {
            get { return 0; }
        }
    }

    internal sealed class NullUpdatable : IUpdatable
    {
        public void Update(float delta)
        {
        }
    }
}
