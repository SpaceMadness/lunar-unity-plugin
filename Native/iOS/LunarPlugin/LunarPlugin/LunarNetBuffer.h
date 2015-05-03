//
//  LUNetBuffer.h
//  Lunar
//
//  Created by Alex Lementuev on 1/16/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface LunarNetBuffer : NSObject

@property (nonatomic, readonly, retain) NSData *data;

@property (nonatomic, readonly, assign) NSUInteger length;

@property (nonatomic, readonly, assign) NSUInteger position;

@property (nonatomic, readonly) NSUInteger available;

- (instancetype)init;
- (instancetype)initWithCapacity:(NSUInteger)capacity;
- (instancetype)initWithData:(NSData *)data;

/// <summary>
/// Reads a boolean value written using Write(boolean)
/// </summary>
- (BOOL)readBool:(BOOL *)outValue;

/// <summary>
/// Reads a byte
/// </summary>
- (BOOL)readInt8:(int8_t *)outValue;

/// <summary>
/// Reads the specified number of bytes
/// </summary>
- (BOOL)readBytes:(NSData **)outValue numberOfBytes:(NSUInteger)numberOfBytes;

/// <summary>
/// Reads the specified number of bytes into a preallocated array
/// </summary>
/// <param name="into">The destination array</param>
/// <param name="offset">The offset where to start writing in the destination array</param>
/// <param name="numberOfBytes">The number of bytes to read</param>
- (BOOL)readBytes:(NSMutableData *)into offset:(NSUInteger)offset numberOfBytes:(NSUInteger)numberOfBytes;

/// <summary>
/// Reads a 16 bit signed integer written using Write(short)
/// </summary>
- (BOOL)readInt16:(int16_t *)outValue;

/// <summary>
/// Reads a 16 bit unsigned integer written using Write(short)
/// </summary>
- (BOOL)readUInt16:(uint16_t *)outValue;

/// <summary>
/// Reads a 32 bit signed integer written using Write(int)
/// </summary>
- (BOOL)readInt32:(int32_t *)outValue;

/// <summary>
/// Reads a String written using Write(String)
/// </summary>
- (BOOL)readString:(NSString **)outValue;

/// <summary>
/// Writes a boolean value using 1 bit
/// </summary>
- (void)writeBool:(BOOL)value;

/// <summary>
/// Write a byte
/// </summary>
- (void)writeInt8:(int8_t)source;

/// <summary>
/// Writes all bytes in an array
/// </summary>
- (void)writeBytes:(NSData *)source;

/// <summary>
/// Writes the specified number of bytes from an array
/// </summary>
- (BOOL)writeBytes:(NSData *)source offsetInBytes:(NSUInteger)offsetInBytes numberOfBytes:(NSUInteger)numberOfBytes;

/// <summary>
/// Writes a signed 16 bit integer
/// </summary>
- (void)writeInt16:(int16_t)source;

/// <summary>
/// Writes a 32 bit signed integer
/// </summary>
- (void)writeInt32:(int32_t)source;

/// <summary>
/// Write a String
/// </summary>
- (void)writeString:(NSString *)source;

/// <summary>
/// Write another buffer
/// </summary>
- (void)writeBuffer:(LunarNetBuffer *)other;

- (NSString *)getLastError;

@end
