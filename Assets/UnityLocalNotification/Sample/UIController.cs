﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;
#if UNITY_ANDROID
using UnityLocalNotifications.Android;

#endif

#if UNITY_IOS
using UnityLocalNotifications.Authorization;
using UnityLocalNotifications.iOS;
#endif

namespace UnityLocalNotifications.Sample
{
    public class UIController : MonoBehaviour
    {
        private const string NOTIFICATION_ENABLED_TEXT = "Notifications enabled: {0}";

        private const string FOREGROUND_RECEIVED_NOTIFICATIONS_TEXT = "Foreground notifications: {0}";
        private const string BACKGROUND_RECEIVED_NOTIFICATIONS_TEXT = "Background notifications: {0}";

        private const string IS_OPENED_BY_NOTIFICATION_TEXT = "Is opened by notification: {0}";

        [SerializeField] private Text _areNotificatonsEnabled;

        [SerializeField] private Text _foregroundReceivedNotifications;

        [SerializeField] private Text _backgroundReceivedNotifications;

        [SerializeField] private Text _isOpenedByNotification = default;

        [SerializeField] private Text _requestStatus = default;

        [SerializeField] private Text _deviceToken = default;

        [SerializeField] private Text _localNotificationReceived = default;

        [SerializeField] private Button _requestAuthorization = default;

        [SerializeField] private Button _scheduleNotification = default;

        [SerializeField] private Button _removeScheduledNotifications = default;

        [SerializeField] private Button _removeDeliveredNotifications = default;

        public IEnumerator Start()
        {
#if UNITY_IOS
            _areNotificatonsEnabled.text = string.Format(NOTIFICATION_ENABLED_TEXT, LocalNotificationController.AreNotificationsEnabled().ToString());
            
            LocalNotificationController.Initialize(NotificationPresentationOptions.Alert | NotificationPresentationOptions.Badge | NotificationPresentationOptions.Sound);
#endif

#if UNITY_ANDROID
            UpdateNotificationStatus();

            var notificationChannel = new NotificationChannel();
            notificationChannel.Id = "id";
            notificationChannel.Name = "notification_channel";
            notificationChannel.Description = "desc";
            notificationChannel.Importance = Importance.High;
            notificationChannel.ShowBadge = true;
            notificationChannel.Lights = true;
            notificationChannel.Vibration = true;

            LocalNotificationController.CreateNotificationChannel(notificationChannel);
#endif
            UpdateOpenedByNotificationStatus();

            LocalNotificationController.NotificationReceivedEvent += NotificationReceivedHandler;

#if UNITY_IOS
            LocalNotificationController.DeviceTokenReceived += DeviceTokenReceived;
#endif
            _requestAuthorization.onClick.AddListener(OnAuthorizationRequestHandler);
            _scheduleNotification.onClick.AddListener(ScheduleLocalNotificationHandler);
            _removeScheduledNotifications.onClick.AddListener(OnRemoveScheduledNotifications);
            _removeDeliveredNotifications.onClick.AddListener(OnRemoveDeliveredNotifications);

            yield return new WaitForEndOfFrame();

            UpdateBackgroundNotifications(LocalNotificationController.ReceivedNotifications);
        }

        public void OnDestroy()
        {
            LocalNotificationController.NotificationReceivedEvent -= NotificationReceivedHandler;
#if UNITY_IOS
            LocalNotificationController.DeviceTokenReceived -= DeviceTokenReceived;
            
            _requestAuthorization.onClick.RemoveListener(OnAuthorizationRequestHandler);
#endif
            _scheduleNotification.onClick.RemoveListener(ScheduleLocalNotificationHandler);
            _removeScheduledNotifications.onClick.RemoveListener(OnRemoveScheduledNotifications);
            _removeDeliveredNotifications.onClick.RemoveListener(OnRemoveDeliveredNotifications);
        }

        public void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                UpdateOpenedByNotificationStatus();
            }
            else
            {
#if UNITY_IOS
                //LocalNotificationController.SavePendingNotifications();
#endif

                //LocalNotificationController.ClearReceivedNotifications();
            }
        }
        
        private void OnAuthorizationRequestHandler()
        {
            _requestAuthorization.onClick.RemoveListener(OnAuthorizationRequestHandler);
#if UNITY_IOS
            LocalNotificationController.AuthorizationRequestResultEvent += AuthorizationRequestResultHandler;
            
            LocalNotificationController.RequestAuthorization(AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound | AuthorizationOption.Provisional);
#endif
#if UNITY_ANDROID
            LocalNotificationController.RequestPermission();
#endif
}


#if UNITY_ANDROID
        private void UpdateNotificationStatus()
        {
            _areNotificatonsEnabled.text = string.Format(NOTIFICATION_ENABLED_TEXT,
                LocalNotificationController.AreNotificationsEnabled().ToString());
        }
#endif

        private void ScheduleLocalNotificationHandler()
        {
            LocalNotificationController.ScheduleNotification(new Notification
            {
                Title = "Test title",
                Body = "Test body",
                Data = "Test data",
                FireInSeconds = Random.Range(1, 5),
                Icon = "Icon"
            });
        }

        private void OnRemoveScheduledNotifications()
        {
            LocalNotificationController.RemoveScheduledNotifications();
        }

        private void OnRemoveDeliveredNotifications()
        {
            LocalNotificationController.RemoveDeliveredNotifications();
        }

        private void UpdateBackgroundNotifications(List<Notification> receivedNotifications)
        {
            _backgroundReceivedNotifications.text =
                string.Format(BACKGROUND_RECEIVED_NOTIFICATIONS_TEXT, receivedNotifications?.Count);
        }

#if UNITY_IOS
        private void AuthorizationRequestResultHandler(AuthorizationRequestResult authorizationRequestResult)
        { 
            LocalNotificationController.AuthorizationRequestResultEvent -= AuthorizationRequestResultHandler;

            _requestStatus.text += authorizationRequestResult.Granted;
        }
        
        private void DeviceTokenReceived(string deviceToken)
        {
            _deviceToken.text += deviceToken;
        }
#endif
        private void UpdateOpenedByNotificationStatus()
        {
            _isOpenedByNotification.text = string.Format(IS_OPENED_BY_NOTIFICATION_TEXT,
                (LocalNotificationController.GetLastNotification() != null).ToString());
        }

        private void NotificationReceivedHandler(Notification notification)
        {
            _localNotificationReceived.text = notification.ToString();
            _foregroundReceivedNotifications.text = string.Format(FOREGROUND_RECEIVED_NOTIFICATIONS_TEXT,
                LocalNotificationController.ForegroundReceivedNotifications?.Count);
        }
    }
}