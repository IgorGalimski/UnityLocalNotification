using System.Collections;
using System.Collections.Generic;

namespace UnityLocalNotifications
{
    public class LocalNotificationEqualityComparer : IEqualityComparer<Notification>
    {
        public bool Equals(Notification firstNotification, Notification secondNotification)
        {
            return firstNotification.GetHashCode() == secondNotification.GetHashCode();
        }

        public int GetHashCode(Notification notification)
        {
            return notification.GetHashCode();
        }
    }
}