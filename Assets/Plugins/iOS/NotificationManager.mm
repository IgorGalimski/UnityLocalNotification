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
    typedef void (*ScheduleLocalNotificationSuccess)(LocalNotification* localNotification);
    typedef void (*ScheduleLocalNotificationFail)(LocalNotification* localNotification);

    ScheduleLocalNotificationSuccess _localNotificationSuccessCallback;
    ScheduleLocalNotificationFail _localNotificationFailCallback;

    void SetCallbacksInternal(ScheduleLocalNotificationSuccess localNotificationSuccessCallback, ScheduleLocalNotificationFail localNotificationFailCallback)
    {
        _localNotificationSuccessCallback = localNotificationSuccessCallback;
        _localNotificationFailCallback = localNotificationFailCallback;
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
                struct AuthorizationRequestResult* result = (struct AuthorizationRequestResult*)malloc(sizeof(*result));
                result->granted = granted;
                result->error = [error domain];
            
                callback(result);
            
            }];
    }

    void ScheduleLocalNotificationInternal(LocalNotification* localNotification)
    {
        NSTimeInterval seconds = 5;
        
        NSDate *now = [NSDate date];
        now = [now dateByAddingTimeInterval:seconds];

        NSCalendar *calendar = [[NSCalendar alloc] initWithCalendarIdentifier:NSCalendarIdentifierGregorian];
        [calendar setTimeZone:[NSTimeZone localTimeZone]];
        
        NSDateComponents *components = [calendar components:NSCalendarUnitYear|NSCalendarUnitMonth|NSCalendarUnitDay|NSCalendarUnitHour|NSCalendarUnitMinute|NSCalendarUnitSecond|NSCalendarUnitTimeZone fromDate:now];

        UNMutableNotificationContent *objNotificationContent = [[UNMutableNotificationContent alloc] init];
        objNotificationContent.title = localNotification->Title;
        objNotificationContent.body = localNotification->Body;
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
}
