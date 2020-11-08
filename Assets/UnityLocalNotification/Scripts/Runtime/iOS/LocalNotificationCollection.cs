using System;

namespace UnityLocalNotifications.iOS
{
    [Serializable]
    public class LocalNotificationCollection
    {
        public LocalNotification[] _localNotifications;

        public LocalNotification[] LocalNotifications()
        {
            return _localNotifications ?? new LocalNotification[0];
        }
    }
}