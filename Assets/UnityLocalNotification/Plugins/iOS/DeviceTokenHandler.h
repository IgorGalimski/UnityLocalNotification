//
//  DeviceTokenHandler.h
//  Unity-iPhone
//
//  Created by Igor Galimski on 9/4/20.
//

#ifndef DeviceTokenHandler_h
#define DeviceTokenHandler_h

typedef void (^DeviceToken)();

@interface DeviceTokenHandler : NSObject

@property (readonly, nullable) char* deviceToken;

+ (instancetype)sharedInstance;

-(void) HandleDeviceToken:(NSData*) deviceToken;
-(void) SetHandleDeviceTokenReceivedCallback:(void(^)())callback;

@end

#endif /* DeviceTokenHandler_h */
