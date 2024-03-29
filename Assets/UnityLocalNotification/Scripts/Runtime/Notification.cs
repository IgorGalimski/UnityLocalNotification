using System;
using System.Runtime.InteropServices;

namespace UnityLocalNotifications
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Notification
    {
        public bool Local;

        public int ID;

        public string Title;

        public string Subtitle;

        public string Body;

        public string Data;

        public string Icon;

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

        public void UpdateID()
        {
            var hash = 0;

            if (!string.IsNullOrEmpty(Title)) hash += Title.GetHashCode();

            if (!string.IsNullOrEmpty(Body)) hash += Body.GetHashCode();

            if (!string.IsNullOrEmpty(Data)) hash += Data.GetHashCode();

            hash += FireInSeconds.GetHashCode();

            ID = hash;
        }
    }
}