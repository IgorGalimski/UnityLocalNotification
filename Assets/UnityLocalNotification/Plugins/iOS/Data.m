//
//  Data.m
//  UnityLocalNotifications
//
//  Created by Igor Galimski on 8/27/20.
//

#import <Foundation/Foundation.h>

typedef struct AuthorizationRequestResult
{
    bool granted;
    char* error;
} AuthorizationRequestResult;

typedef struct LocalNotification
{
    char* Title;
    char* Body;
    char* Data;
    int Seconds;
} LocalNotification;
