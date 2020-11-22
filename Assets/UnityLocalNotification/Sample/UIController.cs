using UnityEngine;
using UnityEngine.UI;

#if UNITY_ANDROID
using UnityLocalNotifications.Android;
#endif

#if UNITY_IOS
using UnityLocalNotifications.Authorization;
#endif

namespace UnityLocalNotifications.Sample
{
    public class UIController : MonoBehaviour
    {
        private const string NOTIFICATION_ENABLED_TEXT = "Notifications enabled: {0}";

        private const string IS_OPENED_BY_NOTIFICATION_TEXT = "Is opened by notification: {0}";
        
        [SerializeField] 
        private Text _areNotificatonsEnabled;

        [SerializeField] 
        private Text _receivedNotifications;
        
        [SerializeField] 
        private Text _isOpenedByNotification = default;
        
        [SerializeField] 
        private Text _requestStatus = default;

        [SerializeField] 
        private Text _deviceToken = default;

        [SerializeField] 
        private Text _localNotificationReceived = default;
        
        [SerializeField] 
        private Button _requestAuthorization = default;

        [SerializeField] 
        private Button _scheduleNotification = default;

        [SerializeField] 
        private Button _removeScheduledNotifications = default;

        [SerializeField] 
        private Button _removeDeliveredNotifications = default;
        
        public void Start()
        {
#if UNITY_IOS
            LocalNotificationController.Initialize(NotificationPresentationOptions.Alert | NotificationPresentationOptions.Badge | NotificationPresentationOptions.Sound);
            LocalNotificationController.UpdatePreviousPendingNotifications();
            LocalNotificationController.RequestNotificationEnabledStatus();

            LocalNotificationController.NotificationEnabledStatusReceived += OnNotificationStatusEnabledHandler;
#endif
            
#if UNITY_ANDROID
            LocalNotificationController.Initialize();

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
            _receivedNotifications.text += LocalNotificationController.GetReceivedNotifications()?.Count;
            
            UpdateOpenedByNotificationStatus();
            
            LocalNotificationController.NotificationReceivedEvent += NotificationReceivedHandler;
            
 #if UNITY_IOS
            LocalNotificationController.DeviceTokenReceived += DeviceTokenReceived;
            
            _requestAuthorization.onClick.AddListener(OnAuthorizationRequestHandler);
#endif
            _scheduleNotification.onClick.AddListener(ScheduleLocalNotificationHandler);
            _removeScheduledNotifications.onClick.AddListener(OnRemoveScheduledNotifications);
            _removeDeliveredNotifications.onClick.AddListener(OnRemoveDeliveredNotifications);
        }

        public void OnDestroy()
        {
            LocalNotificationController.NotificationReceivedEvent -= NotificationReceivedHandler;
#if UNITY_IOS
            LocalNotificationController.NotificationEnabledStatusReceived -= OnNotificationStatusEnabledHandler;
            
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

                //LocalNotificationController.UpdatePreviousPendingNotifications();
            }
            else
            {
                //LocalNotificationController.SavePendingNotifications();
                
                LocalNotificationController.ClearReceivedNotifications();
            }
        }

#if UNITY_IOS

        private void OnNotificationStatusEnabledHandler(bool notificationEnabled)
        {
            LocalNotificationController.NotificationEnabledStatusReceived -= OnNotificationStatusEnabledHandler;
            
            _areNotificatonsEnabled.text = string.Format(NOTIFICATION_ENABLED_TEXT, notificationEnabled.ToString());
        }
        
        private void OnAuthorizationRequestHandler()
        {
            _requestAuthorization.onClick.RemoveListener(OnAuthorizationRequestHandler);
            
            LocalNotificationController.AuthorizationRequestResultEvent += AuthorizationRequestResultHandler;
            
            LocalNotificationController.RequestAuthorization(AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound);
        }
#endif
        
#if UNITY_ANDROID
        private void UpdateNotificationStatus()
        {
            _areNotificatonsEnabled.text = string.Format(NOTIFICATION_ENABLED_TEXT, LocalNotificationController.AreNotificationsEnabled().ToString());
        }
#endif

        private void ScheduleLocalNotificationHandler()
        {
            LocalNotificationController.ScheduleLocalNotification(new LocalNotification
            {
                Title = "Test title",
                Body = "Test body",
                Data = "Test data",
                FireInSeconds = 3
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

        private void UpdateOpenedByNotificationStatus()
        {
            _isOpenedByNotification.text = string.Format(IS_OPENED_BY_NOTIFICATION_TEXT, (LocalNotificationController.GetLastNotification() != null).ToString());
        }
#endif

        private void NotificationReceivedHandler(LocalNotification localNotification)
        {
            _localNotificationReceived.text = localNotification.ToString();
        }
    }   
}