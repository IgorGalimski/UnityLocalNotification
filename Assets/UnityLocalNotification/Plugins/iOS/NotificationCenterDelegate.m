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
    NSString *title = [NSString localizedUserNotificationStringForKey: [NSString stringWithUTF8String: localNotification->Title] arguments:nil];
    NSString *body = [NSString localizedUserNotificationStringForKey: [NSString stringWithUTF8String: localNotification->Body] arguments:nil];
    NSTimeInterval seconds = localNotification->Seconds;
    
    NSDate *now = [NSDate date];
    now = [now dateByAddingTimeInterval:seconds];

    NSCalendar *calendar = [[NSCalendar alloc] initWithCalendarIdentifier:NSCalendarIdentifierGregorian];
    [calendar setTimeZone:[NSTimeZone localTimeZone]];
    
    NSDateComponents *components = [calendar components:NSCalendarUnitYear|NSCalendarUnitMonth|NSCalendarUnitDay|NSCalendarUnitHour|NSCalendarUnitMinute|NSCalendarUnitSecond|NSCalendarUnitTimeZone fromDate:now];

    NSDictionary *userInfo = @{
        @"data": @(localNotification->Data),
    };

    UNMutableNotificationContent *objNotificationContent = [[UNMutableNotificationContent alloc] init];
    objNotificationContent.title = title;
    objNotificationContent.body = body;
    objNotificationContent.userInfo = userInfo;
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
        localNotification->Title = (char*) [content.title  UTF8String];
    }
    
    if (content.body != nil && content.body.length > 0)
    {
        localNotification->Body = (char*) [content.body UTF8String];
    }
    
    localNotification->Data = (char*)[[[content.userInfo objectForKey: @"data"]description] UTF8String];
    
    return localNotification;
}

- (void)userNotificationCenter:(UNUserNotificationCenter *)center
       willPresentNotification:(UNNotification *)notification
         withCompletionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler
{
    completionHandler(UNNotificationPresentationOptionAlert);

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
