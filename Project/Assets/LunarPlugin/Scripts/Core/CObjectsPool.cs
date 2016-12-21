//
//  CObjectsPool.cs
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
    internal interface ICObjectsPool
    {
        void Recycle(CObjectsPoolEntry entry);
    }

    class CObjectsPool<T> : CFastList<CObjectsPoolEntry>, ICObjectsPool, ICDestroyable
        where T : CObjectsPoolEntry, new()
    {
        public CObjectsPool()
        {
        }

        public T NextAutoRecycleObject()
        {
            return (T)NextObject().AutoRecycle();
        }

        public virtual T NextObject()
        {
            CObjectsPoolEntry first = RemoveFirstItem();
            if (first == null)
            {
                first = CreateObject();
            }

            first.pool = this;
            first.recycled = false;

            return (T)first;
        }

        public virtual void Recycle(CObjectsPoolEntry e)
        {
            CAssert.IsInstanceOfType<T>(e);
            CAssert.AreSame(this, e.pool);

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
    
    class CObjectsPoolConcurrent<T> : CObjectsPool<T>
        where T : CObjectsPoolEntry, new()
    {
        public override T NextObject()
        {
            lock (this)
            {
                return base.NextObject();
            }
        }

        public override void Recycle(CObjectsPoolEntry e)
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

    class CObjectsPoolEntry : CFastListNode
    {
        internal ICObjectsPool pool;
        internal bool recycled;

        public CObjectsPoolEntry AutoRecycle()
        {
            CTimerManager.ScheduleTimer(Recycle);
            return this;
        }

        public void Recycle()
        {
            if (pool != null)
            {
                CAssert.IsFalse(recycled);
                recycled = true;

                pool.Recycle(this);
            }
            
            OnRecycleObject();
        }

        protected virtual void OnRecycleObject()
        {
        }
    }
}
