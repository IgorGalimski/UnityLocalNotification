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
        private static extern void SetCallbacksInternal(LocalNotificationDelegate notificationReceivedDelegate);

        [DllImport("__Internal")]
        private static extern void ScheduleLocalNotificationInternal(IntPtr localNotification);
        
        [DllImport("__Internal")]
        private static extern IntPtr GetLastNotificationInternal();
        
        [DllImport("__Internal")]
        private static extern void RemoveScheduledNotificationsInternal();
        
        [DllImport("__Internal")]
        private static extern void RemoveReceivedNotificationsInternal();

        private delegate void AuthorizationStatusCallbackDelegate(AuthorizationRequestResult requestResult);

        private delegate void LocalNotificationDelegate(LocalNotification localNotification);

        public static event Action<AuthorizationRequestResult> AuthorizationRequestResultEvent = status => { };
        public static event Action<LocalNotification> NotificationReceivedEvent = notification => { }; 

        public static void SetCallbacks()
        {
            SetCallbacksInternal(NotificationReceivedCallback);
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

                FreePtr(ptr);
            }
            catch (Exception exception)
            {
                Debug.LogError("ScheduleLocalNotification error: " + exception.Message);
            }
        }

        public static LocalNotification? GetLastNotification()
        {
            try
            {
                LocalNotification? localNotification;
                IntPtr ptr = GetLastNotificationInternal();

                if (ptr != IntPtr.Zero)
                {
                    localNotification = Marshal.PtrToStructure<LocalNotification>(ptr);
                    FreePtr(ptr);
                    return localNotification.Value;
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("GetLastNotification error: " + exception.Message);
            }
            
            return null;
        }

        public static void ClearBadge()
        {
            try
            {
                ClearBadgeInternal();
            }
            catch (Exception exception)
            {
                Debug.LogError("ClearBadge error: " + exception.Message);
            }
        }

        public static void RemoveScheduledNotifications()
        {
            try
            {
                RemoveScheduledNotificationsInternal();
            }
            catch (Exception exception)
            {
                Debug.LogError("RemoveScheduledNotifications error: " + exception.Message);
            }
        }

        public static void RemoveDeliveredNotifications()
        {
            try
            {
                RemoveReceivedNotificationsInternal();
            }
            catch (Exception exception)
            {
                Debug.LogError("RemoveDeliveredNotifications error: " + exception.Message);
            }
        }

        [MonoPInvokeCallback(typeof(AuthorizationStatusCallbackDelegate))]
        private static void AuthorizationRequestResultCallback(AuthorizationRequestResult authorizationRequestResult)
        {
            AuthorizationRequestResultEvent(authorizationRequestResult);
        }
        
        [MonoPInvokeCallback(typeof(LocalNotificationDelegate))]
        private static void NotificationReceivedCallback(LocalNotification localNotification)
        {
            NotificationReceivedEvent(localNotification);
        }

        private static void FreePtr(IntPtr intPtr)
        {
            try
            {
                Marshal.FreeHGlobal(intPtr);
            }
            catch (Exception exception)
            {
                Debug.LogError("FreePtr error: " + exception.Message);
            }
        }
    }
}

#endif