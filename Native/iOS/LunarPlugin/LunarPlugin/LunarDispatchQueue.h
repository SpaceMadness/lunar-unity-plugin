//
//  DelegateQueue.h
//  Lunar
//
//  Created by Alex Lementuev on 1/27/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface LunarDispatchQueue : NSObject

@property (nonatomic, readonly) dispatch_queue_t queue;
@property (nonatomic, readonly) BOOL isCurrent;

- (instancetype)init;
- (instancetype)initWithName:(NSString *)name;
- (instancetype)initWithQueue:(dispatch_queue_t)queue;

- (void)dispatchBlock:(dispatch_block_t)block sync:(BOOL)flag;
- (void)dispatchSync:(dispatch_block_t)block;
- (void)dispatchAsync:(dispatch_block_t)block;

@end
