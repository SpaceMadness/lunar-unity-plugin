//
//  Debug.h
//  Lunar
//
//  Created by Alex Lementuev on 1/18/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <Foundation/Foundation.h>

#define logv __lunar_log_v
#define logd __lunar_log_d
#define logi __lunar_log_i
#define logw __lunar_log_w
#define loge __lunar_log_e
#define logc __lunar_log_c

void __lunar_log_v(NSString *tag, NSString *format, ...);
void __lunar_log_d(NSString *tag, NSString *format, ...);
void __lunar_log_i(NSString *tag, NSString *format, ...);
void __lunar_log_w(NSString *tag, NSString *format, ...);
void __lunar_log_e(NSString *tag, NSString *format, ...);
void __lunar_log_c(NSString *tag, NSString *format, ...);