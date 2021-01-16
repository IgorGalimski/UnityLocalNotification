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

        public long FiredSeconds;

        public DateTime FiredSecondsDateTime
        {
            get
            {
                var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return dateTime.AddMilliseconds(FireInSeconds);
            }
        }

        public override string ToString()
        {
            return $"Title: {Title} \n " +
                   $"Body: {Body} \n " +
                   $"FiredSeconds: {FiredSecondsDateTime} \n " +
                   $"FireInSeconds: {FireInSeconds} \n " +
                   $"Data: {Data}";
        }

        public override int GetHashCode()
        {
            return ID != null ? ID.GetHashCode() : 0;
        }
    }
}