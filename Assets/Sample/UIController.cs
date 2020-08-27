using UnityEngine;
using UnityEngine.UI;
using UnityLocalNotifications.Authorization;

namespace UnityLocalNotifications
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] 
        private Button _requestAuthorization;

        public void Start()
        {
            _requestAuthorization.onClick.AddListener(OnAuthorizationRequest);
        }

        public void OnDestroy()
        {
            _requestAuthorization.onClick.RemoveListener(OnAuthorizationRequest);
        }

        private void OnAuthorizationRequest()
        {
            LocalNotificationController.RequestAuthorization(AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound);
        }
    }   
}
