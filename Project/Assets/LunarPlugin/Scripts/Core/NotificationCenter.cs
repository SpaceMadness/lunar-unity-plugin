//
//  NotificationCenter.cs
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
    delegate void NotificationDelegate(Notification notification);

    struct NotificationInfo
    {
        public string name;
        public NotificationDelegate del;

        public NotificationInfo(string name, NotificationDelegate del)
        {
            this.name = name;
            this.del = del;
        }
    }

    class NotificationCenter : IDestroyable
    {
        private static NotificationCenter s_sharedInstance;

        private TimerManager m_timerManager;
        private IDictionary<string, NotificationDelegateList> m_registerMap;
        private ObjectsPool<Notification> m_notificatoinsPool;

        static NotificationCenter()
        {
            s_sharedInstance = new NotificationCenter(TimerManager.SharedInstance);
        }

        public NotificationCenter(TimerManager timerManager)
        {
            m_timerManager = timerManager;
            m_registerMap = new Dictionary<string, NotificationDelegateList>();
            m_notificatoinsPool = new ObjectsPool<Notification>();
        }

        #region Shared instance

        public static void RegisterNotification(string name, NotificationDelegate del)
        {
            s_sharedInstance.Register(name, del);
        }

        public static void RegisterNotifications(params NotificationInfo[] list)
        {
            s_sharedInstance.Register(list);
        }

        public static void UnregisterNotification(string name, NotificationDelegate del)
        {
            s_sharedInstance.Unregister(name, del);
        }

        public static void UnregisterNotifications(params NotificationInfo[] list)
        {
            s_sharedInstance.Unregister(list);
        }

        public static void UnregisterNotifications(object target)
        {
            s_sharedInstance.UnregisterAll(target);
        }

        public static void UnregisterNotifications(NotificationDelegate del)
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

        public static NotificationCenter SharedInstance // TODO: decrease visiblity
        {
            get { return s_sharedInstance; }
        }

        #endregion
        
        public void Destroy()
        {
            CancelScheduledPosts();
        }
        
        public void Register(string name, NotificationDelegate del)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (del == null)
            {
                throw new NullReferenceException("del");
            }
            
            NotificationDelegateList list = FindList(name);
            if (list == null)
            {
                list = new NotificationDelegateList();
                m_registerMap [name] = list;
            }
            
            list.Add(del);
        }

        public void Register(params NotificationInfo[] list)
        {
            for (int i = 0; i < list.Length; ++i)
            {
                Register(list [i].name, list [i].del);
            }
        }
        
        public bool Unregister(string name, NotificationDelegate del)
        {
            NotificationDelegateList list = FindList(name);
            if (list != null)
            {
                return list.Remove(del);
            }
            
            return false;
        }

        public void Unregister(params NotificationInfo[] list)
        {
            for (int i = 0; i < list.Length; ++i)
            {
                Unregister(list [i].name, list [i].del);
            }
        }
        
        public bool UnregisterAll(NotificationDelegate del)
        {
            bool removed = false;
            foreach (KeyValuePair<string, NotificationDelegateList> e in m_registerMap)
            {   
                NotificationDelegateList list = e.Value;
                removed |= list.Remove(del);
            }
            return removed;
        }
        
        public bool UnregisterAll(Object target)
        {
            bool removed = false;
            foreach (KeyValuePair<string, NotificationDelegateList> e in m_registerMap)
            {
                NotificationDelegateList list = e.Value;
                removed |= list.RemoveAll(target);
            }
            return removed;
        }
        
        public void Post(Object sender, string name, params object[] data)
        {
            NotificationDelegateList list = FindList(name);
            if (list != null && list.Count > 0)
            {
                Notification notification = m_notificatoinsPool.NextObject();
                notification.Init(sender, name, data);
                
                SchedulePost(notification);
            }
        }
        
        public void PostImmediately(Object sender, string name, params object[] data)
        {   
            NotificationDelegateList list = FindList(name);
            if (list != null && list.Count > 0)
            {
                Notification notification = m_notificatoinsPool.NextObject();
                notification.Init(sender, name, data);
                
                list.NotifyDelegates(notification);
                notification.Recycle();
            }
        }
        
        public void PostImmediately(Notification notification)
        {
            string name = notification.Name;
            NotificationDelegateList list = FindList(name);
            if (list != null)
            {
                list.NotifyDelegates(notification);
            }
            notification.Recycle();
        }
        
        private NotificationDelegateList FindList(string name)
        {
            NotificationDelegateList list;
            if (m_registerMap.TryGetValue(name, out list))
            {
                return list;
            }
            
            return null;
        }
        
        private void SchedulePost(Notification notification)
        {
            Timer timer = m_timerManager.Schedule(PostCallback);
            timer.userData = notification;
        }
        
        private void CancelScheduledPosts()
        {
            m_timerManager.Cancel(PostCallback);
        }
        
        private void PostCallback(Timer timer)
        {
            Notification notification = timer.userData as Notification;
            Assert.IsNotNull(notification);
            
            PostImmediately(notification);
        }

        #if LUNAR_DEVELOPMENT

        public IDictionary<string, NotificationDelegateList> RegistryMap
        {
            get { return m_registerMap; }
        }

        #endif
    }
    
    class Notification : ObjectsPoolEntry
    {
        private IDictionary<string, object> m_dictionary;
        
        internal void Init(Object sender, string name, params object[] pairs)
        {
            Sender = sender;
            Name = name;

            Assert.IsTrue(pairs.Length % 2 == 0);
            for (int i = 0; i < pairs.Length;)
            {
                string key = ClassUtils.Cast<string>(pairs [i++]);
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
    
    class NotificationDelegateList : BaseList<NotificationDelegate>
    {
        public NotificationDelegateList()
            : base(NullNotificationDelegate)
        {
        }
        
        public override bool Add(NotificationDelegate del)
        {
            Assert.IsFalse(Contains(del));
            return base.Add(del);
        }
        
        public bool RemoveAll(Object target)
        {
            bool removed = false;
            for (int i = 0; i < list.Count; ++i)
            {
                NotificationDelegate del = list [i];
                if (del.Target == target)
                {
                    RemoveAt(i); // it's safe: the list size will be changed on the next update
                    removed = true;
                }
            }
            
            return removed;
        }
        
        public void NotifyDelegates(Notification notification)
        {
            try
            {
                Lock();
                
                int delegatesCount = list.Count;
                for (int i = 0; i < delegatesCount; ++i)
                {
                    NotificationDelegate del = list[i];
                    try
                    {
                        del(notification);
                    }
                    catch (Exception e)
                    {
                        Log.error(e, "Error while notifying delegate");
                    }
                }
            }
            finally
            {
                Unlock();
            }
        }
        
        private static void NullNotificationDelegate(Notification notification)
        {
        }
    }
}
