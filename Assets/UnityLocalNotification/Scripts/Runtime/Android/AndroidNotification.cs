using System;
using UnityEngine;

namespace UnityLocalNotifications.Android
{
    public class AndroidNotification : AndroidJavaProxy
    {
        public String ID;
        
        public String Title;

        public String Body;

        public String Data;

        public int FireInSeconds;

        public long FiredSeconds;
        
        public AndroidNotification() : base("com.igorgalimski.unitylocalnotification.ILocalNotification")
        {
        }

        public String GetID() => ID;

        public void SetID(int id) => ID = id.ToString();

        public String GetTitle() => Title;

        public String GetBody() => Body;

        public String GetData() => Data;

        public int GetFireInSeconds() => FireInSeconds;

        public long GetFiredSeconds() => FiredSeconds;

        public void SetFiredSeconds(long firedSeconds) => FiredSeconds = firedSeconds;
    }
}