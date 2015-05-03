using UnityEngine;

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