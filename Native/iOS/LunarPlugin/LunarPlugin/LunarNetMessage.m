//
//  LUNetMessage.m
//  Lunar
//
//  Created by Alex Lementuev on 1/18/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarNetMessage.h"
#import "Lunar.h"

#define kNetMessageLengthMax (3 * 1024 * 1024)
#define kLogTag @"LUNetMessage"

@implementation LunarNetMessageHeader

+ (instancetype)readFromBuffer:(LunarNetBuffer *)buffer
{
    int8_t messageTypeByte;
    if (![buffer readInt8:&messageTypeByte])
    {
        logv(kLogTag, @"Can't read message header. Can't read message type");
        return nil;
    }
    
    int32_t payloadLength;
    if (![buffer readInt32:&payloadLength])
    {
        logv(kLogTag, @"Can't read message header. Can't ream message payload length");
        return nil;
    }
    
    LUNetMessageType messageType = messageTypeByte;
    if (!NET_MESSAGE_TYPE_IS_VALID(messageType))
    {
        logv(kLogTag, @"Can't read message header. Invalid message type: %d", messageType);
        return nil;
    }
    
    if (payloadLength < 0 || payloadLength > kNetMessageLengthMax)
    {
        logv(kLogTag, @"Can't read message header. Invalid payload length: %d", payloadLength);
        return nil;
    }
    
    return [[self alloc] initWithMessageType:messageType andLength:payloadLength];
}

- (instancetype)initWithMessageType:(LUNetMessageType)type andLength:(NSUInteger)length
{
    self = [super init];
    if (self)
    {
        _messageType   = type;
        _messageLength = length;
    }
    return self;
}

- (instancetype)initWithData:(NSData *)data
{
    self = [super init];
    if (self)
    {
        if (data.length < kNetMessageHeaderSize)
        {
            logv(kLogTag, @"Can't read message header. Not enough data: %ld", data.length);
            self = nil;
            return self;
        }
        
        const UInt8* bytesPtr = (const UInt8*)(data.bytes);
        LUNetMessageType messageType = bytesPtr[0];
        if (!NET_MESSAGE_TYPE_IS_VALID(messageType))
        {
            logv(kLogTag, @"Can't read message header. Invalid message type: %d", messageType);
            self = nil;
            return self;
        }
        
        NSUInteger messageLength = (((NSUInteger)(bytesPtr[1] & 0xff)) << 24) |
                                   (((NSUInteger)(bytesPtr[2] & 0xff)) << 16) |
                                   (((NSUInteger)(bytesPtr[3] & 0xff)) << 8) |
                                    ((NSUInteger)(bytesPtr[4] & 0xff));
        
        if (messageLength > kNetMessageLengthMax)
        {
            logv(kLogTag, @"Can't read message header. Payload is too big: %ld", messageLength);
            self = nil;
            return self;
        }
        
        _messageType = messageType;
        _messageLength = messageLength;
    }
    return self;
}

- (NSData *)data
{
    static UInt8 header[kNetMessageHeaderSize];
    header[0] = _messageType;
    header[1] = (_messageLength >> 24) & 0xff;
    header[2] = (_messageLength >> 16) & 0xff;
    header[3] = (_messageLength >> 8) & 0xff;
    header[4] = _messageLength & 0xff;
    
    return [NSData dataWithBytes:header length:kNetMessageHeaderSize];
}

@end

@implementation LunarNetMessage

+ (instancetype)messageWithType:(LUNetMessageType)type
{
    return [[self alloc] initWithType:type];
}

+ (instancetype)messageWithType:(LUNetMessageType)type andData:(NSData *)data
{
    return [[self alloc] initWithType:type andData:data];
}

- (instancetype)initWithType:(LUNetMessageType)type
{
    self = [super init];
    if (self)
    {
        _messageType = type;
    }
    return self;
}

- (instancetype)initWithType:(LUNetMessageType)type andData:(NSData *)data
{
    self = [super initWithData:data];
    if (self)
    {
        _messageType = type;
    }
    return self;
}

- (NSData *)serializeData
{
    NSUInteger payloadLength = self.length;
    
    static UInt8 header[kNetMessageHeaderSize];
    header[0] = _messageType;
    header[1] = (payloadLength >> 24) & 0xff;
    header[2] = (payloadLength >> 16) & 0xff;
    header[3] = (payloadLength >> 8) & 0xff;
    header[4] = payloadLength & 0xff;
    
    NSMutableData *data = [NSMutableData dataWithCapacity:kNetMessageHeaderSize + payloadLength];
    [data appendBytes:header length:kNetMessageHeaderSize];
    [data appendData:self.data];
    
    return data;
}

- (LunarNetMessageHeader *)header
{
    return [[LunarNetMessageHeader alloc] initWithMessageType:self.messageType andLength:self.length];
}

@end
