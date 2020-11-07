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
        
        public AndroidNotification() : base("com.igorgalimski.unitylocalnotification.ILocalNotification")
        {
        }

        public String GetID() => ID;

        public String GetTitle() => Title;

        public String GetBody() => Body;

        public String GetData() => Data;

        public int GetFireInSeconds() => FireInSeconds;
    }
}