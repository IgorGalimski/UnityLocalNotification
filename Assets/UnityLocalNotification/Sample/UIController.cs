#if UNITY_IOS || UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;
using UnityLocalNotifications.Authorization;

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
            _isOpenedByNotification.text += LocalNotificationController.GetLastNotification() != null;
            
            LocalNotificationController.Initialize(NotificationPresentationOptions.Alert | NotificationPresentationOptions.Badge | NotificationPresentationOptions.Sound);

            LocalNotificationController.NotificationReceivedEvent += NotificationReceivedHandler;
            LocalNotificationController.DeviceTokenReceived += DeviceTokenReceived;

            _requestAuthorization.onClick.AddListener(OnAuthorizationRequestHandler);
            _scheduleNotification.onClick.AddListener(ScheduleLocalNotificationHandler);
            _removeScheduledNotifications.onClick.AddListener(OnRemoveScheduledNotifications);
            _removeDeliveredNotifications.onClick.AddListener(OnRemoveDeliveredNotifications);
        }

        public void OnDestroy()
        {
            LocalNotificationController.NotificationReceivedEvent -= NotificationReceivedHandler;
            LocalNotificationController.DeviceTokenReceived -= DeviceTokenReceived;
            
            _requestAuthorization.onClick.RemoveListener(OnAuthorizationRequestHandler);
            _scheduleNotification.onClick.RemoveListener(ScheduleLocalNotificationHandler);
            _removeScheduledNotifications.onClick.RemoveListener(OnRemoveScheduledNotifications);
            _removeDeliveredNotifications.onClick.RemoveListener(OnRemoveDeliveredNotifications);
        }

        private void OnAuthorizationRequestHandler()
        {
            _requestAuthorization.onClick.RemoveListener(OnAuthorizationRequestHandler);
            
            LocalNotificationController.AuthorizationRequestResultEvent += AuthorizationRequestResultHandler;
            
            LocalNotificationController.RequestAuthorization(AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound);
        }

        private void ScheduleLocalNotificationHandler()
        {
            LocalNotificationController.ScheduleLocalNotification(new LocalNotification
            {
                Title = "Test title",
                Body = "Test body",
                Data = "Test data",
                Seconds = 3
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

        private void AuthorizationRequestResultHandler(AuthorizationRequestResult authorizationRequestResult)
        {
            LocalNotificationController.AuthorizationRequestResultEvent -= AuthorizationRequestResultHandler;

            _requestStatus.text += authorizationRequestResult.Granted;
        }

        private void NotificationReceivedHandler(LocalNotification localNotification)
        {
            _localNotificationReceived.text = localNotification.ToString();
        }

        private void DeviceTokenReceived(string deviceToken)
        {
            _deviceToken.text += deviceToken;
        }
    }   
}

#endif