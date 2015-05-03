//
//  netutils.h
//  Lunar
//
//  Created by Alex Lementuev on 2/1/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <Foundation/Foundation.h>

OBJC_EXTERN NSString *lunarNetAddressToHost(uint32_t address);
OBJC_EXTERN BOOL lunarNetParseAddress(NSString *host, uint32_t *outAddr);