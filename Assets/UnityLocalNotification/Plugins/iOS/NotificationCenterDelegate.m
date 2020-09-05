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

UNNotificationReceived _callback;

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

- (void) SetNotificationReceivedCallback:(UNNotificationReceived)callback
{
    _callback = callback;
}

-(void)ScheduleLocalNotification:(LocalNotification*)localNotification
{
    NSTimeInterval seconds = localNotification->Seconds;
    
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
        NSDictionary *userInfo = @{
            @"data": @(localNotification->Data),
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
    
    if (content.title != nil && content.title.length > 0)
    {
        localNotification->Title = (char*) [content.title UTF8String];
    }
    else
    {
        localNotification->Title = " ";
    }
    
    if (content.subtitle != nil && content.subtitle.length > 0)
    {
        localNotification->Subtitle = (char*) [content.subtitle UTF8String];
    }
    else
    {
        localNotification->Subtitle = " ";
    }
    
    if (content.body != nil && content.body.length > 0)
    {
        localNotification->Body = (char*) [content.body UTF8String];
    }
    else
    {
        localNotification->Body = " ";
    }
    
    if (content.categoryIdentifier != nil && content.categoryIdentifier.length > 0)
    {
        localNotification->CategoryIdentifier = (char*) [content.categoryIdentifier UTF8String];
    }
    else
    {
        localNotification->CategoryIdentifier = " ";
    }

    if (content.threadIdentifier != nil && content.threadIdentifier.length > 0)
    {
        localNotification->ThreadIdentifier = (char*) [content.threadIdentifier UTF8String];
    }
    else
    {
        localNotification->ThreadIdentifier = " ";
    }
    
    if(content.userInfo != nil && content.userInfo > 0)
    {
        localNotification->Data = (char*)[[[content.userInfo objectForKey: @"data"]description] UTF8String];
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

    if(_callback != nil)
    {
        _callback(ToLocalNotification(notification));
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
