using System;
using UnityEngine;

namespace UnityLocalNotifications.Android
{
    public class AndroidNotification : AndroidJavaProxy
    {
        public String ID;
        
        public String Title;
        
        public bool AutoCancel;

        public String Body;

        public String Data;

        public String SmallIconId;

        public String LargeIconId;

        public int FireInSeconds;

        public long FiredSeconds;
        
        public AndroidNotification() : base("com.igorgalimski.unitylocalnotification.ILocalNotification")
        {
        }

        public String GetID() => ID;

        public void SetID(int id) => ID = id.ToString();

        public Boolean GetAutoCancel() => AutoCancel;

        public String GetTitle() => Title;

        public String GetBody() => Body;

        public String GetData() => Data;

        String GetSmallIconId() => SmallIconId;

        String GetLargeIconId() => LargeIconId;

        public int GetFireInSeconds() => FireInSeconds;

        public long GetFiredSeconds() => FiredSeconds;

        public void SetFiredSeconds(long firedSeconds) => FiredSeconds = firedSeconds;
    }
}