using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.UI;
using UnityLocalNotifications.Android;    
#endif

#if UNITY_IOS
using UnityLocalNotifications.Authorization;
#endif

namespace UnityLocalNotifications
{
    public static class LocalNotificationController
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void RequestAuthorizationInternal(int options,
            AuthorizationStatusCallbackDelegate authorizationStatusCallbackDelegate);

        [DllImport("__Internal")]
        private static extern string GetDeviceTokenInternal();

        [DllImport("__Internal")]
        private static extern void ClearBadgeInternal();

        [DllImport("__Internal")]
        private static extern void InitializeInternal(int notificationOptions, LocalNotificationDelegate notificationReceivedDelegate, DeviceTokenDelegate deviceTokenDelegate);

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

        private delegate void DeviceTokenDelegate(string localNotification);

        public static event Action<AuthorizationRequestResult> AuthorizationRequestResultEvent = status => { };

        public static event Action<string> DeviceTokenReceived = deviceToken => { };

#endif
        
#if UNITY_ANDROID
        private static AndroidJavaClass _notificationManager;
#endif
        
        public static event Action<LocalNotification> NotificationReceivedEvent = notification => { }; 
        
#if UNITY_IOS
        public static void Initialize(NotificationPresentationOptions notificationOptions)
        {
            try
            {
                InitializeInternal((int)notificationOptions, NotificationReceivedCallback, DeviceTokenReceivedCallback);
            }
            catch (Exception exception)
            {
                Debug.LogError("Initialize error: " + exception.Message);
            }
        }
#endif

#if UNITY_ANDROID
        public static void Initialize()
        {
            try
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                var activityClass = activity.Call<AndroidJavaObject>("getClass");
                var context = activity.Call<AndroidJavaObject>("getApplicationContext");
                
                var notificationReceivedCallback = new NotificationReceivedCallback();
                NotificationReceivedCallback.NotificationReceived += OnNotificationReceived;

                _notificationManager = new AndroidJavaClass("com.igorgalimski.unitylocalnotification.NotificationManager");
                _notificationManager.CallStatic("InitializeInternal", context, activityClass, notificationReceivedCallback);
            }
            catch (Exception e)
            {
            }
        }

        public static void CreateNotificationChannel(NotificationChannel notificationChannel)
        {
            try
            {
                _notificationManager.CallStatic("CreateChannelInternal", notificationChannel);
            }
            catch (Exception e)
            {
            }
        }
#endif

#if UNITY_IOS
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
        
        public static string GetDeviceToken()
        {
            try
            {
                return GetDeviceTokenInternal();
            }
            catch (Exception exception)
            {
                Debug.LogError("GetDeviceToken error: " + exception.Message);
            }

            return null;
        }
#endif

        public static void ScheduleLocalNotification(LocalNotification localNotification)
        {
            try
            {
#if UNITY_IOS
                var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(localNotification));
                Marshal.StructureToPtr(localNotification, ptr, false);
                
                ScheduleLocalNotificationInternal(ptr);
#endif
#if UNITY_ANDROID
                var androidNotification = new AndroidNotification
                {
                    Title = localNotification.Title,
                    Body = localNotification.Body,
                    Data = localNotification.Data,
                    FireInSeconds = localNotification.FireInSeconds
                };

                _notificationManager.CallStatic("ScheduleLocalNotificationInternal", androidNotification);
#endif

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
#if UNITY_IOS
                LocalNotification? localNotification;
                IntPtr ptr = GetLastNotificationInternal();

                if (ptr != IntPtr.Zero)
                {
                    localNotification = Marshal.PtrToStructure<LocalNotification>(ptr);
                    return localNotification.Value;
                }
#endif
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
#if UNITY_IOS
                ClearBadgeInternal();
#endif
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
#if UNITY_IOS
                RemoveScheduledNotificationsInternal();
#endif
                
#if UNITY_ANDROID
                _notificationManager.CallStatic("RemoveScheduledNotificationsInternal");
#endif
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
#if UNITY_IOS
                RemoveReceivedNotificationsInternal();
#endif
#if UNITY_ANDROID
                _notificationManager.CallStatic("RemoveReceivedNotificationsInternal");
#endif
            }
            catch (Exception exception)
            {
                Debug.LogError("RemoveDeliveredNotifications error: " + exception.Message);
            }
        }
        
#if UNITY_ANDROID
        private static void OnNotificationReceived()
        {
            var notification = _notificationManager.GetStatic<AndroidJavaObject>("LastReceivedNotification");
            var title = notification.Call<string>("GetTitle");
            var body = notification.Call<string>("GetBody");
            var data = notification.Call<string>("GetData");
            
            var localNotification = new LocalNotification();
            localNotification.Title = title;
            localNotification.Body = body;
            localNotification.Data = data;
            
            NotificationReceivedEvent?.Invoke(localNotification);
        }
#endif
        
#if UNITY_IOS
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
        
        [MonoPInvokeCallback(typeof(DeviceTokenDelegate))]
        private static void DeviceTokenReceivedCallback(string deviceToken)
        {
            DeviceTokenReceived(deviceToken);
        }
#endif
    }
}