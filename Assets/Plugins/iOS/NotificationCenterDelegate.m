//
//  NotificationCenterDelegate.m
//  Unity-iPhone
//
//  Created by Igor Galimski on 8/28/20.
//

#import <Foundation/Foundation.h>
#import <UserNotifications/UserNotifications.h>

@interface NotificationCenterDelegate : NSObject<UNUserNotificationCenterDelegate>

+ (instancetype)sharedInstance;
- (void) parseDeviceToken:(NSData*)data;

@end


@implementation NotificationCenterDelegate

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
    });
    return sharedInstance;
}

- (void)parseDeviceToken:(NSData*) data
{
        NSUInteger len = data.length;
        if (len == 0) 
        {
            return;
        }
        const unsigned char *buffer = data.bytes;
        NSMutableString *str  = [NSMutableString stringWithCapacity:(len * 2)];
        for (int i = 0; i < len; ++i) 
        {
            [str appendFormat:@"%02x", buffer[i]];
        }
        //self.deviceToken = [str copy];
}

- (void)userNotificationCenter:(UNUserNotificationCenter *)center
       willPresentNotification:(UNNotification *)notification
         withCompletionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler
{
    completionHandler(UNNotificationPresentationOptionAlert);
}


- (void)userNotificationCenter:(UNUserNotificationCenter *)center
didReceiveNotificationResponse:(UNNotificationResponse *)response
         withCompletionHandler:(void (^)(void))completionHandler;
{
    completionHandler();
}

@end
