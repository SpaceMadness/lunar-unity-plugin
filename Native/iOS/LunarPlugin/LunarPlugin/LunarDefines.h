//
//  Defines.h
//  Lunar
//
//  Created by Alex Lementuev on 1/18/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <TargetConditionals.h>

#ifndef Lunar_Defines_h
#define Lunar_Defines_h

    #if !OS_OBJECT_USE_OBJC
        #define DISPATCH_RETAIN_QUEUE(X) dispatch_retain(X)
        #define DISPATCH_RELEASE_QUEUE(X) if (X) dispatch_release(X)
    #else
        #define DISPATCH_RETAIN_QUEUE(X)
        #define DISPATCH_RELEASE_QUEUE(X)
    #endif /* !OS_OBJECT_USE_OBJC */

#endif
