#if UNITY_IOS

using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityLocalNotifications.Authorization;

namespace UnityLocalNotifications
{
    public static class LocalNotificationController
    {
        [DllImport("__Internal")]
        private static extern void RequestAuthorizationInternal(int options, AuthorizationStatusCallbackDelegate authorizationStatusCallbackDelegate);
        
        private delegate void AuthorizationStatusCallbackDelegate(AuthorizationRequestResult requestResult);
        
        public static event Action<AuthorizationRequestResult>  AuthorizationRequestResultEvent = status => { };
        
        public static void RequestAuthorization(AuthorizationOption authorizationOption)
        {
            try
            {
                RequestAuthorizationInternal((int) authorizationOption, AuthorizationRequestResultCallback);
            }
            catch (Exception exception)
            {
                Debug.LogError("RequestAuthorization error: " + exception.Message);
            }
        }
        
        [MonoPInvokeCallback(typeof(AuthorizationStatusCallbackDelegate))]
        private static void AuthorizationRequestResultCallback(AuthorizationRequestResult authorizationRequestResult)
        {
            AuthorizationRequestResultEvent(authorizationRequestResult);
        }
    }
}

#endif