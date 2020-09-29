//
//  DeviceTokenHandler.h
//  Unity-iPhone
//
//  Created by Igor Galimski on 9/4/20.
//

#import "Data.m"

#ifndef DeviceTokenHandler_h
#define DeviceTokenHandler_h

typedef void (^DeviceToken)();

@interface DeviceTokenHandler : NSObject

@property (readonly, nullable) char* deviceToken;
@property (nonatomic) DeviceTokenReceived deviceTokenReceived;

+ (instancetype)sharedInstance;

-(void) HandleDeviceToken:(NSData*) deviceToken;

@end

#endif /* DeviceTokenHandler_h */
