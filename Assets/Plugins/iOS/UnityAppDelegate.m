//
//  UnityAppDelegate.m
//  Unity-iPhone
//
//  Created by Igor Galimski on 8/28/20.
//

#import "UnityAppController.h"
#import "AppDelegateListener.h"
#import <Foundation/Foundation.h>

@interface UnityAppDelegate : UnityAppController// <AppDelegateListener>
{
}
@end


@implementation UnityAppDelegate

- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken {
}

@end
IMPL_APP_CONTROLLER_SUBCLASS(UnityAppDelegate)
