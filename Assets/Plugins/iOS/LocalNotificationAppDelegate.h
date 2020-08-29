//
//  LocalNotificationAppDelegate.h
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/29/20.
//

#import "UnityAppController.h"

#ifndef LocalNotificationAppDelegate_h
#define LocalNotificationAppDelegate_h

typedef void (*DeviceTokenReceived)(NSString* deviceToke);

@interface LocalNotificationAppDelegate : UnityAppController
+ (instancetype)sharedInstance;
-(void) setDeviceTokenCallback:(DeviceTokenReceived)callback;
@end


#endif /* LocalNotificationAppDelegate_h */
