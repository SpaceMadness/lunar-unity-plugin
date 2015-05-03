//
//  LUNetBuffer.m
//  Lunar
//
//  Created by Alex Lementuev on 1/16/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarNetBuffer.h"
#import "dataio.h"

#define CHECK_READ_AVAILABLE(X) \
if ([self available] < (X)) { \
    _lastError = [NSString stringWithFormat:@"Read overflow: required %ld available %ld", (NSUInteger)(X), [self available]]; \
    return NO; \
}

#define CHECK_RANGE(X, min, max) \
if ((X) < (min) || (X) > (max)) { \
    _lastError = [NSString stringWithFormat:@"'%s' is out of range %d..%d", #X, (min), (max)]; \
    return LUNetBufferResultInvalidArgs; \
}

@interface LunarNetBuffer ()
{
    NSMutableData * _data;
    NSUInteger      _readPosition;
    NSString      * _lastError;
}

@end

@implementation LunarNetBuffer

- (instancetype)init
{
    self = [super init];
    if (self)
    {
        _data = [NSMutableData new];
    }
    return self;
}

- (instancetype)initWithCapacity:(NSUInteger)capacity
{
    self = [super init];
    if (self)
    {
        _data = [NSMutableData dataWithCapacity:capacity];
    }
    return self;
}

- (instancetype)initWithData:(NSData *)data
{
    self = [super init];
    if (self)
    {
        _data = [NSMutableData dataWithData:data];
    }
    return self;
}

#pragma mark -
#pragma mark Read

- (BOOL)readBool:(BOOL *)outValue;
{
    int8_t retval;
    if ([self readInt8:&retval])
    {
        if (outValue) *outValue = retval != 0;
        return YES;
    }
    
    return NO;
}

- (BOOL)readInt8:(int8_t *)outValue
{
    CHECK_READ_AVAILABLE(1);
    
    int8_t retval = lunar_read_int8(_data, _readPosition);
    _readPosition += 1;
    
    if (outValue) *outValue = retval;
    return YES;
}

- (BOOL)readBytes:(NSData **)outValue numberOfBytes:(NSUInteger)numberOfBytes
{
    CHECK_READ_AVAILABLE(numberOfBytes)
    
    const void* dataPtr = (const void*)(_data.bytes + _readPosition);
    NSMutableData *dest = [NSMutableData dataWithCapacity:numberOfBytes];
    [dest appendBytes:dataPtr length:numberOfBytes];
    _readPosition += numberOfBytes;
    
    if (outValue) *outValue = dest;
    return YES;
}

- (BOOL)readBytes:(NSMutableData *)into offset:(NSUInteger)offset numberOfBytes:(NSUInteger)numberOfBytes
{
    CHECK_READ_AVAILABLE(numberOfBytes)
    
    if (offset + numberOfBytes > into.length)
    {
        _lastError = [NSString stringWithFormat:@"offset (%ld) + numberOfBytes (%ld) > destination length (%ld)", offset, numberOfBytes, into.length];
        return NO;
    }
    
    lunar_read_bytes(_data, _readPosition, into, offset, numberOfBytes);
    _readPosition += numberOfBytes;
    
    return YES;
}

- (BOOL)readInt16:(int16_t *)outValue
{
    CHECK_READ_AVAILABLE(2)
    
    int retval = lunar_read_int16(_data, _readPosition);
    _readPosition += 2;
    
    if (outValue) *outValue = retval;
    return YES;
}

- (BOOL)readUInt16:(uint16_t *)outValue
{
    CHECK_READ_AVAILABLE(2)
    
    int retval = lunar_read_int16(_data, _readPosition);
    _readPosition += 2;
    
    if (outValue) *outValue = (uint16_t)retval;
    return YES;
}

- (BOOL)readInt32:(int32_t *)outValue
{
    CHECK_READ_AVAILABLE(4)
    
    int retval = lunar_read_int32(_data, _readPosition);
    _readPosition += 4;
    
    if (outValue) *outValue = retval;
    return YES;
}

- (BOOL)readString:(NSString **)outValue
{
    BOOL notNil;
    if (![self readBool:&notNil])
    {
        return NO;
    }
    
    if (!notNil)
    {
        if (outValue) *outValue = nil;
        return YES;
    }
    
    uint32_t byteLen;
    if (![self readVariableUInt32:&byteLen])
    {
        return NO;
    }
    
    if (byteLen == 0)
    {
        if (outValue) *outValue = @"";
        return YES;
    }
    
    CHECK_READ_AVAILABLE(byteLen)
    
    NSData *stringData;
    if (![self readBytes:&stringData numberOfBytes:byteLen])
    {
        return NO;
    }
    
    NSString *retval = [[NSString alloc] initWithData:stringData encoding:NSUTF8StringEncoding];
    
    if (outValue) *outValue = retval;
    return YES;
}

- (BOOL)readVariableUInt32:(uint32_t *)outValue
{
    uint32_t num1 = 0;
    uint32_t num2 = 0;
    while (self.available >= 1)
    {
        int8_t nextByte;
        if (![self readInt8:&nextByte])
        {
            return NO;
        }
        
        uint32_t num3 = nextByte & 0xff;
        num1 |= (num3 & 0x7f) << num2;
        num2 += 7;
        if ((num3 & 0x80) == 0)
        {
            if (outValue) *outValue = num1;
            return YES;
        }
    }
    
    return YES;
}

#pragma mark -
#pragma mark Write

- (void)writeBool:(BOOL)value
{
    [self writeInt8:(value ? 1 : 0)];
}

- (void)writeInt8:(int8_t)source
{
    [self appendBytes:&source length:1];
}

- (void)writeBytes:(NSData *)source
{
    [self appendData:source];
}

- (BOOL)writeBytes:(NSData *)source offsetInBytes:(NSUInteger)offsetInBytes numberOfBytes:(NSUInteger)numberOfBytes
{
    if (source.length > 0)
    {
        if (offsetInBytes + numberOfBytes > source.length)
        {
            _lastError = [NSString stringWithFormat:@"offset (%ld) + numberOfBytes (%ld) > source length (%ld)",
                          offsetInBytes, numberOfBytes, source.length];
            return NO;
        }
        
        const void* srcPtr = (const void*)(source.bytes + offsetInBytes);
        [self appendBytes:srcPtr length:numberOfBytes];
    }
    
    return YES;
}

- (void)writeInt16:(int16_t)source
{
    static uint8_t temp[2];
    temp[0] = (source >> 8) & 0xff;
    temp[1] = source & 0xff;
    
    [self appendBytes:temp length:2];
}

- (void)writeInt32:(int32_t)source
{
    static uint8_t temp[4];
    temp[0] = (source >> 24) & 0xff;
    temp[1] = (source >> 16) & 0xff;
    temp[2] = (source >> 8) & 0xff;
    temp[3] = source & 0xff;
    
    [self appendBytes:temp length:4];
}

- (void)writeString:(NSString *)source
{
    BOOL notNil = source != nil;
    [self writeBool:notNil];
    
    if (notNil)
    {
        if (source.length == 0)
        {
            [self writeVariableUInt32:0];
        }
        else
        {
            NSData *bytes = [source dataUsingEncoding:NSUTF8StringEncoding];
            [self writeVariableUInt32:(uint32_t)bytes.length];
            [self writeBytes:bytes];
        }
    }
}

- (void)writeBuffer:(LunarNetBuffer *)other
{
    [self writeBytes:other.data offsetInBytes:0 numberOfBytes:other.length];
}

- (uint32_t)writeVariableUInt32:(uint32_t)value
{
    uint32_t retval = 1;
    uint32_t num1 = value;
    while (num1 >= 0x80)
    {
        [self writeInt8:(int8_t)(num1 | 0x80)];
        num1 = num1 >> 7;
        retval++;
    }
    [self writeInt8:(int8_t)num1];
    return retval;
}

#pragma mark -
#pragma mark Helpers

- (void)appendData:(NSData *)data
{
    [_data appendData:data];
}

- (void)appendBytes:(const void *)bytes length:(NSUInteger)length
{
    [_data appendBytes:bytes length:length];
}

- (NSString *)getLastError
{
    NSString *lastError = _lastError;
    _lastError = nil;
    return lastError;
}

#pragma mark -
#pragma mark Properties

- (NSUInteger)length
{
    return _data.length;
}

- (NSUInteger)position
{
    return _readPosition;
}

- (void)setPosition:(NSUInteger)position
{
    _readPosition = position;
}

- (NSUInteger)available
{
    return self.length - self.position;
}

@end
