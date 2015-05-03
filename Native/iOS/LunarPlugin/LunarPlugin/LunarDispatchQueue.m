//
//  DelegateQueue.m
//  Lunar
//
//  Created by Alex Lementuev on 1/27/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarDispatchQueue.h"
#import "Lunar.h"

static int _nextQueueIndex;

@interface LunarDispatchQueue ()
{
    void * _queueKey;
}
@end

@implementation LunarDispatchQueue

- (instancetype)init
{
    NSString *name = [NSString stringWithFormat:@"LunarDispatchQueue-%d", _nextQueueIndex++];
    return [self initWithName:name];
}

- (instancetype)initWithName:(NSString *)name
{
    self = [super init];
    if (self)
    {
        _queue = dispatch_queue_create(name.UTF8String, DISPATCH_QUEUE_SERIAL);
        
        [self setSpecific];
    }
    return self;
}

- (instancetype)initWithQueue:(dispatch_queue_t)queue
{
    self = [super init];
    if (self)
    {
        DISPATCH_RETAIN_QUEUE(queue);
        _queue = queue;
        
        [self setSpecific];
    }
    return self;
}

- (void)dealloc
{
    DISPATCH_RELEASE_QUEUE(_queue);
}

- (void)setSpecific
{
    _queueKey = &_queueKey;
    dispatch_queue_set_specific(_queue, _queueKey, (__bridge void*)self, NULL);
}

- (void)dispatchBlock:(dispatch_block_t)block sync:(BOOL)flag
{
    if (flag)
    {
        [self dispatchSync:block];
    }
    else
    {
        [self dispatchAsync:block];
    }
}

- (void)dispatchSync:(dispatch_block_t)block
{
    dispatch_sync(_queue, block);
}

- (void)dispatchAsync:(dispatch_block_t)block
{
    dispatch_async(_queue, block);
}

#pragma mark -
#pragma mark Properties

- (BOOL)isCurrent
{
    return dispatch_get_specific(_queueKey) != NULL;
}

@end
