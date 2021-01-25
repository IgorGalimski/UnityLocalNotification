using System;
using System.Runtime.InteropServices;

namespace UnityLocalNotifications
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Notification
    {
        public bool Local;
        
        public string ID;
        
        public string Title;
        
        public string Subtitle;

        public string Body;

        public string Data;

        public string CategoryIdentifier;

        public string ThreadIdentifier;

        public double FireInSeconds;

        public long FiredSeconds;

        public DateTime FiredSecondsDateTime
        {
            get
            {
                    var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    return dateTime.Add(TimeSpan.FromSeconds(FiredSeconds));
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