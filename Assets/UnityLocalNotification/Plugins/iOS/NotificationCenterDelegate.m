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

NSArray<UNNotificationRequest*>* pendingRequests;

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
    static dispatch_once_t onceTokenActive;
    static NotificationCenterDelegate *sharedInstance = nil;

    dispatch_once(&onceTokenActive, ^{
        sharedInstance = [[NotificationCenterDelegate alloc] init];

        NSNotificationCenter *nc = [NSNotificationCenter defaultCenter];
        [nc addObserverForName: UIApplicationWillResignActiveNotification
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

    NSMutableDictionary *userInfo = [[NSMutableDictionary alloc] init];
    [userInfo setObject: @(seconds) forKey:FIRE_IN_SECONDS_KEY];
   
    if(localNotification->Data != nil)
    {
        [userInfo setObject:@(localNotification->Data) forKey:@"data"];
    }
    
    objNotificationContent.userInfo = userInfo;
    
    if(localNotification->CategoryIdentifier != nil)
    {
        objNotificationContent.categoryIdentifier = [NSString stringWithUTF8String: localNotification->CategoryIdentifier];
    }
    
    if(localNotification->ThreadIdentifier != nil)
    {
        objNotificationContent.threadIdentifier = [NSString stringWithUTF8String: localNotification->ThreadIdentifier];
    }
    
    objNotificationContent.sound = [UNNotificationSound defaultSound];

    UNCalendarNotificationTrigger *trigger = [UNCalendarNotificationTrigger triggerWithDateMatchingComponents:components repeats:NO];

    NSString* notificationId;

    if(localNotification->ID != nil)
    {
        notificationId = [NSString stringWithUTF8String:localNotification->ID];
    }
    else
    {
        NSUUID *uuid = [NSUUID UUID];
        notificationId = [uuid UUIDString];
    }

    UNNotificationRequest *request = [UNNotificationRequest requestWithIdentifier:notificationId
                                                                         content:objNotificationContent trigger:trigger];
    
    UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
    [center addNotificationRequest:request withCompletionHandler:^(NSError * _Nullable error)
    {
        UpdateBugdeCounter();
        
        [self UpdateScheduledNotificationList];
    }];
}

void UpdateBugdeCounter()
{
    UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
    [center getPendingNotificationRequestsWithCompletionHandler:^(NSArray<UNNotificationRequest *> * _Nonnull requests) {
        dispatch_async(dispatch_get_main_queue(), ^
        {
            [center removeAllPendingNotificationRequests];

            NSArray *sortedArray = [requests sortedArrayUsingComparator:^NSComparisonResult(UNNotificationRequest *obj1, UNNotificationRequest *obj2) {
        
                NSDictionary *userInfo1 = obj1.content.userInfo;
                NSDictionary *userInfo2 = obj2.content.userInfo;
                
                int seconds1 = [userInfo1[FIRE_IN_SECONDS_KEY] intValue];
                int seconds2 = [userInfo2[FIRE_IN_SECONDS_KEY] intValue];
                
                return seconds1 > seconds2;
            }];
            
            long badgeNbr = [[UIApplication sharedApplication] applicationIconBadgeNumber] + 1;
            
            for (UNNotificationRequest *request in sortedArray)
            {
                UNMutableNotificationContent *content = request.content.mutableCopy;
                content.badge = [NSNumber numberWithLong:badgeNbr++];

                UNNotificationRequest *newRequest = [UNNotificationRequest requestWithIdentifier:[request identifier]
                                                                                     content:content
                                                                                         trigger:[request trigger]];
                
                [center addNotificationRequest:newRequest withCompletionHandler:^(NSError * _Nullable error)
                {
                }];
            }
        });
    }];
}

-(void)UpdateScheduledNotificationList
{
    UNUserNotificationCenter* center = [UNUserNotificationCenter currentNotificationCenter];
    [center getPendingNotificationRequestsWithCompletionHandler:^(NSArray<UNNotificationRequest *> * _Nonnull requests) 
    {
        pendingRequests = requests;
        
        if(_pendingNotificationUpdated != nil)
        {
            _pendingNotificationUpdated();
        } 
    }];
}

LocalNotification* ToLocalNotification(UNNotificationRequest* request)
{
    UNNotificationContent* content = request.content;
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
        NSMutableDictionary *extraDictionary = [NSMutableDictionary dictionaryWithDictionary:content.userInfo];
        
        if([[extraDictionary allKeys] containsObject:@"ab"])
        {
            [extraDictionary removeObjectForKey:@"ab"];
        }
        
        if([[extraDictionary allKeys] containsObject:@"aps"])
        {
            NSDictionary *apsDictionary = [extraDictionary valueForKey:@"aps"];
            
            if([[apsDictionary allKeys] containsObject:@"alert"])
            {
                NSDictionary *alertDictionary = [apsDictionary valueForKey:@"alert"];
                
                if([[alertDictionary allKeys] containsObject:@"title"])
                {
                    NSString* title = [alertDictionary valueForKey:@"title"];
                    if(title != nil && title.length > 0)
                    {
                        localNotification->Title = strdup([title UTF8String]);
                    }
                }
                
                if([[alertDictionary allKeys] containsObject:@"body"])
                {
                    NSString* body = [alertDictionary valueForKey:@"body"];
                    if(body != nil && body.length > 0)
                    {
                        localNotification->Body = strdup([body UTF8String]);
                    }
                }
            }
            
            [extraDictionary removeObjectForKey:@"aps"];
        }
        
        NSError* error;
        NSData* data = [NSJSONSerialization dataWithJSONObject: extraDictionary options: NSJSONWritingPrettyPrinted error: &error];
        
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

-(int) GetPendingNotificationsCount
{
    if(pendingRequests == nil)
    {
        return 0;
    }
    
    return pendingRequests.count;
}

-(LocalNotification*) GetPendingNotificationAtIndex:(int) index
{
    if(index >= pendingRequests.count)
    {
        return NULL;
    }
    
    return ToLocalNotification(pendingRequests[index]);
}

- (void)userNotificationCenter:(UNUserNotificationCenter *)center
       willPresentNotification:(UNNotification *)notification
         withCompletionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler
{
    completionHandler((UNNotificationPresentationOptions)_notificationOptions);

    if(_notificationReceived != nil)
    {
        _lastReceivedNotification = ToLocalNotification(notification.request);
        _notificationReceived(_lastReceivedNotification);
    }
}


- (void)userNotificationCenter:(UNUserNotificationCenter *)center
didReceiveNotificationResponse:(UNNotificationResponse *)response
         withCompletionHandler:(void (^)(void))completionHandler;
{
    _lastOpenedNotification = ToLocalNotification(response.notification.request);

    completionHandler();
}

@end
