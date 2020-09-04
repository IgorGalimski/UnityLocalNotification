#import "UnityAppController.h"
#import "AppDelegateListener.h"
#import <Foundation/Foundation.h>

#import "DeviceTokenHandler.h"

@interface UnityAppDelegate : UnityAppController <AppDelegateListener>
{
}
@end



@implementation UnityAppDelegate

- (void) application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken
{
    [[DeviceTokenHandler sharedInstance] HandleDeviceToken:deviceToken];
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(UnityAppDelegate)
