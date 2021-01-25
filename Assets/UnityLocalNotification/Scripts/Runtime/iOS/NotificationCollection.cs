using System;
using System.Collections.Generic;

namespace UnityLocalNotifications.iOS
{
    [Serializable]
    public class NotificationCollection
    {
        public List<Notification> _localNotifications;

        public List<Notification> LocalNotifications()
        {
            return _localNotifications ?? new List<Notification>();
        }
    }
}