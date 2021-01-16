//
//  Data.m
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/27/20.
//

#import <Foundation/Foundation.h>

typedef void (*AuthorizationStatusCallback)(struct AuthorizationRequestResult* result);
typedef void (*NotificationReceived)(struct LocalNotification* localNotification);
typedef void (*PendingNotificationsUpdated)(void);
typedef void (*DeviceTokenReceived)(char* deviceToken);

typedef struct AuthorizationRequestResult
{
    bool granted;
    char* error;
} AuthorizationRequestResult;

typedef struct LocalNotification
{
    char* ID;
    char* Title;
    char* Subtitle;
    char* Body;
    char* Data;
    char* CategoryIdentifier;
    char* ThreadIdentifier;
    int FireInSeconds;
    int FiredSeconds;
} LocalNotification;
