using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityLocalNotifications.iOS;
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

        private const string PENDING_NOTIFICATIONS_KEY = nameof(PENDING_NOTIFICATIONS_KEY);

        private static LocalNotificationCollection _previousPendingNotifications;
        
        [DllImport("__Internal")]
        private static extern void RequestAuthorizationInternal(int options,
            AuthorizationStatusCallbackDelegate authorizationStatusCallbackDelegate);

        [DllImport("__Internal")]
        private static extern string GetDeviceTokenInternal();

        [DllImport("__Internal")]
        private static extern void ClearBadgeInternal();

        [DllImport("__Internal")]
        private static extern void InitializeInternal(int notificationOptions,
            LocalNotificationDelegate notificationReceivedDelegate, DeviceTokenDelegate deviceTokenDelegate);

        [DllImport("__Internal")]
        private static extern void ScheduleLocalNotificationInternal(IntPtr localNotification);

        [DllImport("__Internal")]
        private static extern IntPtr GetLastNotificationInternal();

        [DllImport("__Internal")]
        private static extern void RemoveScheduledNotificationsInternal();

        [DllImport("__Internal")]
        private static extern void RemoveReceivedNotificationsInternal();

        [DllImport("__Internal")]
        private static extern bool AreNotificationEnabledInternal();

        [DllImport("__Internal")]
        private static extern void RequestNotificationEnabledStatusInternal(RequestNotificationsEnabledStatusDelegate notificationsEnabledStatusDelegate);

        [DllImport("__Internal")]
        private static extern int GetPendingNotificationsCountInternal();

        [DllImport("__Internal")]
        private static extern IntPtr GetPendingNotificationInternal(int index);

        private delegate void AuthorizationStatusCallbackDelegate(AuthorizationRequestResult requestResult);

        private delegate void LocalNotificationDelegate(LocalNotification localNotification);

        private delegate void DeviceTokenDelegate(string localNotification);

        private delegate void RequestNotificationsEnabledStatusDelegate(bool enabled);

        public static event Action<AuthorizationRequestResult> AuthorizationRequestResultEvent = status => { };

        public static event Action<string> DeviceTokenReceived = deviceToken => { };

        public static event Action<bool> NotificationEnabledStatusReceived = enabled => { };

#endif
        
#if UNITY_ANDROID
        private static AndroidJavaClass _notificationManager;
        private static AndroidJavaClass GetNotificationManager()
        {
            if (_notificationManager == null)
            {
                Initialize();
            }

            return _notificationManager;
        }
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

        public static void RequestNotificationEnabledStatus()
        {
            try
            {
                RequestNotificationEnabledStatusInternal(DeviceTokenReceivedCallback);
            }
            catch (Exception exception)
            {
                Debug.LogError("RequestNotificationEnabledStatus error: " + exception.Message);
            }
        }

        public static void UpdatePreviousPendingNotifications()
        {
            try
            {
                var previousPendingNotificationsString = PlayerPrefs.GetString(PENDING_NOTIFICATIONS_KEY);
                if (!string.IsNullOrEmpty(previousPendingNotificationsString))
                {
                    _previousPendingNotifications = JsonUtility.FromJson<LocalNotificationCollection>(previousPendingNotificationsString);
                }
                else
                {
                    _previousPendingNotifications = new LocalNotificationCollection();
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("ParsePreviousPendingNotifications error: " + exception.Message);
            }
        }

        public static void SavePendingNotifications()
        {
            try
            {
                var pendingNotifications = GetPendingNotifications();

                var pendingNotificationsString = JsonUtility.ToJson(pendingNotifications);
                
                PlayerPrefs.SetString(PENDING_NOTIFICATIONS_KEY, pendingNotificationsString);
                PlayerPrefs.Save();
            }
            catch (Exception exception)
            {
                Debug.LogError("SavePendingNotifications error: " + exception.Message);
            }
        }
#endif

#if UNITY_ANDROID
        public static void Initialize()
        {
            try
            {
                var notificationReceivedCallback = new NotificationReceivedCallback();
                NotificationReceivedCallback.NotificationReceived += OnNotificationReceived;

                _notificationManager = new AndroidJavaClass("com.igorgalimski.unitylocalnotification.NotificationManager");
                _notificationManager.CallStatic("InitializeInternal", notificationReceivedCallback);
            }
            catch (Exception exception)
            {
                Debug.LogError("Initialize error: " + exception.Message);
            }
        }

        public static void CreateNotificationChannel(NotificationChannel notificationChannel)
        {
            try
            {
                GetNotificationManager().CallStatic("CreateChannelInternal", notificationChannel);
            }
            catch (Exception exception)
            {
                Debug.LogError("CreateNotificationChannel error: " + exception.Message);
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
                localNotification.ID = localNotification.ID ?? Guid.NewGuid().ToString();
                
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

                GetNotificationManager().CallStatic("ScheduleLocalNotificationInternal", androidNotification);
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
                
#if UNITY_ANDROID
                
                var notificationIntent = GetNotificationManager().CallStatic<AndroidJavaObject>("GetOpenedNotificationInternal");
                return ParseNotificationFromAndroidJavaObject(notificationIntent);
#endif
            }
            catch (Exception exception)
            {
                Debug.LogError("GetLastNotification error: " + exception.Message);
            }
            
            return null;
        }

        public static List<LocalNotification> GetReceivedNotifications()
        {
            try
            {
#if UNITY_IOS
                var deliveredNotifications = new List<LocalNotification>();

                deliveredNotifications =
                    _previousPendingNotifications.LocalNotifications().Except(GetPendingNotifications(), new LocalNotificationEqualityComparer()).ToList();

                return deliveredNotifications;
#endif

#if UNITY_ANDROID
                var notificationIntent = GetNotificationManager().CallStatic<AndroidJavaObject>("GetReceivedNotificationsListInternal");
                return ParseNotificationsFromAndroidJavaObject(notificationIntent);
#endif
            }
            catch (Exception exception)
            {
                Debug.LogError("GetReceivedNotifications error: " + exception.Message);
            }
            
            return null;
        }
#if UNITY_IOS
        private static List<LocalNotification> GetPendingNotifications()
        {
            var size = GetPendingNotificationsCountInternal();
            var pendingNotifications = new List<LocalNotification>();
                
            for (var i = 0; i < size; i++)
            {
                LocalNotification data;
                var ptr = GetPendingNotificationInternal(i);

                if (ptr != IntPtr.Zero)
                {
                    data = (LocalNotification)Marshal.PtrToStructure(ptr, typeof(LocalNotification));
                    pendingNotifications.Add(data);
                }
            }

            return pendingNotifications;
        }
#endif
        public static void ClearReceivedNotifications()
        {
            try
            {
#if UNITY_IOS

#endif
                
#if UNITY_ANDROID
                GetNotificationManager().CallStatic("ClearReceivedNotificationsListInternal");
#endif
            }
            catch (Exception exception)
            {
                Debug.LogError("ClearReceivedNotifications error: " + exception.Message);
            }
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
                GetNotificationManager().CallStatic("RemoveScheduledNotificationsInternal");
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
                GetNotificationManager().CallStatic("RemoveReceivedNotificationsInternal");
#endif
            }
            catch (Exception exception)
            {
                Debug.LogError("RemoveDeliveredNotifications error: " + exception.Message);
            }
        }

        public static bool AreNotificationsEnabled()
        {
            try
            {
#if UNITY_IOS
                return AreNotificationEnabledInternal();
#endif
#if UNITY_ANDROID
                return GetNotificationManager().CallStatic<bool>("AreNotificationEnabledInternal");
#endif
            }
            catch (Exception exception)
            {
                Debug.LogError("RemoveDeliveredNotifications error: " + exception.Message);
            }

            return false;
        }
        
#if UNITY_ANDROID
        private static void OnNotificationReceived()
        {
            var notification = GetNotificationManager().GetStatic<AndroidJavaObject>("LastReceivedNotification");
            
            NotificationReceivedEvent?.Invoke((LocalNotification)ParseNotificationFromAndroidJavaObject(notification));
        }

        private static LocalNotification? ParseNotificationFromAndroidJavaObject(AndroidJavaObject notification)
        {
            if (notification == null)
            {
                return null;
            }
            
            var title = notification.Call<string>("GetTitle");
            var body = notification.Call<string>("GetBody");
            var data = notification.Call<string>("GetData");
            
            var localNotification = new LocalNotification();
            localNotification.Title = title;
            localNotification.Body = body;
            localNotification.Data = data;

            return localNotification;
        }

        private static List<LocalNotification> ParseNotificationsFromAndroidJavaObject(AndroidJavaObject notificationsJavaObject)
        {
            var notifications = new List<LocalNotification>();

            var count = notificationsJavaObject.Call<int>("size");
            
            for (int i = 0; i < count; i++)
            {
                var notification = notificationsJavaObject.Call<AndroidJavaObject>("get", i);

                var localNotification = ParseNotificationFromAndroidJavaObject(notification);

                if (localNotification.HasValue)
                {
                    notifications.Add(localNotification.Value);
                }
            }

            return notifications;
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
        
        [MonoPInvokeCallback(typeof(RequestNotificationsEnabledStatusDelegate))]
        private static void DeviceTokenReceivedCallback(bool enabled)
        {
            NotificationEnabledStatusReceived(enabled);
        }
#endif
    }
}