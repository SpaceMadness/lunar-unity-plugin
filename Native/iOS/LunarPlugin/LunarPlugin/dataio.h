//
//  netbitwriter.h
//  Lunar
//
//  Created by Alex Lementuev on 1/16/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <Foundation/Foundation.h>

int8_t  lunar_read_int8(NSData *data, size_t offset);
void    lunar_write_int8(int8_t source, NSMutableData *dest, size_t offset);

int16_t lunar_read_int16(NSData *data, size_t offset);
void    lunar_write_int16(int16_t source, NSMutableData *dest, size_t offset);

int32_t lunar_read_int32(NSData *data, size_t offset);
void    lunar_write_int32(int32_t source, NSMutableData *dest, size_t offset);

void    lunar_read_bytes(NSData *src, size_t src_offset, NSMutableData *dst, size_t dst_offset, size_t len);