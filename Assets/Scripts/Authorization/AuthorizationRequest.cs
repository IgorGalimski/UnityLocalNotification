using System.Runtime.InteropServices;

namespace UnityLocalNotifications.Authorization
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AuthorizationRequestResult
    {
        public bool Granted;

        public string Error;
    }
}