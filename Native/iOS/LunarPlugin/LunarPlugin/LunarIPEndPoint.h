//
//  LUIPEndPoint.h
//  Lunar
//
//  Created by Alex Lementuev on 1/19/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface LunarIPEndPoint : NSObject

@property (nonatomic, readonly) NSString * host;
@property (nonatomic, readonly) uint32_t   address;
@property (nonatomic, readonly) uint16_t   port;

+ (instancetype)endPointWithAddressData:(NSData *)data;
+ (instancetype)endPointWithAddress:(uint32_t)address port:(uint16_t)port;
+ (instancetype)endPointWithHost:(NSString *)host port:(uint16_t)port;

- (instancetype)initWithAddress:(uint32_t)address port:(uint16_t)port;
- (instancetype)initWithHost:(NSString *)host port:(uint16_t)port;

@end
