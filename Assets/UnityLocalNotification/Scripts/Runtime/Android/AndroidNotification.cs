using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityLocalNotifications.Android
{
    public class AndroidNotification : AndroidJavaProxy
    {
        public String Title;
        
        public bool AutoCancel;

        public String Body;

        public String Data;

        public String SmallIconId;

        public String LargeIconId;

        public double FireInSeconds;

        public long FiredSeconds;
        
        public AndroidNotification() : base("com.igorgalimski.unitylocalnotification.ILocalNotificationBridge")
        {
        }

        [Preserve]
        public int GetID()
        {
            var hash = 0;

            if (!string.IsNullOrEmpty(Title))
            {
                hash += Title.GetHashCode();
            }
            
            if (!string.IsNullOrEmpty(Body))
            {
                hash += Body.GetHashCode();
            }
            
            if (!string.IsNullOrEmpty(Data))
            {
                hash += Data.GetHashCode();
            }
            
            hash += FireInSeconds.GetHashCode();

            return hash;
        }

        [Preserve]
        public Boolean GetAutoCancel() => AutoCancel;

        [Preserve]
        public String GetTitle() => Title;

        [Preserve]
        public String GetBody() => Body;

        [Preserve]
        public String GetData() => Data;

        [Preserve]
        public String GetSmallIconId() => SmallIconId;

        [Preserve]
        public String GetLargeIconId() => LargeIconId;

        [Preserve]
        public double GetFireInSeconds() => FireInSeconds;

        [Preserve]
        public long GetFiredSeconds() => FiredSeconds;

        [Preserve]
        public void SetFiredSeconds(long firedSeconds) => FiredSeconds = firedSeconds;
    }
}