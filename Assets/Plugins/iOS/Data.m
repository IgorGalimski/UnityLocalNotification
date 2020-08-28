//
//  Data.m
//  Unity-iPhone
//
//  Created by Igor Galimski on 8/27/20.
//

#import <Foundation/Foundation.h>

typedef struct AuthorizationRequestResult
{
    bool granted;
    NSString* error;
} AuthorizationRequestResult;

typedef struct LocalNotification
{
    NSString* Title;
    NSString* Body;
    NSInteger* Seconds;
} LocalNotification;
