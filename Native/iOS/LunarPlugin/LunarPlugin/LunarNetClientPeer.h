//
//  LUNetClientPeer.h
//  Lunar
//
//  Created by Alex Lementuev on 1/18/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <Foundation/Foundation.h>

@class LunarIPEndPoint;

@class LunarNetBuffer;
@class LunarNetConfiguration;
@class LunarNetMessage;

typedef NS_ENUM(NSUInteger, LUNetConnectionStatus) {
    
    /* No connection, or attempt, in place */
    LUNetConnectionStatusNone,
    
    /* TCP-socket connection established: handshaking */
    LUNetConnectionStatusConnecting,
    
    /* Connected */
    LUNetConnectionStatusConnected,
    
    /* In the process of disconnecting */
    LUNetConnectionStatusDisconnecting,
    
    /* Disconnected */
    LUNetConnectionStatusDisconnected,
};

@interface LunarNetClientPeer : NSObject

- (instancetype)initWithConfiguration:(LunarNetConfiguration *)configuration;

- (void)start;
- (void)stop;

- (void)connectTo:(LunarIPEndPoint *)ep;
- (LunarNetMessage *)readMessage;
- (void)sendMessage:(LunarNetMessage *)message;

- (void)sendDiscoveryRequest;
- (void)sendDiscoveryRequestWithPayload:(LunarNetBuffer *)payload;

@end
