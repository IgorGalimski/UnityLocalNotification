#if UNITY_IOS

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityLocalNotifications.Authorization;

namespace UnityLocalNotifications
{
    public static class LocalNotificationController
    {
        [DllImport("__Internal")]
        private static extern void RequestAuthorizationInternal(int options);
        
        public static void RequestAuthorization(AuthorizationOption authorizationOption)
        {
            try
            {
                RequestAuthorizationInternal((int) authorizationOption);
            }
            catch (Exception exception)
            {
                Debug.LogError("RequestAuthorization error: " + exception.Message);
            }
        }
    }
}

#endif