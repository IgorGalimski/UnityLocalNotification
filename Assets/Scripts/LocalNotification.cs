using System.Runtime.InteropServices;

namespace UnityLocalNotifications
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LocalNotification
    {
        public string Title;

        public string Body;

        public int Seconds;
    }
}