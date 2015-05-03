//
//  LUUnityNetClientPeer.h
//  Lunar
//
//  Created by Alex Lementuev on 1/20/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarNetClientPeer.h"

@interface LunarUnityNetClientPeer : LunarNetClientPeer

@property (nonatomic, readonly) int32_t handle;

- (instancetype)initWithConfiguration:(LunarNetConfiguration *)configuration;

- (LunarNetMessage *)peekMessage;
- (LunarNetMessage *)takeMessage;

@end
