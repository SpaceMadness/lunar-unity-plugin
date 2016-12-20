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
}
