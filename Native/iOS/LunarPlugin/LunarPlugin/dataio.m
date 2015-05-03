//
//  netbitwriter.c
//  Lunar
//
//  Created by Alex Lementuev on 1/16/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "string.h"
#import "dataio.h"

int8_t lunar_read_int8(NSData *data, size_t offset)
{
    const int8_t* data_ptr = (const int8_t*)(data.bytes + offset);
    return *data_ptr;
}

void lunar_write_int8(int8_t source, NSMutableData *dest, size_t offset)
{
    int8_t* dest_ptr = (int8_t*)(dest.mutableBytes + offset);
    *dest_ptr = source;
}

int16_t lunar_read_int16(NSData *data, size_t offset)
{
    const uint8_t* data_ptr = (const uint8_t*)(data.bytes + offset);
    int16_t b1 = data_ptr[0];
    int16_t b2 = data_ptr[1];
    return (b1 << 8) | b2;
}

void lunar_write_int16(int16_t source, NSMutableData *dest, size_t offset)
{
    uint8_t* dest_ptr = (uint8_t*)(dest.mutableBytes + offset);
    dest_ptr[0] = (source >> 8) & 0xff;
    dest_ptr[1] = source & 0xff;
}

int32_t lunar_read_int32(NSData *data, size_t offset)
{
    const uint8_t* data_ptr = (const uint8_t*)(data.bytes + offset);
    int32_t b1 = data_ptr[0];
    int32_t b2 = data_ptr[1];
    int32_t b3 = data_ptr[2];
    int32_t b4 = data_ptr[3];
    return (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;
}

void lunar_write_int32(int32_t source, NSMutableData *dest, size_t offset)
{
    uint8_t* dest_ptr = (uint8_t*)(dest.mutableBytes + offset);
    dest_ptr[0] = (source >> 24) & 0xff;
    dest_ptr[1] = (source >> 16) & 0xff;
    dest_ptr[2] = (source >> 8) & 0xff;
    dest_ptr[3] = source & 0xff;
}

void lunar_read_bytes(NSData *src, size_t src_offset, NSMutableData *dst, size_t dst_offset, size_t len)
{
    const void* src_ptr = (const void *)(src.bytes + src_offset);
    void* dest_ptr = (void *)(dst.mutableBytes + dst_offset);
    memcpy(dest_ptr, src_ptr, len);
}