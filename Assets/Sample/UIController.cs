using UnityEngine;
using UnityEngine.UI;
using UnityLocalNotifications.Authorization;

namespace UnityLocalNotifications
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] 
        private Text _requestStatus;

        [SerializeField] 
        private Text _deviceToken;
        
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
            LocalNotificationController.SetCallbacks();

            LocalNotificationController.DeviceTokenReceived += DeviceTokenReceived;
            
            _requestAuthorization.onClick.AddListener(OnAuthorizationRequestHandler);
            _scheduleNotification.onClick.AddListener(ScheduleLocalNotificationHandler);
            _removeScheduledNotifications.onClick.AddListener(OnRemoveScheduledNotifications);
            _removeDeliveredNotifications.onClick.AddListener(OnRemoveDeliveredNotifications);
        }

        public void OnDestroy()
        {
            LocalNotificationController.DeviceTokenReceived -= DeviceTokenReceived;
            
            _requestAuthorization.onClick.RemoveListener(OnAuthorizationRequestHandler);
            _scheduleNotification.onClick.RemoveListener(ScheduleLocalNotificationHandler);
            _removeScheduledNotifications.onClick.RemoveListener(OnRemoveScheduledNotifications);
            _removeDeliveredNotifications.onClick.RemoveListener(OnRemoveDeliveredNotifications);
        }

        private void DeviceTokenReceived(string deviceToken)
        {
            _deviceToken.text = deviceToken;
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
                Body = "Body",
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
    }   
}
