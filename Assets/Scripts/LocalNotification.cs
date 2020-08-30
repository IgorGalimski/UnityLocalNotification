using System.Runtime.InteropServices;

namespace UnityLocalNotifications
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LocalNotification
    {
        public string Title;

        public string Body;

        public string Data;

        public int Seconds;

        public override string ToString()
        {
            return $"Title: {Title} Body: {Body} Data: {Data}";
        }
    }
}