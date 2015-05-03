//
//  LUUnityNetClientPeer.m
//  Lunar
//
//  Created by Alex Lementuev on 1/20/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarUnityNetClientPeer.h"

static int32_t _nextPeerHandle = 0;

@interface LunarUnityNetClientPeer ()
{
    LunarNetMessage * _peekMessage;
}

@end

@implementation LunarUnityNetClientPeer

- (instancetype)initWithConfiguration:(LunarNetConfiguration *)configuration
{
    self = [super initWithConfiguration:configuration];
    if (self)
    {
        _handle = _nextPeerHandle++;
    }
    return self;
}

- (LunarNetMessage *)peekMessage
{
    _peekMessage = [self readMessage];
    return _peekMessage;
}

- (LunarNetMessage *)takeMessage
{
    LunarNetMessage *msg = _peekMessage;
    _peekMessage = nil;
    return msg;
}

@end
