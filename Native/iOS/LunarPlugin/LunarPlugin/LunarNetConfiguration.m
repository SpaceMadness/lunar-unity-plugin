//
//  LUNetConfiguration.m
//  Lunar
//
//  Created by Alex Lementuev on 1/27/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarNetConfiguration.h"
#import "Lunar.h"

static NSString * const kLogTag = @"LunarNetConfiguration";

@implementation LunarNetConfiguration

- (instancetype)initWithAppId:(NSString *)appId
{
    self = [super init];
    if (self)
    {
        if (appId == nil)
        {
            logd(kLogTag, @"Can't initialize net configutaion: appId is nil");
            
            self = nil;
            return nil;
        }
        
        _appId = appId;
        _multicastHost = @"239.0.0.222";
        _multicastPort = 10600;
    }
    return self;
}

#pragma mark -
#pragma mark Properties

- (LunarIPEndPoint *)multicastAddress
{
    return [[LunarIPEndPoint alloc] initWithHost:_multicastHost port:_multicastPort];
}

@end
