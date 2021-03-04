using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityLocalNotifications.Android
{
    public class AndroidNotification : AndroidJavaProxy
    {
        public string Title;

        public bool AutoCancel;

        public string Body;

        public string Data;

        public string SmallIconId;

        public string LargeIconId;

        public double FireInSeconds;

        public long FiredSeconds;

        public AndroidNotification() : base("com.igorgalimski.unitylocalnotification.ILocalNotificationBridge")
        {
        }

        [Preserve]
        public int GetID()
        {
            var hash = 0;

            if (!string.IsNullOrEmpty(Title)) hash += Title.GetHashCode();

            if (!string.IsNullOrEmpty(Body)) hash += Body.GetHashCode();

            if (!string.IsNullOrEmpty(Data)) hash += Data.GetHashCode();

            hash += FireInSeconds.GetHashCode();

            return hash;
        }

        [Preserve]
        public bool GetAutoCancel()
        {
            return AutoCancel;
        }

        [Preserve]
        public string GetTitle()
        {
            return Title;
        }

        [Preserve]
        public string GetBody()
        {
            return Body;
        }

        [Preserve]
        public string GetData()
        {
            return Data;
        }

        [Preserve]
        public string GetSmallIconId()
        {
            return SmallIconId;
        }

        [Preserve]
        public string GetLargeIconId()
        {
            return LargeIconId;
        }

        [Preserve]
        public double GetFireInSeconds()
        {
            return FireInSeconds;
        }

        [Preserve]
        public long GetFiredSeconds()
        {
            return FiredSeconds;
        }

        [Preserve]
        public void SetFiredSeconds(long firedSeconds)
        {
            FiredSeconds = firedSeconds;
        }
    }
}