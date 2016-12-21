//
//  CNotificationCenter.cs
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

using LunarPlugin;

namespace LunarPluginInternal
{
    delegate void CNotificationDelegate(CNotification notification);

    struct CNotificationInfo
    {
        public string name;
        public CNotificationDelegate del;

        public CNotificationInfo(string name, CNotificationDelegate del)
        {
            this.name = name;
            this.del = del;
        }
    }

    class CNotificationCenter : ICDestroyable
    {
        private static CNotificationCenter s_sharedInstance;

        private CTimerManager m_timerManager;
        private IDictionary<string, CNotificationDelegateList> m_registerMap;
        private CObjectsPool<CNotification> m_notificatoinsPool;

        static CNotificationCenter()
        {
            s_sharedInstance = new CNotificationCenter(CTimerManager.SharedInstance);
        }

        public CNotificationCenter(CTimerManager timerManager)
        {
            m_timerManager = timerManager;
            m_registerMap = new Dictionary<string, CNotificationDelegateList>();
            m_notificatoinsPool = new CObjectsPool<CNotification>();
        }

        #region Shared instance

        public static void RegisterNotification(string name, CNotificationDelegate del)
        {
            s_sharedInstance.Register(name, del);
        }

        public static void RegisterNotifications(params CNotificationInfo[] list)
        {
            s_sharedInstance.Register(list);
        }

        public static void UnregisterNotification(string name, CNotificationDelegate del)
        {
            s_sharedInstance.Unregister(name, del);
        }

        public static void UnregisterNotifications(params CNotificationInfo[] list)
        {
            s_sharedInstance.Unregister(list);
        }

        public static void UnregisterNotifications(object target)
        {
            s_sharedInstance.UnregisterAll(target);
        }

        public static void UnregisterNotifications(CNotificationDelegate del)
        {
            s_sharedInstance.UnregisterAll(del);
        }

        public static void PostNotification(object sender, string name, params object[] data)
        {
            s_sharedInstance.Post(sender, name, data);
        }

        public static void PostNotificationImmediately(object sender, string name, params object[] data)
        {
            s_sharedInstance.PostImmediately(sender, name, data);
        }

        public static CNotificationCenter SharedInstance // TODO: decrease visiblity
        {
            get { return s_sharedInstance; }
        }

        #endregion
        
        public void Destroy()
        {
            CancelScheduledPosts();
        }
        
        public void Register(string name, CNotificationDelegate del)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (del == null)
            {
                throw new NullReferenceException("del");
            }
            
            CNotificationDelegateList list = FindList(name);
            if (list == null)
            {
                list = new CNotificationDelegateList();
                m_registerMap [name] = list;
            }
            
            list.Add(del);
        }

        public void Register(params CNotificationInfo[] list)
        {
            for (int i = 0; i < list.Length; ++i)
            {
                Register(list [i].name, list [i].del);
            }
        }
        
        public bool Unregister(string name, CNotificationDelegate del)
        {
            CNotificationDelegateList list = FindList(name);
            if (list != null)
            {
                return list.Remove(del);
            }
            
            return false;
        }

        public void Unregister(params CNotificationInfo[] list)
        {
            for (int i = 0; i < list.Length; ++i)
            {
                Unregister(list [i].name, list [i].del);
            }
        }
        
        public bool UnregisterAll(CNotificationDelegate del)
        {
            bool removed = false;
            foreach (KeyValuePair<string, CNotificationDelegateList> e in m_registerMap)
            {   
                CNotificationDelegateList list = e.Value;
                removed |= list.Remove(del);
            }
            return removed;
        }
        
        public bool UnregisterAll(Object target)
        {
            bool removed = false;
            foreach (KeyValuePair<string, CNotificationDelegateList> e in m_registerMap)
            {
                CNotificationDelegateList list = e.Value;
                removed |= list.RemoveAll(target);
            }
            return removed;
        }
        
        public void Post(Object sender, string name, params object[] data)
        {
            CNotificationDelegateList list = FindList(name);
            if (list != null && list.Count > 0)
            {
                CNotification notification = m_notificatoinsPool.NextObject();
                notification.Init(sender, name, data);
                
                SchedulePost(notification);
            }
        }
        
        public void PostImmediately(Object sender, string name, params object[] data)
        {   
            CNotificationDelegateList list = FindList(name);
            if (list != null && list.Count > 0)
            {
                CNotification notification = m_notificatoinsPool.NextObject();
                notification.Init(sender, name, data);
                
                list.NotifyDelegates(notification);
                notification.Recycle();
            }
        }
        
        public void PostImmediately(CNotification notification)
        {
            string name = notification.Name;
            CNotificationDelegateList list = FindList(name);
            if (list != null)
            {
                list.NotifyDelegates(notification);
            }
            notification.Recycle();
        }
        
        private CNotificationDelegateList FindList(string name)
        {
            CNotificationDelegateList list;
            if (m_registerMap.TryGetValue(name, out list))
            {
                return list;
            }
            
            return null;
        }
        
        private void SchedulePost(CNotification notification)
        {
            CTimer timer = m_timerManager.Schedule(PostCallback);
            timer.userData = notification;
        }
        
        private void CancelScheduledPosts()
        {
            m_timerManager.Cancel(PostCallback);
        }
        
        private void PostCallback(CTimer timer)
        {
            CNotification notification = timer.userData as CNotification;
            CAssert.IsNotNull(notification);
            
            PostImmediately(notification);
        }

        #if LUNAR_DEVELOPMENT

        public IDictionary<string, CNotificationDelegateList> RegistryMap
        {
            get { return m_registerMap; }
        }

        #endif
    }
    
    class CNotification : CObjectsPoolEntry
    {
        private IDictionary<string, object> m_dictionary;
        
        internal void Init(Object sender, string name, params object[] pairs)
        {
            Sender = sender;
            Name = name;

            CAssert.IsTrue(pairs.Length % 2 == 0);
            for (int i = 0; i < pairs.Length;)
            {
                string key = CClassUtils.Cast<string>(pairs [i++]);
                object value = pairs [i++];

                this.Set(key, value);
            }
        }

        public T Get<T>(string key)
        {
            object value = Get(key);
            if (value is T)
            {
                return (T)value;
            }
            return default(T);
        }

        public object Get(string key)
        {
            object value;
            if (m_dictionary != null && m_dictionary.TryGetValue(key, out value))
            {
                return value;
            }

            return null;
        }

        internal void Set(string key, object value)
        {
            if (m_dictionary == null)
            {
                m_dictionary = new Dictionary<string, object>(1);
            }
            m_dictionary [key] = value;
        }
        
        protected override void OnRecycleObject()
        {
            Sender = null;
            Name = null;

            if (m_dictionary != null)
            {
                m_dictionary.Clear();
            }
        }
        
        public string Name { get; private set; }
        public object Sender { get; private set; }
    }
    
    class CNotificationDelegateList : CBaseList<CNotificationDelegate>
    {
        public CNotificationDelegateList()
            : base(NullNotificationDelegate)
        {
        }
        
        public override bool Add(CNotificationDelegate del)
        {
            CAssert.IsFalse(Contains(del));
            return base.Add(del);
        }
        
        public bool RemoveAll(Object target)
        {
            bool removed = false;
            for (int i = 0; i < list.Count; ++i)
            {
                CNotificationDelegate del = list [i];
                if (del.Target == target)
                {
                    RemoveAt(i); // it's safe: the list size will be changed on the next update
                    removed = true;
                }
            }
            
            return removed;
        }
        
        public void NotifyDelegates(CNotification notification)
        {
            try
            {
                Lock();
                
                int delegatesCount = list.Count;
                for (int i = 0; i < delegatesCount; ++i)
                {
                    CNotificationDelegate del = list[i];
                    try
                    {
                        del(notification);
                    }
                    catch (Exception e)
                    {
                        CLog.error(e, "Error while notifying delegate");
                    }
                }
            }
            finally
            {
                Unlock();
            }
        }
        
        private static void NullNotificationDelegate(CNotification notification)
        {
        }
    }
}
