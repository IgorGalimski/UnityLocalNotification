using System;
using UnityEngine;

namespace UnityLocalNotifications.Android
{
    public class NotificationReceivedCallback : AndroidJavaProxy
    {
        public static event Action<LocalNotification> NotificationReceived;

        public NotificationReceivedCallback() : base("com.igorgalimski.unitylocalnotification.INotificationReceivedCallback")
        {
        }
        
        public void OnNotificationReceived(AndroidNotification androidNotification)
        {
            var localNotification = new LocalNotification
            {
                Title = androidNotification.Title, 
                Body = androidNotification.Body
            };
            androidNotification.Data = androidNotification.Data;
            
            NotificationReceived?.Invoke(localNotification);
        }
    }
}