//
//  BaseObject.cs
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

using System.Collections;

namespace LunarPluginInternal
{
    abstract class BaseObject : IDestroyable
    {
        protected void RegisterNotifications(params NotificationInfo[] list)
        {
            NotificationCenter.RegisterNotifications(list);
        }

        protected void RegisterNotification(string name, NotificationDelegate del)
        {
            NotificationCenter.RegisterNotification(name, del);
        }

        protected void UnregisterNotifications(params NotificationInfo[] list)
        {
            NotificationCenter.UnregisterNotifications(list);
        }

        protected void UnregisterNotification(string name, NotificationDelegate del)
        {
            NotificationCenter.UnregisterNotification(name, del);
        }

        protected void UnregisterNotifications(NotificationDelegate del)
        {
            NotificationCenter.UnregisterNotifications(del);
        }

        protected void UnregisterNotifications()
        {
            NotificationCenter.UnregisterNotifications(this);
        }

        protected void PostNotification(string name, params object[] data)
        {
            NotificationCenter.PostNotification(this, name, data);
        }

        protected void PostNotificationImmediately(string name, params object[] data)
        {
            NotificationCenter.PostNotificationImmediately(this, name, data);
        }

        #region IDestroyable

        public virtual void Destroy()
        {
            UnregisterNotifications();
        }

        #endregion
    }
}