//
//  LUNetMessage.h
//  Lunar
//
//  Created by Alex Lementuev on 1/18/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarNetBuffer.h"

typedef NS_ENUM(NSUInteger, LUNetMessageType) {
    LUNetMessageTypeData = 0,
    LUNetMessageTypeError,
    LUNetMessageTypeStatusChanged,
    LUNetMessageTypeConnectionRequest,
    LUNetMessageTypeConnectionResponse,
    LUNetMessageTypeDiscoveryRequest,
    LUNetMessageTypeDiscoveryResponse,
    LUNetMessageTypeLast,
};

#define kNetMessageHeaderSize 5
#define NET_MESSAGE_TYPE_IS_VALID(X) ((X) >= 0 && (X) < LUNetMessageTypeLast)

@class LunarIPEndPoint;
@class LunarNetBuffer;

@interface LunarNetMessageHeader : NSObject

@property (nonatomic, readonly) LUNetMessageType messageType;
@property (nonatomic, readonly) NSUInteger messageLength;

+ (instancetype)readFromBuffer:(LunarNetBuffer *)buffer;

- (instancetype)initWithMessageType:(LUNetMessageType)type andLength:(NSUInteger)length;
- (instancetype)initWithData:(NSData *)data;

- (NSData *)data;

@end

@interface LunarNetMessage : LunarNetBuffer

@property (nonatomic, assign) LUNetMessageType messageType;
@property (nonatomic, retain) LunarIPEndPoint *remoteEndPoint;

+ (instancetype)messageWithType:(LUNetMessageType)type;
+ (instancetype)messageWithType:(LUNetMessageType)type andData:(NSData *)data;

- (instancetype)initWithType:(LUNetMessageType)type;
- (instancetype)initWithType:(LUNetMessageType)type andData:(NSData *)data;

- (NSData *)serializeData;
- (LunarNetMessageHeader *)header;

@end
