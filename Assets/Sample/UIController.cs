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
        private Button _requestAuthorization;

        [SerializeField] 
        private Button _scheduleNotification;

        public void Start()
        {
            _requestAuthorization.onClick.AddListener(OnAuthorizationRequest);
            _scheduleNotification.onClick.AddListener(ScheduleLocalNotificationHandler);
        }

        public void OnDestroy()
        {
            _requestAuthorization.onClick.RemoveListener(OnAuthorizationRequest);
            _scheduleNotification.onClick.RemoveListener(ScheduleLocalNotificationHandler);
        }

        private void OnAuthorizationRequest()
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

        private void AuthorizationRequestResultHandler(AuthorizationRequestResult authorizationRequestResult)
        {
            LocalNotificationController.AuthorizationRequestResultEvent -= AuthorizationRequestResultHandler;

            _requestStatus.text += authorizationRequestResult.Granted;
        }
    }   
}
