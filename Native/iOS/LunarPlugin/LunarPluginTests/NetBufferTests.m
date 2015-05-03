//
//  LUNetBufferTests.m
//  Lunar
//
//  Created by Alex Lementuev on 1/17/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <XCTest/XCTest.h>

#import "LunarNetBuffer.h"

#define Bool BOOL
#define Int8 int8_t
#define Int16 int16_t
#define Int32 int32_t
#define String NSString*

#define BUFFER_WRITE(TYPE, VALUE) \
    [buffer write ## TYPE:VALUE]

#define BUFFER_READ(TYPE, EXPECTED) { \
    TYPE ACTUAL; \
    XCTAssertEqual(YES, [buffer read ## TYPE:&ACTUAL]); \
    XCTAssertEqual(EXPECTED, ACTUAL); \
}

#define BUFFER_READ_OBJ(TYPE, EXPECTED) { \
    TYPE ACTUAL; \
    XCTAssertEqual(YES, [buffer read ## TYPE:&ACTUAL]); \
    XCTAssertEqualObjects(EXPECTED, ACTUAL); \
}

@interface LUNetBufferTests : XCTestCase

@end

@implementation LUNetBufferTests

- (void)testTypes {
    
    Bool expectedBool1 = YES;
    Bool expectedBool2 = NO;
    
    Int8 expectedByte1 = 0;
    Int8 expectedByte2 = INT8_MIN;
    Int8 expectedByte3 = INT8_MAX;
    
    Int16 exectedInt16_1 = 0;
    Int16 exectedInt16_2 = INT16_MIN;
    Int16 exectedInt16_3 = INT16_MAX;
    
    Int32 exectedInt32_1 = 0;
    Int32 exectedInt32_2 = INT32_MIN;
    Int32 exectedInt32_3 = INT32_MAX;
    
    NSString *expectedString1 = nil;
    NSString *expectedString2 = @"";
    NSString *expectedString3 = @"Test";
    NSString *expectedString4 = @"Тест";
    NSString *expectedString5 = [NSString stringWithContentsOfFile:[NSString stringWithUTF8String:__FILE__]
                                                          encoding:NSUTF8StringEncoding
                                                             error:NULL];
    LunarNetBuffer *buffer = [LunarNetBuffer new];
    
    BUFFER_WRITE(Bool, expectedBool1);
    BUFFER_WRITE(Bool, expectedBool2);
    
    BUFFER_WRITE(Int8, expectedByte1);
    BUFFER_WRITE(Int8, expectedByte2);
    BUFFER_WRITE(Int8, expectedByte3);
    
    BUFFER_WRITE(Int16, exectedInt16_1);
    BUFFER_WRITE(Int16, exectedInt16_2);
    BUFFER_WRITE(Int16, exectedInt16_3);
    
    BUFFER_WRITE(Int32, exectedInt32_1);
    BUFFER_WRITE(Int32, exectedInt32_2);
    BUFFER_WRITE(Int32, exectedInt32_3);

    BUFFER_WRITE(String, expectedString1);
    BUFFER_WRITE(String, expectedString2);
    BUFFER_WRITE(String, expectedString3);
    BUFFER_WRITE(String, expectedString4);
    BUFFER_WRITE(String, expectedString5);
    
    BUFFER_READ(Bool, expectedBool1);
    BUFFER_READ(Bool, expectedBool2);
    
    BUFFER_READ(Int8, expectedByte1);
    BUFFER_READ(Int8, expectedByte2);
    BUFFER_READ(Int8, expectedByte3);
    
    BUFFER_READ(Int16, exectedInt16_1);
    BUFFER_READ(Int16, exectedInt16_2);
    BUFFER_READ(Int16, exectedInt16_3);
    
    BUFFER_READ(Int32, exectedInt32_1);
    BUFFER_READ(Int32, exectedInt32_2);
    BUFFER_READ(Int32, exectedInt32_3);
    
    BUFFER_READ_OBJ(String, expectedString1);
    BUFFER_READ_OBJ(String, expectedString2);
    BUFFER_READ_OBJ(String, expectedString3);
    BUFFER_READ_OBJ(String, expectedString4);
    BUFFER_READ_OBJ(String, expectedString5);
}

@end
