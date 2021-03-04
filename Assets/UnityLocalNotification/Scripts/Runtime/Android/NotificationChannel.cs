using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityLocalNotifications.Android
{
    public class NotificationChannel : AndroidJavaProxy
    {
        public string Id;
        public string Name;
        public string Description;
        public bool ShowBadge;
        public Importance Importance;
        public bool Vibration;
        public bool Lights;

        public NotificationChannel() : base("com.igorgalimski.unitylocalnotification.INotificationChannel")
        {
        }

        [Preserve]
        public string GetId()
        {
            return Id;
        }

        [Preserve]
        public string GetName()
        {
            return Name;
        }

        [Preserve]
        public string GetDescription()
        {
            return Description;
        }

        [Preserve]
        public bool GetShowBadge()
        {
            return ShowBadge;
        }

        [Preserve]
        public int GetImportance()
        {
            return (int) Importance;
        }

        [Preserve]
        public bool GetVibration()
        {
            return Vibration;
        }

        [Preserve]
        public bool GetLight()
        {
            return Lights;
        }
    }
}