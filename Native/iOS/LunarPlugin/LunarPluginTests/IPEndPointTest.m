//
//  LUIPEndPointTest.m
//  Lunar
//
//  Created by Alex Lementuev on 2/1/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <XCTest/XCTest.h>
#import <netinet/in.h>

#import "Lunar.h"

#define PACK_ADDR(O1,O2,O3,O4) ((((uint32_t)(O1)) << 24) | (((uint32_t)(O2)) << 16) | (((uint32_t)(O3)) << 8) | (((uint32_t)(O4)) & 0xff))

@interface LUIPEndPointTest : XCTestCase

@end

@implementation LUIPEndPointTest

- (void)testEndPointWithAddressData
{
    uint32_t addr = PACK_ADDR(192, 168, 1, 2);
    uint16_t port = 12345;
    
    struct sockaddr_in sockaddr;
    memset(&sockaddr, 0, sizeof(sockaddr));
    sockaddr.sin_addr.s_addr = htonl(addr);
    sockaddr.sin_port = htons(port);
    
    NSData *addrData = [NSData dataWithBytes:&sockaddr length:sizeof(sockaddr)];
    LunarIPEndPoint *endPoint = [LunarIPEndPoint endPointWithAddressData:addrData];
    
    XCTAssertEqual(endPoint.address, addr);
    XCTAssertEqual(endPoint.port, port);
    XCTAssertEqualObjects(endPoint.host, @"192.168.1.2");
}

- (void)testEndPointWithAddress
{
    uint32_t addr = PACK_ADDR(192, 168, 1, 2);
    uint16_t port = 12345;
    
    LunarIPEndPoint *endPoint = [LunarIPEndPoint endPointWithAddress:addr port:port];
    
    XCTAssertEqual(endPoint.address, addr);
    XCTAssertEqual(endPoint.port, port);
    XCTAssertEqualObjects(endPoint.host, @"192.168.1.2");
}

- (void)testEndPointWithHost
{
    uint32_t addr = PACK_ADDR(192, 168, 1, 2);
    uint16_t port = 12345;
    NSString *host = @"192.168.1.2";
    
    LunarIPEndPoint *endPoint = [LunarIPEndPoint endPointWithHost:host port:port];
    
    XCTAssertEqual(endPoint.address, addr);
    XCTAssertEqual(endPoint.port, port);
    XCTAssertEqualObjects(endPoint.host, @"192.168.1.2");
}

@end
