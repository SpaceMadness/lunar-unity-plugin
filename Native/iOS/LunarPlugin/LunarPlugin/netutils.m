//
//  netutils.m
//  Lunar
//
//  Created by Alex Lementuev on 2/1/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "netutils.h"

NSString *lunarNetAddressToHost(uint32_t address)
{
    return [NSString stringWithFormat:@"%d.%d.%d.%d",
            (address >> 24) & 0xff,
            (address >> 16) & 0xff,
            (address >> 8) & 0xff,
            address & 0xff];
}

BOOL lunarNetParseAddress(NSString *host, uint32_t *outAddr)
{
    NSArray *components = [host componentsSeparatedByString:@"."];
    if (components.count != 4)
    {
        return NO;
    }
    
    uint32_t address = 0;
    for (NSString *component in components)
    {
        NSScanner *scanner = [[NSScanner alloc] initWithString:component];
        scanner.locale = [[NSLocale alloc] initWithLocaleIdentifier:@"en_US"];
        scanner.charactersToBeSkipped = nil;
        
        int c;
        if (![scanner scanInt:&c])
        {
            return NO;
        }
        
        if (c < 0 || c > 255)
        {
            return NO;
        }
        
        address = (address << 8) | (c & 0xff);
    }
    
    if (outAddr) *outAddr = address;
    return YES;
}