//
//  debug.m
//  Lunar
//
//  Created by Alex Lementuev on 2/15/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <UIKit/UIKit.h>

#import "debug.h"

#define kButtonTitleContinue    @"Continue"
#define kButtonTitleAbort       @"Abort"

@interface LunarAssertNotification : NSObject<UIActionSheetDelegate>

@property (atomic, assign) BOOL blocked;

@end

@implementation LunarAssertNotification

- (void)showMessage:(NSString *)message
{
    UIActionSheet *actionSheet = [[UIActionSheet alloc] initWithTitle:message
                                                             delegate:self
                                                    cancelButtonTitle:nil
                                               destructiveButtonTitle:kButtonTitleAbort
                                                    otherButtonTitles:kButtonTitleContinue,
                                  nil];
    [actionSheet showInView:[UIApplication sharedApplication].keyWindow];
    
    self.blocked = YES;
}

#pragma mark -
#pragma mark UIActionSheetDelegate

// Called when a button is clicked. The view will be automatically dismissed after this call returns
- (void)actionSheet:(UIActionSheet *)actionSheet clickedButtonAtIndex:(NSInteger)buttonIndex
{
    NSString *buttonTitle = [actionSheet buttonTitleAtIndex:buttonIndex];
    if ([buttonTitle isEqualToString:kButtonTitleAbort])
    {
        abort();
    }
    
    self.blocked = NO;
}

// Called when we cancel a view (eg. the user clicks the Home button). This is not called when the user clicks the cancel button.
// If not defined in the delegate, we simulate a click in the cancel button
- (void)actionSheetCancel:(UIActionSheet *)actionSheet
{
    self.blocked = NO;
}

@end

static void display_blocking_alert(NSString *message, NSString *stackTrace)
{
    LunarAssertNotification *notification = [LunarAssertNotification new];
    
    [notification showMessage:[[NSString alloc] initWithFormat:@"%@\n%@", message, stackTrace]];
    
    while (notification.blocked)
    {
        while(CFRunLoopRunInMode(kCFRunLoopDefaultMode, 0, TRUE) == kCFRunLoopRunHandledSource);
    }
}

void __lunar_unity_assertion(const char *messageStr, const char *stackTraceStr)
{
    NSString *message = [NSString stringWithCString:messageStr encoding:NSUTF8StringEncoding];
    NSString *stackTrace = [NSString stringWithCString:stackTraceStr encoding:NSUTF8StringEncoding];
    
    if ([NSThread isMainThread])
    {
        display_blocking_alert(message, stackTrace);
    }
    else
    {
        dispatch_sync(dispatch_get_main_queue(), ^{
            display_blocking_alert(message, stackTrace);
        });
    }
}