//
//  LocalNotificationAppDelegate.m
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/29/20.
//

#import <Foundation/Foundation.h>
#import "LocalNotificationAppDelegate.h"

@implementation LocalNotificationAppDelegate

- (void)application:(UIApplication *)application
didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
    NSUInteger len = deviceToken.length;
    if (len == 0)
    {
        return;
    }
    const unsigned char *buffer = deviceToken.bytes;
    NSMutableString *str  = [NSMutableString stringWithCapacity:(len * 2)];
    for (int i = 0; i < len; ++i)
    {
        [str appendFormat:@"%02x", buffer[i]];
    }
    
    _deviceToken = [str copy];
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(LocalNotificationAppDelegate)
