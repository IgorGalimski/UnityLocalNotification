#if UNITY_IOS || UNITY_EDITOR

using System.Runtime.InteropServices;

namespace UnityLocalNotifications
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LocalNotification
    {
        public string Title;
        
        public string Subtitle;

        public string Body;

        public string Data;

        public string CategoryIdentifier;

        public string ThreadIdentifier;

        public int Seconds;

        public override string ToString()
        {
            return $"Title: {Title} Body: {Body} Data: {Data}";
        }
    }
}

#endif