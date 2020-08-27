//
//  NotificationManager.m
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/27/20.
//

#import <UserNotifications/UserNotifications.h>

extern "C"
{
void RequestAuthorizationInternal(NSInteger authorizationOptions)
{
UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];

[center requestAuthorizationWithOptions:authorizationOptions completionHandler:^(BOOL granted, NSError * _Nullable error) {
}];
}
}
