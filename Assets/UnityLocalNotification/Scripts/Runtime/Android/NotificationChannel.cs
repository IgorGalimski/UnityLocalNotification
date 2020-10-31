using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityLocalNotifications.Android
{
    public class NotificationChannel : AndroidJavaProxy
    {
        public String Id;
        public String Name;
        public String Description;
        public Boolean ShowBadge;
        public Importance Importance;
        public bool Vibration;
        public bool Lights;
        
        public NotificationChannel() : base("com.igorgalimski.unitylocalnotification.INotificationChannel")
        {
        }

        [Preserve]
        public string GetId() => Id;
        
        [Preserve]
        public string GetName() => Name;
        
        [Preserve]
        public string GetDescription() => Description;
        
        [Preserve]
        public bool GetShowBadge() => ShowBadge;
        
        [Preserve]
        public int GetImportance() => (int)Importance;

        [Preserve]
        public bool GetVibration() => Vibration;

        [Preserve]
        public bool GetLight() => Lights;
    }
}