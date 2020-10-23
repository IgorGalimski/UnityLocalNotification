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
        
        public void OnNotificationReceived()
        {
            var notification = new LocalNotification
            {/*
                 Title = localNotification.Call<string>("GetTitle"),
                 Body = localNotification.Call<string>("GetBody"),
                 Data = localNotification.CallStatic<string>("GetData")*/
            };

            NotificationReceived?.Invoke(notification);
        }
    }
}