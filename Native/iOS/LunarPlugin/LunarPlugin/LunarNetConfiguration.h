//
//  LUNetConfiguration.h
//  Lunar
//
//  Created by Alex Lementuev on 1/27/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <Foundation/Foundation.h>

@class LunarIPEndPoint;

@interface LunarNetConfiguration : NSObject

@property (nonatomic, readonly) NSString * appId;
@property (nonatomic, retain) NSString   * multicastHost;
@property (nonatomic, assign) uint16_t     multicastPort;

@property (nonatomic, readonly) LunarIPEndPoint *multicastAddress;

- (instancetype)initWithAppId:(NSString *)appId;

@end
