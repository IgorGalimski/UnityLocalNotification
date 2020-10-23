using System;
using UnityEngine;

namespace UnityLocalNotifications.Android
{
    public class NotificationChannel : AndroidJavaProxy
    {
        public String Id;
        public String Name;
        public String Description;
        public Boolean ShowBadge;
        public int Importance;
        
        public NotificationChannel() : base("com.igorgalimski.unitylocalnotification.NotificationChannel")
        {
        }

        public String GetId() => Id;
        public String GetName() => Name;
        public String GetDescription() => Description;
        public Boolean GetShowBadge() => ShowBadge;
        public int GetImportance() => Importance;
    }
}