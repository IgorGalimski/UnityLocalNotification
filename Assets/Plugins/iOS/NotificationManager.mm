//
//  NotificationManager.m
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/27/20.
//

#import <UserNotifications/UserNotifications.h>
#import "Data.m"
#import "LocalNotificationAppDelegate.h"
#import "NotificationCenterDelegate.h"

extern "C"
{
    typedef void (*AuthorizationStatusCallback)(AuthorizationRequestResult* result);
    typedef void (*ScheduleLocalNotificationSuccess)(LocalNotification* localNotification);
    typedef void (*ScheduleLocalNotificationFail)(LocalNotification* localNotification);

    ScheduleLocalNotificationSuccess _localNotificationSuccessCallback;
    ScheduleLocalNotificationFail _localNotificationFailCallback;

    void SetCallbacksInternal(ScheduleLocalNotificationSuccess localNotificationSuccessCallback, ScheduleLocalNotificationFail localNotificationFailCallback)
                              //DeviceTokenReceived deviceTokenCallback)
    {
        _localNotificationSuccessCallback = localNotificationSuccessCallback;
        _localNotificationFailCallback = localNotificationFailCallback;
        
        //[[NotificationCenterDelegate sharedInstance] setDeviceTokenCallback:deviceTokenCallback];
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
                [[UIApplication sharedApplication] registerForRemoteNotifications];

                struct AuthorizationRequestResult* result = (struct AuthorizationRequestResult*)malloc(sizeof(*result));
                result->granted = granted;
                result->error = [error domain];
            
                callback(result);
            
            }];
    }

    void ScheduleLocalNotificationInternal(LocalNotification* localNotification)
    {
        NSString *title = [NSString localizedUserNotificationStringForKey: [NSString stringWithUTF8String: localNotification->Title] arguments:nil];
        NSString *body = [NSString localizedUserNotificationStringForKey: [NSString stringWithUTF8String: localNotification->Body] arguments:nil];
        NSTimeInterval seconds = localNotification->Seconds;
        
        NSDate *now = [NSDate date];
        now = [now dateByAddingTimeInterval:seconds];

        NSCalendar *calendar = [[NSCalendar alloc] initWithCalendarIdentifier:NSCalendarIdentifierGregorian];
        [calendar setTimeZone:[NSTimeZone localTimeZone]];
        
        NSDateComponents *components = [calendar components:NSCalendarUnitYear|NSCalendarUnitMonth|NSCalendarUnitDay|NSCalendarUnitHour|NSCalendarUnitMinute|NSCalendarUnitSecond|NSCalendarUnitTimeZone fromDate:now];

        UNMutableNotificationContent *objNotificationContent = [[UNMutableNotificationContent alloc] init];
        objNotificationContent.title = title;
        objNotificationContent.body = body;
        objNotificationContent.sound = [UNNotificationSound defaultSound];
        objNotificationContent.badge = @([[UIApplication sharedApplication] applicationIconBadgeNumber] + 1);

        UNCalendarNotificationTrigger *trigger = [UNCalendarNotificationTrigger triggerWithDateMatchingComponents:components repeats:NO];
        
        NSUUID *uuid = [NSUUID UUID];

        UNNotificationRequest *request = [UNNotificationRequest requestWithIdentifier:[uuid UUIDString]
                                                                             content:objNotificationContent trigger:trigger];
        
        UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
        [center addNotificationRequest:request withCompletionHandler:^(NSError * _Nullable error)
        {
           if (!error)
           {
               if(_localNotificationSuccessCallback != nil)
               {
                   _localNotificationSuccessCallback(localNotification);
               }
           }
           else
           {
               if(_localNotificationFailCallback != nil)
               {
                   _localNotificationFailCallback(localNotification);
               }
           }
        }];
    }

    NSString* GetDeviceTokenInternal()
    {
        //[LocalNotificationAppDelegate shar]
        
        return @"123";
        //return [DeviceTokenController sharedInstance].deviceToken;
    }

    LocalNotification* GetLastNotificationInternal()
    {
        UNNotification* notification = [NotificationCenterDelegate sharedInstance].lastReceivedNotification;
        if(notification == nil)
        {
            return nil;
        }
        UNNotificationContent* content = notification.request.content;
        
        struct LocalNotification* localNotification = (struct LocalNotification*)malloc(sizeof(*localNotification));
        
        if (content.title != nil && content.title.length > 0)
        {
            localNotification->Title = (char*) [content.title  UTF8String];
        }
        
        if (content.body != nil && content.body.length > 0)
        {
            localNotification->Body = (char*) [content.body UTF8String];
        }
    
        //TODO parse date
        
        return localNotification;
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
