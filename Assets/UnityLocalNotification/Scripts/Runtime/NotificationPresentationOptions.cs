using System;

namespace UnityLocalNotifications
{
    [Flags]
    public enum NotificationPresentationOptions
    {
        Badge = 1,
        Sound = 1 << 1,
        Alert = 1 << 2
    }
}