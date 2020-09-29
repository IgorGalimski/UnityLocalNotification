//
//  NotificationCenterDelegate.m
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/28/20.
//

#import <Foundation/Foundation.h>
#import <UserNotifications/UserNotifications.h>
#import "NotificationCenterDelegate.h"

@implementation NotificationCenterDelegate

NSString *const FIRE_IN_SECONDS_KEY = @"fireInSeconds";

+ (void)load
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^
    {
        UNUserNotificationCenter* center = [UNUserNotificationCenter currentNotificationCenter];
        center.delegate = [NotificationCenterDelegate sharedInstance];
    });
}

+ (instancetype)sharedInstance;
{
    static dispatch_once_t onceToken;
    static NotificationCenterDelegate *sharedInstance = nil;

    dispatch_once(&onceToken, ^{
        sharedInstance = [[NotificationCenterDelegate alloc] init];
        
        NSNotificationCenter *nc = [NSNotificationCenter defaultCenter];
        [nc addObserverForName: UIApplicationDidEnterBackgroundNotification
         object: nil
         queue: [NSOperationQueue mainQueue]
         usingBlock:^(NSNotification *notification) {
             [NotificationCenterDelegate sharedInstance].lastOpenedNotification = NULL;
         }];
    });
    return sharedInstance;
}

-(void)ScheduleLocalNotification:(LocalNotification*)localNotification
{
    NSTimeInterval seconds = localNotification->FireInSeconds;
    
    NSDate *now = [NSDate date];
    now = [now dateByAddingTimeInterval:seconds];

    NSCalendar *calendar = [[NSCalendar alloc] initWithCalendarIdentifier:NSCalendarIdentifierGregorian];
    [calendar setTimeZone:[NSTimeZone localTimeZone]];
    
    NSDateComponents *components = [calendar components:NSCalendarUnitYear|NSCalendarUnitMonth|NSCalendarUnitDay|NSCalendarUnitHour|NSCalendarUnitMinute|NSCalendarUnitSecond|NSCalendarUnitTimeZone fromDate:now];

    UNMutableNotificationContent *objNotificationContent = [[UNMutableNotificationContent alloc] init];
    
    if(localNotification->Title != nil)
    {
        objNotificationContent.title =  [NSString localizedUserNotificationStringForKey: [NSString stringWithUTF8String: localNotification->Title] arguments:nil];
    }
    
    if(localNotification->Subtitle != nil)
    {
        objNotificationContent.subtitle = [NSString localizedUserNotificationStringForKey: [NSString stringWithUTF8String: localNotification->Subtitle] arguments:nil];
    }
    
    if(localNotification->Body != nil)
    {
        objNotificationContent.body = [NSString localizedUserNotificationStringForKey: [NSString stringWithUTF8String: localNotification->Body] arguments:nil];
    }
   
    if(localNotification->Data != nil)
    {
        NSDictionary *userInfo =
        @{
            @"data": @(localNotification->Data),
            FIRE_IN_SECONDS_KEY: @(seconds),
        };
        
        objNotificationContent.userInfo = userInfo;
    }
    
    if(localNotification->CategoryIdentifier != nil)
    {
        objNotificationContent.categoryIdentifier = [NSString stringWithUTF8String: localNotification->CategoryIdentifier];
    }
    
    if(localNotification->ThreadIdentifier != nil)
    {
        objNotificationContent.threadIdentifier = [NSString stringWithUTF8String: localNotification->ThreadIdentifier];
    }
    
    objNotificationContent.sound = [UNNotificationSound defaultSound];
    objNotificationContent.badge = @([[UIApplication sharedApplication] applicationIconBadgeNumber] + 1);

    UNCalendarNotificationTrigger *trigger = [UNCalendarNotificationTrigger triggerWithDateMatchingComponents:components repeats:NO];
    
    NSUUID *uuid = [NSUUID UUID];

    UNNotificationRequest *request = [UNNotificationRequest requestWithIdentifier:[uuid UUIDString]
                                                                         content:objNotificationContent trigger:trigger];
    
    UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
    [center addNotificationRequest:request withCompletionHandler:^(NSError * _Nullable error)
    {
    }];
}

LocalNotification* ToLocalNotification(UNNotification* notification)
{
    UNNotificationContent* content = notification.request.content;
    struct LocalNotification* localNotification = (struct LocalNotification*)malloc(sizeof(*localNotification));
    
    NSDate *now = [NSDate date];
    NSTimeInterval seconds = [now timeIntervalSinceNow];
    
    localNotification->FiredSeconds = seconds;
    
    if (content.title != nil && content.title.length > 0)
    {
        localNotification->Title = strdup([content.title UTF8String]);
    }
    else
    {
        localNotification->Title = " ";
    }
    
    if (content.subtitle != nil && content.subtitle.length > 0)
    {
        localNotification->Subtitle = strdup([content.subtitle UTF8String]);
    }
    else
    {
        localNotification->Subtitle = " ";
    }
    
    if (content.body != nil && content.body.length > 0)
    {
        localNotification->Body = strdup([content.body UTF8String]);
    }
    else
    {
        localNotification->Body = " ";
    }
    
    if (content.categoryIdentifier != nil && content.categoryIdentifier.length > 0)
    {
        localNotification->CategoryIdentifier = strdup([content.categoryIdentifier UTF8String]);
    }
    else
    {
        localNotification->CategoryIdentifier = " ";
    }

    if (content.threadIdentifier != nil && content.threadIdentifier.length > 0)
    {
        localNotification->ThreadIdentifier = strdup([content.threadIdentifier UTF8String]);
    }
    else
    {
        localNotification->ThreadIdentifier = " ";
    }
    
    if(content.userInfo != nil && content.userInfo > 0)
    {
        NSError* error;
        NSData* data = [NSJSONSerialization dataWithJSONObject: content.userInfo options: NSJSONWritingPrettyPrinted error: &error];
        
        localNotification->Data = strdup([data bytes]);

        if ([[content.userInfo allKeys] containsObject:FIRE_IN_SECONDS_KEY])
        {
            int fireInSeconds = [content.userInfo[FIRE_IN_SECONDS_KEY] intValue];
            localNotification->FireInSeconds = fireInSeconds;
        }
    }
    else
    {
        localNotification->Data = " ";
    }
    
    return localNotification;
}

- (void)userNotificationCenter:(UNUserNotificationCenter *)center
       willPresentNotification:(UNNotification *)notification
         withCompletionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler
{
    completionHandler((UNNotificationPresentationOptions)_notificationOptions);

    if(_notificationReceived != nil)
    {
        _lastReceivedNotification = ToLocalNotification(notification);
        _notificationReceived(_lastReceivedNotification);
    }
}


- (void)userNotificationCenter:(UNUserNotificationCenter *)center
didReceiveNotificationResponse:(UNNotificationResponse *)response
         withCompletionHandler:(void (^)(void))completionHandler;
{
    _lastOpenedNotification = ToLocalNotification(response.notification);

    completionHandler();
}

@end
