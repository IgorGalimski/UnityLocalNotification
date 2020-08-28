#if UNITY_IOS

using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityLocalNotifications.Authorization;

namespace UnityLocalNotifications
{
    public static class LocalNotificationController
    {
        [DllImport("__Internal")]
        private static extern void RequestAuthorizationInternal(int options,
            AuthorizationStatusCallbackDelegate authorizationStatusCallbackDelegate);
        
        [DllImport("__Internal")]
        private static extern void ClearBadgeInternal();

        [DllImport("__Internal")]
        private static extern void SetCallbacksInternal(LocalNotificationAddedDelegate localNotificationSuccessAddedDelegate,
            LocalNotificationAddedDelegate localNotificationFailAddedDelegate);

        [DllImport("__Internal")]
        private static extern void ScheduleLocalNotificationInternal(IntPtr localNotification);

        private delegate void AuthorizationStatusCallbackDelegate(AuthorizationRequestResult requestResult);

        private delegate void LocalNotificationAddedDelegate(LocalNotification localNotification);

        public static event Action<AuthorizationRequestResult> AuthorizationRequestResultEvent = status => { };

        public static event Action<LocalNotification> LocalNotificationAddedSuccessEvent = notification => { };
        public static event Action<LocalNotification> LocalNotificationAddedFailEventEvent = notification => { };

        public static void SetCallbacks()
        {
            SetCallbacksInternal(LocalNotificationAddedSuccessCallback, LocalNotificationAddedFailCallback);
        }
        
        public static void RequestAuthorization(AuthorizationOption authorizationOption)
        {
            try
            {
                RequestAuthorizationInternal((int) authorizationOption, AuthorizationRequestResultCallback);
            }
            catch (Exception exception)
            {
                Debug.LogError("RequestAuthorization error: " + exception.Message);
            }
        }

        public static void ScheduleLocalNotification(LocalNotification localNotification)
        {
            try
            {
                var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(localNotification));
                Marshal.StructureToPtr(localNotification, ptr, false);
                
                ScheduleLocalNotificationInternal(ptr);
            }
            catch (Exception exception)
            {
                Debug.LogError("ScheduleLocalNotification error: " + exception.Message);
            }
        }

        [MonoPInvokeCallback(typeof(AuthorizationStatusCallbackDelegate))]
        private static void LocalNotificationAddedSuccessCallback(LocalNotification localNotification)
        {
            LocalNotificationAddedSuccessEvent(localNotification);
        }

        [MonoPInvokeCallback(typeof(AuthorizationStatusCallbackDelegate))]
        private static void LocalNotificationAddedFailCallback(LocalNotification localNotification)
        {
            LocalNotificationAddedFailEventEvent(localNotification);
        }
        
        [MonoPInvokeCallback(typeof(AuthorizationStatusCallbackDelegate))]
        private static void AuthorizationRequestResultCallback(AuthorizationRequestResult authorizationRequestResult)
        {
            AuthorizationRequestResultEvent(authorizationRequestResult);
        }
    }
}

#endif