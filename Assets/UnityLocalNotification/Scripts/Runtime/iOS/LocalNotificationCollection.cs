using System;
using System.Collections.Generic;

namespace UnityLocalNotifications.iOS
{
    [Serializable]
    public class LocalNotificationCollection
    {
        public List<LocalNotification> _localNotifications;

        public List<LocalNotification> LocalNotifications()
        {
            return _localNotifications ?? new List<LocalNotification>();
        }
    }
}