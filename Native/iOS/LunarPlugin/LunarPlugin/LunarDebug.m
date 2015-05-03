//
//  Debug.m
//  Lunar
//
//  Created by Alex Lementuev on 1/18/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarDebug.h"

#define __LUNAR_LOG(TAG,LEVEL) \
    if (LEVEL >= __log_level) \
    { \
        va_list ap; \
        va_start(ap, format); \
        __lunar_log(LEVEL, TAG, format, ap); \
        va_end(ap); \
    }

typedef enum : NSUInteger {
    __lunar_log_level_verbose,
    __lunar_log_level_debug,
    __lunar_log_level_info,
    __lunar_log_level_warn,
    __lunar_log_level_error,
    __lunar_log_level_crit
} __lunar_log_level;

static const char __lunar_log_level_lookup[] =
{
    'V', // __lunar_log_level_verbose
    'D', // __lunar_log_level_debug
    'I', // __lunar_log_level_info
    'W', // __lunar_log_level_warn
    'E', // __lunar_log_level_error
    'C'  // __lunar_log_level_crit
};

static __lunar_log_level __log_level = __lunar_log_level_debug;

static NSString* __lunar_get_thread_name()
{
    if ([NSThread currentThread].isMainThread)
    {
        return nil;
    }
    
    NSString *threadName = [NSThread currentThread].name;
    if (threadName.length > 0)
    {
        return [NSString stringWithFormat:@"%@", threadName];
    }
    
    if (dispatch_get_current_queue)
    {
        dispatch_queue_t currentQueue = dispatch_get_current_queue();
        if (currentQueue != NULL)
        {
            if (currentQueue == dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0))
            {
                return @"<QUEUE_DEFAULT>";
            }
            if (currentQueue == dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_HIGH, 0))
            {
                return @"<QUEUE_HIGH>";
            }
            if (currentQueue == dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_LOW, 0))
            {
                return @"<QUEUE_LOW>";
            }
            if (currentQueue == dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0))
            {
                return @"<QUEUE_BACKGROUND>";
            }
            
            const char *label = dispatch_queue_get_label(currentQueue);
            return label != NULL ? [NSString stringWithFormat:@"%s", label] : @"Serial queue";
        }
    }
    
    return @"<Thread>";
}

static void __lunar_log(__lunar_log_level level, NSString *tag, NSString *format, va_list args)
{
    NSString *threadName = __lunar_get_thread_name();
    NSString *message = [[NSString alloc] initWithFormat:format arguments:args];
    
    NSLog(@"Lunar[%c]%@%@: %@", __lunar_log_level_lookup[level],
          threadName != nil ? [NSString stringWithFormat:@":%@", threadName] : @"",
          tag != nil ? [NSString stringWithFormat:@":%@", tag] : @"",
          message);
}

void __lunar_log_v(NSString *tag, NSString *format, ...) { __LUNAR_LOG(tag, __lunar_log_level_verbose) }

void __lunar_log_d(NSString *tag, NSString *format, ...) { __LUNAR_LOG(tag, __lunar_log_level_debug) }

void __lunar_log_i(NSString *tag, NSString *format, ...) { __LUNAR_LOG(tag, __lunar_log_level_info) }

void __lunar_log_w(NSString *tag, NSString *format, ...) { __LUNAR_LOG(tag, __lunar_log_level_warn) }

void __lunar_log_e(NSString *tag, NSString *format, ...) { __LUNAR_LOG(tag, __lunar_log_level_error) }

void __lunar_log_c(NSString *tag, NSString *format, ...) { __LUNAR_LOG(tag, __lunar_log_level_crit) }