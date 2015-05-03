//
//  Queue.h
//  Lunar
//
//  Created by Alex Lementuev on 1/18/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <Foundation/Foundation.h>

@class LunarDispatchQueue;

@interface LunarConcurrentQueue : NSObject

@property (nonatomic, readonly) NSUInteger count;
@property (nonatomic, readonly) LunarDispatchQueue *dispatchQueue;
@property (nonatomic, readonly) BOOL isCurrentQueue; // YES if code is executed on the assosiated queue

- (instancetype)init;
- (instancetype)initWithDispatchQueue:(LunarDispatchQueue *)queue;

- (void)dispatchAsync:(void(^)(LunarConcurrentQueue *queue))operation;
- (void)dispatchSync:(void(^)(LunarConcurrentQueue *queue))operation;

- (void)enqueueObject:(id)object;
- (id)dequeueObject;
- (id)peekObject;

@end
