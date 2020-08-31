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
    char* Subtitle;
    char* Body;
    char* Data;
    char* CategoryIdentifier;
    char* ThreadIdentifier;
    int Seconds;
} LocalNotification;
