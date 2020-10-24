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
#endif
            
#if UNITY_ANDROID
            LocalNotificationController.Initialize();

            var notificationChannel = new NotificationChannel();
            notificationChannel.Id = "id";
            notificationChannel.Name = "notification_channel";
            notificationChannel.Description = "desc";
            notificationChannel.Importance = 5;
            notificationChannel.ShowBadge = true;
            
            LocalNotificationController.CreateNotificationChannel(notificationChannel);
#endif
            
            _isOpenedByNotification.text += LocalNotificationController.GetLastNotification() != null;
            
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
            LocalNotificationController.DeviceTokenReceived -= DeviceTokenReceived;
            
            _requestAuthorization.onClick.RemoveListener(OnAuthorizationRequestHandler);
#endif
            _scheduleNotification.onClick.RemoveListener(ScheduleLocalNotificationHandler);
            _removeScheduledNotifications.onClick.RemoveListener(OnRemoveScheduledNotifications);
            _removeDeliveredNotifications.onClick.RemoveListener(OnRemoveDeliveredNotifications);
        }

#if UNITY_IOS
        private void OnAuthorizationRequestHandler()
        {
            _requestAuthorization.onClick.RemoveListener(OnAuthorizationRequestHandler);
            
            LocalNotificationController.AuthorizationRequestResultEvent += AuthorizationRequestResultHandler;
            
            LocalNotificationController.RequestAuthorization(AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound);
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
#endif

        private void NotificationReceivedHandler(LocalNotification localNotification)
        {
            _localNotificationReceived.text = localNotification.ToString();
        }
    }   
}