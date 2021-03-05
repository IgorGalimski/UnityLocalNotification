//
//  NotificationCenterDelegate.h
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/28/20.
//

#import "Data.m"

#ifndef NotificationCenterDelegate_h
#define NotificationCenterDelegate_h

@interface NotificationCenterDelegate : NSObject<UNUserNotificationCenterDelegate>

@property (nonatomic) LocalNotification* lastOpenedNotification;
@property (nonatomic) LocalNotification* lastReceivedNotification;
@property (nonatomic) NSInteger notificationOptions;
@property (nonatomic) NotificationReceived notificationReceived;

+ (instancetype)sharedInstance;

-(void) ScheduleLocalNotification:(LocalNotification*) LocalNotification;
-(int) GetPendingNotificationsCount;
-(LocalNotification*) GetPendingNotificationAtIndex:(int) index;
-(void) UpdateScheduledNotificationList;

@end

#endif /* NotificationCenterDelegate_h */
