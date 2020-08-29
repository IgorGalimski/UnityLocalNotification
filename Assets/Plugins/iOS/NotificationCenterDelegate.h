//
//  NotificationCenterDelegate.h
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/28/20.
//

#ifndef NotificationCenterDelegate_h
#define NotificationCenterDelegate_h

@interface NotificationCenterDelegate : NSObject<UNUserNotificationCenterDelegate>

@property (nonatomic) UNNotification* lastReceivedNotification;

+ (instancetype)sharedInstance;

@end

#endif /* NotificationCenterDelegate_h */
