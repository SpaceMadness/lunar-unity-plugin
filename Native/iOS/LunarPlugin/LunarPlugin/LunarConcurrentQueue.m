//
//  Queue.m
//  Lunar
//
//  Created by Alex Lementuev on 1/18/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarConcurrentQueue.h"
#import "Lunar.h"

static NSString * const kLogTag = @"LunarConcurrentQueue";

@interface LunarConcurrentQueue ()
{
    NSMutableArray * _array;
}

@end

@implementation LunarConcurrentQueue

- (instancetype)init
{
    self = [super init];
    if (self)
    {
        _array = [NSMutableArray new];
        _dispatchQueue = [[LunarDispatchQueue alloc] initWithName:@"LunarConcurrentQueue"];
    }
    return self;
}

- (instancetype)initWithDispatchQueue:(LunarDispatchQueue *)queue
{
    self = [super init];
    if (self)
    {
        _array = [NSMutableArray new];
        _dispatchQueue = queue;
    }
    return self;
}

#pragma mark -
#pragma mark Dispatch queue operations

- (void)dispatchOperation:(void(^)(LunarConcurrentQueue *queue))operation sync:(BOOL)sync
{
    if (!operation)
    {
        loge(kLogTag, @"Unable to dispatch nil operation");
        return;
    }
    
    if (self.isCurrentQueue)
    {
        operation(self);
    }
    else
    {
        __weak id weakSelf = self;
        dispatch_block_t block = ^{
            __strong id strongSelf = weakSelf;
            if (strongSelf)
            {
                operation(strongSelf);
            }
        };
        [_dispatchQueue dispatchBlock:block sync:sync];
    }
}

- (void)dispatchAsync:(void(^)(LunarConcurrentQueue *queue))operation
{
    [self dispatchOperation:operation sync:NO];
}

- (void)dispatchSync:(void(^)(LunarConcurrentQueue *queue))operation
{
    [self dispatchOperation:operation sync:YES];
}

- (void)enqueueObject:(id)object
{
    if (object == nil)
    {
        loge(kLogTag, @"Trying to enqueue nil object");
        return;
    }
    
    if (self.isCurrentQueue)
    {
        [self enqueueObjectSync:object];
    }
    else
    {
        [self dispatchAsync:^(LunarConcurrentQueue *queue)
        {
            [queue enqueueObjectSync:object];
        }];
    }
}

- (id)dequeueObject
{
    if (self.isCurrentQueue)
    {
        return [self dequeueObjectSync];
    }
    
    __block id object = nil;
    [self dispatchSync:^(LunarConcurrentQueue *queue)
    {
        object = [queue dequeueObjectSync];
    }];
    
    return object;
}

- (id)peekObject
{
    if (self.isCurrentQueue)
    {
        return [self peekObjectSync];
    }
    
    __block id object = nil;
    [self dispatchSync:^(LunarConcurrentQueue *queue)
    {
        object = [queue peekObjectSync];
    }];
    
    return object;
}

#pragma mark -
#pragma mark Sync operations

- (void)enqueueObjectSync:(id)object
{
    if (object)
    {
        [_array addObject:object];
    }
    else
    {
        loge(kLogTag, @"Can't enqueue nil message");
    }
}

- (id)dequeueObjectSync
{
    if (_array.count > 0)
    {
        id object = [_array objectAtIndex:0];
        [_array removeObjectAtIndex:0];
        return object;
    }
    
    return nil;
}

- (id)peekObjectSync
{
    if (_array.count > 0)
    {
        return [_array objectAtIndex:0];
    }
    
    return nil;
}

#pragma mark -
#pragma mark Properties

- (NSUInteger)count
{
    if (self.isCurrentQueue)
    {
        return _array.count;
    }
    else
    {
        __block NSUInteger count = 0;
        NSMutableArray *internalArray = _array;
        [_dispatchQueue dispatchSync:^{
            count = internalArray.count;
        }];
        
        return count;
    }
}

- (BOOL)isCurrentQueue
{
    return _dispatchQueue.isCurrent;
}

@end
