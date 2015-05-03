//
//  LUNetClientPeer.m
//  Lunar
//
//  Created by Alex Lementuev on 1/18/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import <UIKit/UIKit.h>

#import "LunarNetClientPeer.h"

#import "Lunar.h"
#import "LunarNetMulticastSocket.h"

#import "LUGCDAsyncSocket.h"

static const NSUInteger kSocketReadTagHeader   = 1;
static const NSUInteger kSocketReadTagPayload  = 2;
static const NSUInteger kSocketWriteTagMessage = 3;

static const NSTimeInterval kSocketConnectionTimeout = 5.0;
static const NSTimeInterval kSocketReadTimeout       = -1;

#if DEBUG
static const NSTimeInterval kSocketWriteTimeout = -1;
#else
static const NSTimeInterval kSocketWriteTimeout = 5.0;
#endif /* DEBUG */

static NSString * const kLogTag = @"LUNetClientPeer";

#define TAG(TYPE,DATA) ((DATA << 8)|(TYPE & 0xff))
#define TAG_TYPE(X) ((X) & 0xff)
#define TAG_DATA(X) ((X) >> 8)

typedef NS_ENUM(NSUInteger, LUNetClientPeerState) {
    LUNetClientPeerStateDisconnected,
    LUNetClientPeerStateConnecting,
    LUNetClientPeerStateConnected,
    LUNetClientPeerStateDisconnecting
};

@interface LunarNetClientPeer () <LUGCDAsyncSocketDelegate, LUNetMulticastSocketDelegate>
{
    LUNetClientPeerState       _state;
    
    LunarNetConfiguration    * _configuration;
    LunarNetMulticastSocket  * _multicastSocket;
    LUGCDAsyncSocket         * _socket;
    LunarConcurrentQueue     * _incomingMessages;
    LunarConcurrentQueue     * _outgoingMessages;
    LunarIPEndPoint          * _remoteEndPoint;
    
    BOOL                       _messageIsSending; // YES is socket is busy sending a message: all other messages are going into the "outgoing" queue
}

@end

@implementation LunarNetClientPeer

- (instancetype)initWithConfiguration:(LunarNetConfiguration *)configuration
{
    self = [super init];
    if (self)
    {
        [self registerNotifications];
        
        _configuration = configuration;
        
        LunarDispatchQueue *queue = [[LunarDispatchQueue alloc] initWithName:@"Delegate queue"];
        
        _socket = [[LUGCDAsyncSocket alloc] initWithDelegate:self delegateQueue:queue.queue];
        
        _incomingMessages = [LunarConcurrentQueue new];
        _outgoingMessages = [[LunarConcurrentQueue alloc] initWithDispatchQueue:queue];
        
        _state = LUNetClientPeerStateDisconnected;
    }
    return self;
}

- (void)dealloc
{
    [self unregisterNotifications];
    [self stop];
}

- (void)connectTo:(LunarIPEndPoint *)ep
{
    if (_state != LUNetClientPeerStateDisconnected)
    {
        loge(kLogTag, @"Can't connect to host: unexpected state %ld", _state);
        return;
    }
    
    _state = LUNetClientPeerStateDisconnecting;
    
    logd(kLogTag, @"Connecting to %@...", ep);
    
    NSError *error = nil;
    [_socket connectToHost:ep.host onPort:ep.port withTimeout:kSocketConnectionTimeout error:&error];
    if (error != nil)
    {
        loge(kLogTag, @"Can't connect to the server: %@", ep);
        _state = LUNetClientPeerStateDisconnected;
    }
}

- (void)start
{
}

- (void)stop
{
    [self stopMulticastSocket];
    [_socket disconnect];
}

- (LunarNetMessage *)readMessage
{
    return [_incomingMessages dequeueObject];
}

- (void)sendMessage:(LunarNetMessage *)message
{
    __weak id weakSelf = self;
    [_outgoingMessages dispatchAsync:^(LunarConcurrentQueue *queue) {
        __strong id strongSelf = weakSelf;
        if (strongSelf)
        {
            [strongSelf syncSendMessage:message];
        }
    }];
}

- (void)syncSendMessage:(LunarNetMessage *)message
{
    assert(_outgoingMessages.isCurrentQueue);
    
    [_outgoingMessages enqueueObject:message];
    
    if (!_messageIsSending) {
        [self socketSendNextMessage:_socket];
    }
}

- (void)socketSendNextMessage:(LUGCDAsyncSocket *)sock
{
    assert(_outgoingMessages.isCurrentQueue);
    assert(!_messageIsSending);
    
    if (_outgoingMessages.count > 0)
    {
        _messageIsSending = YES;
        [self socket:sock sendMessage:[_outgoingMessages peekObject]]; // don't dequeue message here: only when it's sent
    }
}

#pragma mark -
#pragma mark Discovery

- (void)sendDiscoveryRequest
{
    [self sendDiscoveryRequestWithPayload:nil];
}

- (void)sendDiscoveryRequestWithPayload:(LunarNetBuffer *)payload
{
    if (_multicastSocket == nil)
    {
        _multicastSocket = [[LunarNetMulticastSocket alloc] initWithConfiguration:_configuration andDelegate:self];
        _multicastSocket.delegate = self;
        [_multicastSocket start];
    }
    
    [_multicastSocket sendDiscoveryRequestWithPayload:payload];
}

- (void)stopMulticastSocket
{
    if (_multicastSocket != nil)
    {
        logv(kLogTag, @"Stopping multicast socket");
        _multicastSocket.delegate = nil;
        [_multicastSocket stop];
        _multicastSocket = nil;
    }
}

#pragma mark -
#pragma mark Messages

- (void)beginReadMessagesFromSocket:(LUGCDAsyncSocket *)socket
{
    logv(kLogTag, @"Read message from socket: %ld", kNetMessageHeaderSize);
    [socket readDataToLength:kNetMessageHeaderSize withTimeout:kSocketReadTimeout tag:TAG(kSocketReadTagHeader, 0)];
}

- (void)socket:(LUGCDAsyncSocket *)sock didReadMessageHeaderData:(NSData *)data
{
    logv(kLogTag, @"Receive message header: %ld", data.length);
    
    LunarNetMessageHeader *header = [[LunarNetMessageHeader alloc] initWithData:data];
    if (header == nil)
    {
        [self handleErrorMessage:@"Can't receive message header"];
        return;
    }

    LUNetMessageType type = header.messageType;
    NSUInteger length = header.messageLength;
    
    logv(kLogTag, @"Read message %d length %ld", type, length);
    [sock readDataToLength:length withTimeout:kSocketReadTimeout tag:TAG(kSocketReadTagPayload, type)];
}

- (void)socket:(LUGCDAsyncSocket *)sock didReadMessageType:(LUNetMessageType)type withPayloadData:(NSData *)data
{
    logv(kLogTag, @"Receive message %d length %ld", type, data.length);
    
    LunarNetMessage *message = [LunarNetMessage messageWithType:type andData:data];
    message.remoteEndPoint = _remoteEndPoint;
    
    [self enqueueMessage:message];
    
    [self beginReadMessagesFromSocket:sock];
}

- (void)socket:(LUGCDAsyncSocket *)sock sendMessage:(LunarNetMessage *)message
{
    assert(_outgoingMessages.isCurrentQueue);
    assert(_messageIsSending);
    
    [sock writeData:[message serializeData] withTimeout:kSocketWriteTimeout tag:kSocketWriteTagMessage];
}

- (void)socketDidSendMessage:(LUGCDAsyncSocket *)sock
{
    assert(_outgoingMessages.isCurrentQueue);
    assert(_messageIsSending);
    
    [_outgoingMessages dequeueObject]; // remove successfuly sent message
    _messageIsSending = NO;
    
    [self socketSendNextMessage:sock];
}

- (void)enqueueMessage:(LunarNetMessage *)message
{
    [_incomingMessages enqueueObject:message];
}

- (void)enqueueStatusChangedMessage:(LUNetConnectionStatus)status
{
    LunarNetMessage *msg = [LunarNetMessage messageWithType:LUNetMessageTypeStatusChanged];
    [msg writeInt8:status];
    [self enqueueMessage:msg];
}

#pragma mark -
#pragma mark Errors

- (void)handleErrorMessage:(NSString *)format, ...
{
    
}

#pragma mark -
#pragma mark Notifications

- (void)registerNotifications
{
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(applicationDidEnterBackgroundNotification:)
                                                 name:UIApplicationDidEnterBackgroundNotification
                                               object:nil];
}

- (void)unregisterNotifications
{
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

- (void)applicationDidEnterBackgroundNotification:(NSNotification *)notification
{
    [self stopMulticastSocket];
}

#pragma mark -
#pragma mark LUGCDAsyncSocketDelegate

/**
 * Called when a socket connects and is ready for reading and writing.
 * The host parameter will be an IP address, not a DNS name.
 **/
- (void)socket:(LUGCDAsyncSocket *)sock didConnectToHost:(NSString *)host port:(uint16_t)port
{
    _state = LUNetClientPeerStateConnected;
    _remoteEndPoint = [LunarIPEndPoint endPointWithHost:host port:port];
    
    logd(kLogTag, @"Connected to server %@:%d", host, port);
    [self enqueueStatusChangedMessage:LUNetConnectionStatusConnecting];
    [self beginReadMessagesFromSocket:sock];
}

/**
 * Called when a socket has completed reading the requested data into memory.
 * Not called if there is an error.
 **/
- (void)socket:(LUGCDAsyncSocket *)sock didReadData:(NSData *)data withTag:(long)tag
{
    switch (TAG_TYPE(tag))
    {
        case kSocketReadTagHeader:
        {
            [self socket:sock didReadMessageHeaderData:data];
            break;
        }
            
        case kSocketReadTagPayload:
        {
            LUNetMessageType type = TAG_DATA(tag);
            [self socket:sock didReadMessageType:type withPayloadData:data];
            break;
        }
    }
}

/**
 * Called when a socket has completed writing the requested data. Not called if there is an error.
 **/
- (void)socket:(LUGCDAsyncSocket *)sock didWriteDataWithTag:(long)tag
{
    switch (tag)
    {
        case kSocketWriteTagMessage:
            [self socketDidSendMessage:sock];
            break;
        default:
            loge(kLogTag, @"Unexpected socket write tag: %d", tag);
            break;
    }
}

/**
 * Called when a socket disconnects with or without error.
 *
 * If you call the disconnect method, and the socket wasn't already disconnected,
 * then an invocation of this delegate method will be enqueued on the delegateQueue
 * before the disconnect method returns.
 *
 * Note: If the LUGCDAsyncSocket instance is deallocated while it is still connected,
 * and the delegate is not also deallocated, then this method will be invoked,
 * but the sock parameter will be nil. (It must necessarily be nil since it is no longer available.)
 * This is a generally rare, but is possible if one writes code like this:
 *
 * asyncSocket = nil; // I'm implicitly disconnecting the socket
 *
 * In this case it may preferrable to nil the delegate beforehand, like this:
 *
 * asyncSocket.delegate = nil; // Don't invoke my delegate method
 * asyncSocket = nil; // I'm implicitly disconnecting the socket
 *
 * Of course, this depends on how your state machine is configured.
 **/
- (void)socketDidDisconnect:(LUGCDAsyncSocket *)sock withError:(NSError *)err
{
    if (_state == LUNetClientPeerStateConnected)
    {
        [self enqueueStatusChangedMessage:LUNetConnectionStatusDisconnected];
    }
    _state = LUNetClientPeerStateDisconnected;
    logv(kLogTag, @"Socket disconnected: %@", err);
}

#pragma mark -
#pragma mark LUNetMulticastSocketDelegate

- (void)multicastSocket:(LunarNetMulticastSocket *)socket didReceiveMessage:(LunarNetMessage *)message
{
    [self enqueueMessage:message];
}

- (void)multicastSocket:(LunarNetMulticastSocket *)socket didCloseWithError:(NSError *)error
{
    
}

@end