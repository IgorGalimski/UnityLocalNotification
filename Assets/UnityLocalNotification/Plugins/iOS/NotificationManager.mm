//
//  NotificationManager.m
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/27/20.
//

#import <UserNotifications/UserNotifications.h>
#import "Data.m"
#import "NotificationCenterDelegate.h"

extern "C"
{
    typedef void (*AuthorizationStatusCallback)(AuthorizationRequestResult* result);
    typedef void (*NotificationReceived)(LocalNotification* localNotification);

    NotificationReceived _notificationReceived;

    void SetCallbacksInternal(NotificationReceived notificationReceived)
    {
        _notificationReceived = notificationReceived;
       
        [[NotificationCenterDelegate sharedInstance] SetNotificationReceivedCallback:^(LocalNotification* localNotication)
        {
            _notificationReceived(localNotication);
        }];
    }

    void ClearBadgeInternal()
    {
        [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
    }

    void RequestAuthorizationInternal(NSInteger authorizationOptions, AuthorizationStatusCallback callback)
    {
        UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
        [center requestAuthorizationWithOptions:authorizationOptions completionHandler:^(BOOL granted, NSError * _Nullable error)
            {
                dispatch_async(dispatch_get_main_queue(), ^{
                    [[UIApplication sharedApplication] registerForRemoteNotifications];
                });

                struct AuthorizationRequestResult* result = (struct AuthorizationRequestResult*)malloc(sizeof(*result));
                result->granted = granted;
                result->error = (char*)[[error localizedDescription]cStringUsingEncoding: NSUTF8StringEncoding];
            
                callback(result);
            }];
    }

    void ScheduleLocalNotificationInternal(LocalNotification* localNotification)
    {
        [[NotificationCenterDelegate sharedInstance] ScheduleLocalNotification:localNotification];
    }

    LocalNotification* GetLastNotificationInternal()
    {
        return [NotificationCenterDelegate sharedInstance].lastOpenedNotification;
    }

    void RemoveScheduledNotificationsInternal()
    {
        UNUserNotificationCenter* center = [UNUserNotificationCenter currentNotificationCenter];
        [center removeAllPendingNotificationRequests];
    }
    
    void RemoveReceivedNotificationsInternal()
    {
        UNUserNotificationCenter* center = [UNUserNotificationCenter currentNotificationCenter];
        [center removeAllDeliveredNotifications];
    }
}
