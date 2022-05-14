#if UNITY_IOS || UNITY_EDITOR

using System;

namespace UnityLocalNotifications.Authorization
{
    [Flags]
    public enum AuthorizationOption
    {
        Badge = 1 << 0,
        Sound = 1 << 1,
        Alert = 1 << 2,
        CarPlay = 1 << 3,
        CriticalAlert = 1 << 4,
        Provisional = 1 << 6
    }
}

#endif