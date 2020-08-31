//
//  NotificationCenterDelegate.h
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/28/20.
//

#import "Data.m"

#ifndef NotificationCenterDelegate_h
#define NotificationCenterDelegate_h

typedef void (^UNNotificationReceived)();

@interface NotificationCenterDelegate : NSObject<UNUserNotificationCenterDelegate>

@property (nonatomic) LocalNotification* lastOpenedNotification;

+ (instancetype)sharedInstance;

-(void) SetNotificationReceivedCallback:(void(^)(LocalNotification*))callback;
-(void) ScheduleLocalNotification:(LocalNotification*) LocalNotification;

@end

#endif /* NotificationCenterDelegate_h */
