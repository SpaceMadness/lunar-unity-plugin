//
//  LUIPEndPoint.m
//  Lunar
//
//  Created by Alex Lementuev on 1/19/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <netinet/in.h>

#import "Lunar.h"

@implementation LunarIPEndPoint

+ (instancetype)endPointWithAddressData:(NSData *)data
{
    if (data.length == sizeof(struct sockaddr_in))
    {
        struct sockaddr_in *sockaddr = (struct sockaddr_in *)data.bytes;
        int16_t port = ntohs(sockaddr->sin_port);
        int32_t addr = ntohl(sockaddr->sin_addr.s_addr);
        
        return [LunarIPEndPoint endPointWithAddress:addr port:port];
    }
    
    return nil;
}

+ (instancetype)endPointWithAddress:(uint32_t)address port:(uint16_t)port
{
    return [[self alloc] initWithAddress:address port:port];
}

+ (instancetype)endPointWithHost:(NSString *)host port:(uint16_t)port
{
    return [[self alloc] initWithHost:host port:port];
}

- (instancetype)initWithAddress:(uint32_t)address port:(uint16_t)port
{
    self = [super init];
    if (self)
    {
        _host = lunarNetAddressToHost(address);
        _address = address;
        _port = port;
    }
    return self;
}

- (instancetype)initWithHost:(NSString *)host port:(uint16_t)port
{
    self = [super init];
    if (self)
    {
        uint32_t address;
        if (!lunarNetParseAddress(host, &address))
        {
            loge(@"LunarIPEndPoint", @"Can't parse host: %@", host);
            self = nil;
            return nil;
        }
        
        _host = host;
        _address = address;
        _port = port;
    }
    return self;
}

- (NSString *)description
{
    return [NSString stringWithFormat:@"%@:%d", _host, _port];
}

@end
