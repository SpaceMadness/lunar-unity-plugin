//
//  NetTupple.h
//  Lunar
//
//  Created by Alex Lementuev on 1/27/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <Foundation/Foundation.h>

@class LunarIPEndPoint;
@class LunarNetMessage;

@interface LunarNetTuple : NSObject

@property (nonatomic, readonly) LunarIPEndPoint *address;
@property (nonatomic, readonly) LunarNetMessage *message;

- (instancetype)initWithEndPoint:(LunarIPEndPoint *)address andMessage:(LunarNetMessage *)message;

@end
