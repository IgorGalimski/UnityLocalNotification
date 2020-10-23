using System;
using UnityEngine;

namespace UnityLocalNotifications.Android
{
    public class AndroidNotification : AndroidJavaProxy
    {
        public String Title;

        public String Body;

        public String Data;

        public int FireInSeconds;
        
        public AndroidNotification() : base("com.igorgalimski.unitylocalnotification.LocalNotification")
        {
        }

        public String GetTitle() => Title;

        public String GetBody() => Body;

        public String GetData() => Data;

        public int GetFireInSeconds() => FireInSeconds;
    }
}