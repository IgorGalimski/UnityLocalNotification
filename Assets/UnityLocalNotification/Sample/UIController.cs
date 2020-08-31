#if UNITY_IOS || UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;
using UnityLocalNotifications.Authorization;

namespace UnityLocalNotifications.Sample
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] 
        private Text _isOpenedByNotification;
        
        [SerializeField] 
        private Text _requestStatus;

        [SerializeField] 
        private Text _localNotificationReceived;
        
        [SerializeField] 
        private Button _requestAuthorization;

        [SerializeField] 
        private Button _scheduleNotification;

        [SerializeField] 
        private Button _removeScheduledNotifications;

        [SerializeField] 
        private Button _removeDeliveredNotifications;
        
        public void Start()
        {
            _isOpenedByNotification.text += LocalNotificationController.GetLastNotification() != null;
            
            LocalNotificationController.SetCallbacks();

            LocalNotificationController.NotificationReceivedEvent += NotificationReceivedHandler;

            _requestAuthorization.onClick.AddListener(OnAuthorizationRequestHandler);
            _scheduleNotification.onClick.AddListener(ScheduleLocalNotificationHandler);
            _removeScheduledNotifications.onClick.AddListener(OnRemoveScheduledNotifications);
            _removeDeliveredNotifications.onClick.AddListener(OnRemoveDeliveredNotifications);
        }

        public void OnDestroy()
        {
            LocalNotificationController.NotificationReceivedEvent -= NotificationReceivedHandler;
            
            _requestAuthorization.onClick.RemoveListener(OnAuthorizationRequestHandler);
            _scheduleNotification.onClick.RemoveListener(ScheduleLocalNotificationHandler);
            _removeScheduledNotifications.onClick.RemoveListener(OnRemoveScheduledNotifications);
            _removeDeliveredNotifications.onClick.RemoveListener(OnRemoveDeliveredNotifications);
        }

        private void OnAuthorizationRequestHandler()
        {
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
    }   
}

#endif