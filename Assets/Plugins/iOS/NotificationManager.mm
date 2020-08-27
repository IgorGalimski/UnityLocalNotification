//
//  NotificationManager.m
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/27/20.
//

#import <UserNotifications/UserNotifications.h>

#import "Data.m"

extern "C"
{

typedef void (*AuthorizationStatusCallback)(AuthorizationRequestResult* result);

void RequestAuthorizationInternal(NSInteger authorizationOptions, AuthorizationStatusCallback callback)
{
    UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];

    [center requestAuthorizationWithOptions:authorizationOptions completionHandler:^(BOOL granted, NSError * _Nullable error)
        {
            struct AuthorizationRequestResult* result = (struct AuthorizationRequestResult*)malloc(sizeof(*result));
            result->granted = granted;
            result->error = [error domain];
        
            callback(result);
        }];
    }
}
