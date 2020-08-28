//
//  UnityAppDelegate.m
//  Unity-iPhone
//
//  Created by Igor Galimski on 8/28/20.
//

#import <Foundation/Foundation.h>
#import "UnityAppController.h"
#import "NotificationCenterDelegate.m"

@interface UnityAppDelegate : UnityAppController
@end

IMPL_APP_CONTROLLER_SUBCLASS(UnityAppDelegate)

@implementation UnityAppDelegate

- (void)application:(UIApplication *)application
didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
    [[NotificationCenterDelegate sharedInstance] parseDeviceToken:deviceToken];
}

@end
