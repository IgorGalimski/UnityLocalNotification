using System;
using System.Runtime.InteropServices;

namespace UnityLocalNotifications
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct LocalNotification
    {
        public string ID;
        
        public string Title;
        
        public string Subtitle;

        public string Body;

        public string Data;

        public string CategoryIdentifier;

        public string ThreadIdentifier;

        public int FireInSeconds;

        public int FiredSeconds;

        public override string ToString()
        {
            var fireIn = TimeSpan.FromSeconds(FireInSeconds);
            
            var firedTimeSpan = TimeSpan.FromSeconds(FiredSeconds);
            var fireDateTime = DateTime.Now.Add(firedTimeSpan);
            
            return $"Title: {Title} \n Body: {Body} \n FireIn: {fireIn} \n FiredSeconds: {fireDateTime.ToLongTimeString()} \n Data: {Data}";
        }

        public override int GetHashCode()
        {
            return ID != null ? ID.GetHashCode() : 0;
        }
    }
}