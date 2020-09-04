//
//  DeviceTokenHandler.m
//  Unity-iPhone
//
//  Created by Igor Galimski on 9/4/20.
//

#import <Foundation/Foundation.h>
#import "DeviceTokenHandler.h"

@implementation DeviceTokenHandler

DeviceToken _callback;

+ (instancetype)sharedInstance;
{
    static dispatch_once_t onceToken;
    static DeviceTokenHandler *sharedInstance = nil;

    dispatch_once(&onceToken, ^{
        sharedInstance = [[DeviceTokenHandler alloc] init];
    });
    return sharedInstance;
}

-(void) HandleDeviceToken:(NSData*) deviceToken
{
    NSUInteger len = deviceToken.length;
    if (len == 0)
    {
        return;
    }
    const unsigned char *buffer = deviceToken.bytes;
    NSMutableString *hexString  = [NSMutableString stringWithCapacity:(len * 2)];
    for (int i = 0; i < len; ++i)
    {
        [hexString appendFormat:@"%02x", buffer[i]];
    }
    
    NSString* deviceTokenString = [hexString copy];
    
    _deviceToken = (char*)[deviceTokenString cStringUsingEncoding:NSUTF8StringEncoding];
    
    if(_callback != nil)
    {
        _callback();
    }
}

-(void) SetHandleDeviceTokenReceivedCallback:(void(^)())callback
{
    _callback = callback;
}

@end
