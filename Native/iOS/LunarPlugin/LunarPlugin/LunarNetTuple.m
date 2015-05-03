//
//  NetTupple.m
//  Lunar
//
//  Created by Alex Lementuev on 1/27/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarNetTuple.h"

#import "Lunar.h"

@implementation LunarNetTuple

- (instancetype)initWithEndPoint:(LunarIPEndPoint *)address andMessage:(LunarNetMessage *)message
{
    self = [super init];
    if (self)
    {
        _address = address;
        _message = message;
    }
    return self;
}

@end
