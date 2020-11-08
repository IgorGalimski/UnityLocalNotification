using System.Collections;
using System.Collections.Generic;

namespace UnityLocalNotifications
{
    public class LocalNotificationEqualityComparer : IEqualityComparer<LocalNotification>
    {
        public bool Equals(LocalNotification firstLocalNotification, LocalNotification secondLocalNotification)
        {
            return firstLocalNotification.GetHashCode() == secondLocalNotification.GetHashCode();
        }

        public int GetHashCode(LocalNotification localNotification)
        {
            return localNotification.GetHashCode();
        }
    }
}