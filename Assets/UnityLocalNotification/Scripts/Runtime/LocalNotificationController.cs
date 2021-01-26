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

        [DllImport("__Internal")]
        private static extern void RequestPushAuthorizationInternal(int options,
            AuthorizationStatusCallbackDelegate authorizationStatusCallbackDelegate);

        [DllImport("__Internal")]
        private static extern string GetDeviceTokenInternal();

        [DllImport("__Internal")]
        private static extern void ClearBadgeInternal();

        [DllImport("__Internal")]
        private static extern void InitializeInternal(int notificationOptions,
            LocalNotificationDelegate notificationReceivedDelegate, 
            DeviceTokenDelegate deviceTokenDelegate,
            PendingNotificationsUpdatedDelegate pendingNotificationsUpdatedDelegate);

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

        [DllImport("__Internal")]
        private static extern void UpdateScheduledNotificationListInternal();

        private delegate void AuthorizationStatusCallbackDelegate(AuthorizationRequestResult requestResult);

        private delegate void LocalNotificationDelegate(Notification localNotification);

        private delegate void DeviceTokenDelegate(string localNotification);

        private delegate void RequestNotificationsEnabledStatusDelegate(bool enabled);
        
        private delegate void PendingNotificationsUpdatedDelegate();

        public static event Action<AuthorizationRequestResult> AuthorizationRequestResultEvent = status => { };

        public static event Action<string> DeviceTokenReceived = deviceToken => { };

        public static event Action<bool> NotificationEnabledStatusReceived = enabled => { };

        public static event Action PendingNotificationUpdated = () => { };

#endif
        public static List<Notification> ReceivedNotifications { get; private set; }
        
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
        
        public static List<Notification> ForegroundReceivedNotifications { get; private set; } = new List<Notification>();
        
        public static event Action<Notification> NotificationReceivedEvent = notification => { }; 
        
#if UNITY_IOS
        public static void Initialize(NotificationPresentationOptions notificationOptions)
        {
            try
            {
                ForegroundReceivedNotifications = new List<Notification>();
                
                InitializeInternal((int)notificationOptions, NotificationReceivedCallback, DeviceTokenReceivedCallback, PendingNotificationsUpdatedCallback);

                GetReceivedNotifications(OnGetReceivedNotifications);

                SavePendingNotifications();
            }
            catch (Exception exception)
            {
                Debug.LogError("Initialize error: " + exception.Message);
            }
        }

        private static void OnGetReceivedNotifications(List<Notification> localNotifications)
        {
            ReceivedNotifications = localNotifications;
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

        private static void RequestUpdatePendingNotifications()
        {
            try
            {
                UpdateScheduledNotificationListInternal();
            }
            catch (Exception exception)
            {
                Debug.LogError("ParsePreviousPendingNotifications error: " + exception.Message);
            }
        }

        private static void SavePendingNotifications()
        {
            try
            {
                PendingNotificationUpdated += OnPendingNotificationUpdated;
                
                RequestUpdatePendingNotifications();

                void OnPendingNotificationUpdated()
                {
                    PendingNotificationUpdated -= OnPendingNotificationUpdated;
                    
                    var pendingNotifications = GetPendingNotifications();
                    var localNotificationCollection = new NotificationCollection();
                    localNotificationCollection._localNotifications = pendingNotifications;

                    var pendingNotificationsString = JsonUtility.ToJson(localNotificationCollection);

                    PlayerPrefs.SetString(PENDING_NOTIFICATIONS_KEY, pendingNotificationsString);
                    PlayerPrefs.Save();
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("SavePendingNotifications error: " + exception.Message);
            }
        }

        private static void GetReceivedNotifications(Action<List<Notification>> receivedNotifications)
        {
            try
            {
                if (receivedNotifications == null)
                {
                    Debug.LogError("GetReceivedNotifications error: callback is null");
                    
                    return;
                }
                
                var previousPendingNotificationsString = PlayerPrefs.GetString(PENDING_NOTIFICATIONS_KEY);
                var savedPendingNotifications = !string.IsNullOrEmpty(previousPendingNotificationsString) 
                    ? JsonUtility.FromJson<NotificationCollection>(previousPendingNotificationsString) 
                    : new NotificationCollection();

                PendingNotificationUpdated += OnPendingNotificationUpdated;
                
                RequestUpdatePendingNotifications();

                void OnPendingNotificationUpdated()
                {
                    PendingNotificationUpdated -= OnPendingNotificationUpdated;
                    
                    var pendingNotifications = GetPendingNotifications();

                    var deliveredNotifications =
                        savedPendingNotifications.LocalNotifications().Except(pendingNotifications, 
                            new LocalNotificationEqualityComparer()).ToList();
                    
                    receivedNotifications.Invoke(deliveredNotifications);
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("GetReceivedNotifications error: " + exception.Message);
            }
        }
#endif

#if UNITY_ANDROID
        private static void Initialize()
        {
            try
            {
                var notificationReceivedCallback = new NotificationReceivedCallback();
                NotificationReceivedCallback.NotificationReceived += OnNotificationReceived;

                _notificationManager = new AndroidJavaClass("com.igorgalimski.unitylocalnotification.NotificationManager");
                _notificationManager.CallStatic("InitializeInternal", notificationReceivedCallback);

                ReceivedNotifications = GetReceivedNotifications();
                
                ForegroundReceivedNotifications = new List<Notification>();
                ClearReceivedNotifications();
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
                RequestPushAuthorizationInternal((int) authorizationOption, AuthorizationRequestResultCallback);
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

#if UNITY_IOS
        public static void ScheduleLocalNotification(Notification localNotification)
        {
            try
            {

                var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(localNotification));
                Marshal.StructureToPtr(localNotification, ptr, false);
                
                ScheduleLocalNotificationInternal(ptr);

                SavePendingNotifications();
            }
            catch (Exception exception)
            {
                Debug.LogError("ScheduleLocalNotification error: " + exception.Message);
            }
        }
#endif

#if UNITY_ANDROID
        public static void ScheduleLocalNotification(Notification notification, 
            bool autoCancel = true, string smallIconId = null, string largeIconId = null)
        {
            try
            {
                var androidNotification = new AndroidNotification
                {
                    Title = notification.Title,
                    AutoCancel = autoCancel,
                    Body = notification.Body,
                    Data = notification.Data,
                    SmallIconId = smallIconId,
                    LargeIconId = largeIconId,
                    FireInSeconds = notification.FireInSeconds
                };

                GetNotificationManager().CallStatic("ScheduleLocalNotificationInternal", androidNotification);
            }
        
            catch (Exception exception)
            {
                Debug.LogError("ScheduleLocalNotification error: " + exception.Message);
            }
        }
#endif

        public static Notification? GetLastNotification()
        {
            try
            {
#if UNITY_IOS
                Notification? localNotification;
                IntPtr ptr = GetLastNotificationInternal();

                if (ptr != IntPtr.Zero)
                {
                    localNotification = Marshal.PtrToStructure<Notification>(ptr);
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
        
#if UNITY_ANDROID     
        public static List<Notification> GetReceivedNotifications()
        {
            try
            {
                var notificationIntent = GetNotificationManager().CallStatic<AndroidJavaObject>("GetReceivedNotificationsListInternal");
                return ParseNotificationsFromAndroidJavaObject(notificationIntent);

            }
            catch (Exception exception)
            {
                Debug.LogError("GetReceivedNotifications error: " + exception.Message);
            }
            
            return null;
        }
#endif
        
#if UNITY_IOS
        private static List<Notification> GetPendingNotifications()
        {
            var size = GetPendingNotificationsCountInternal();
            
            var pendingNotifications = new List<Notification>();
                
            for (var i = 0; i < size; i++)
            {
                Notification data;
                var ptr = GetPendingNotificationInternal(i);

                if (ptr != IntPtr.Zero)
                {
                    data = (Notification)Marshal.PtrToStructure(ptr, typeof(Notification));
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
                RemoveReceivedNotificationsInternal();
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
            var notificationJavaObject = GetNotificationManager().GetStatic<AndroidJavaObject>("LastReceivedNotification");
            var localNotification = (Notification)ParseNotificationFromAndroidJavaObject(notificationJavaObject);
            
            ForegroundReceivedNotifications.Add(localNotification);

            NotificationReceivedEvent?.Invoke(localNotification);
        }

        private static Notification? ParseNotificationFromAndroidJavaObject(AndroidJavaObject notification)
        {
            if (notification == null)
            {
                return null;
            }
            
            var title = notification.Call<string>("GetTitle");
            var body = notification.Call<string>("GetBody");
            var data = notification.Call<string>("GetData");
            var fireInSeconds = notification.Call<int>("GetFireInSeconds");
            var firedSeconds = notification.Call<long>("GetFiredSeconds");
            
            var localNotification = new Notification();
            localNotification.Title = title;
            localNotification.Body = body;
            localNotification.Data = data;
            localNotification.FireInSeconds = fireInSeconds;
            localNotification.FiredSeconds = firedSeconds;

            return localNotification;
        }

        private static List<Notification> ParseNotificationsFromAndroidJavaObject(AndroidJavaObject notificationsJavaObject)
        {
            var notifications = new List<Notification>();

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
        private static void NotificationReceivedCallback(Notification localNotification)
        {
            ForegroundReceivedNotifications.Add(localNotification);
            
            SavePendingNotifications();
            
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
        
        [MonoPInvokeCallback(typeof(PendingNotificationsUpdatedDelegate))]
        private static void PendingNotificationsUpdatedCallback()
        {
            PendingNotificationUpdated();
        }
#endif
    }
}