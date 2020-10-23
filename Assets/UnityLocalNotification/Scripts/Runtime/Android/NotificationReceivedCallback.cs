using System;
using UnityEngine;

namespace UnityLocalNotifications.Android
{
    public class NotificationReceivedCallback : AndroidJavaProxy
    {
        public static event Action NotificationReceived;

        public NotificationReceivedCallback() : base("com.igorgalimski.unitylocalnotification.INotificationReceivedCallback")
        {
        }
        
        public void OnNotificationReceived()
        {
            NotificationReceived?.Invoke();
        }
    }
}