//
//  MulticastSocket.h
//  Lunar
//
//  Created by Alex Lementuev on 1/21/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <Foundation/Foundation.h>

@class LunarIPEndPoint;

@class LunarNetBuffer;
@class LunarNetConfiguration;
@class LunarNetMessage;

@class LunarNetMulticastSocket;

@protocol LUNetMulticastSocketDelegate <NSObject>

@optional

- (void)multicastSocket:(LunarNetMulticastSocket *)socket didReceiveMessage:(LunarNetMessage *)message;
- (void)multicastSocket:(LunarNetMulticastSocket *)socket didCloseWithError:(NSError *)error;

@end

@interface LunarNetMulticastSocket : NSObject

@property (nonatomic, assign) id<LUNetMulticastSocketDelegate> delegate;

- (instancetype)initWithConfiguration:(LunarNetConfiguration *)configuration andDelegate:(id<LUNetMulticastSocketDelegate>)delegate;

- (void)start;
- (void)stop;

- (void)sendDiscoveryRequestWithPayload:(LunarNetBuffer *)payload;

- (void)sendDiscoveryResponseToRemote:(LunarIPEndPoint *)host;
- (void)sendDiscoveryResponseToRemote:(LunarIPEndPoint *)host withPayloadMessage:(LunarNetMessage *)payload;

@end
