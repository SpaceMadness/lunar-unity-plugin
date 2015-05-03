//
//  MulticastSocket.m
//  Lunar
//
//  Created by Alex Lementuev on 1/21/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#import "LunarNetMulticastSocket.h"

#import "Lunar.h"
#import "LUGCDAsyncUdpSocket.h"

static NSString * const kLogTag = @"LUNetMulticastSocket";
static const int kProtocolVersion = 1;

@interface LunarNetMulticastSocket () <LUGCDAsyncUdpSocketDelegate>
{
    LunarNetConfiguration  * _configuration;
    LUGCDAsyncUdpSocket    * _socket;
    LunarConcurrentQueue   * _outgoingMessages;
    BOOL _sendBusy;
}

@end

@implementation LunarNetMulticastSocket

- (instancetype)initWithConfiguration:(LunarNetConfiguration *)configuration andDelegate:(id<LUNetMulticastSocketDelegate>)delegate
{
    self = [super init];
    if (self)
    {
        _configuration = configuration;
        _outgoingMessages = [LunarConcurrentQueue new];
    }
    
    return self;
}

- (void)start
{
    _socket = [[LUGCDAsyncUdpSocket alloc] initWithDelegate:self delegateQueue:_outgoingMessages.dispatchQueue.queue];
    
    NSError *error = nil;
    if (![_socket bindToPort:0 error:&error])
    {
        NSLog(@"Can't bind socket: %@", error);
        return;
    }
    
    if (![_socket beginReceiving:&error])
    {
        NSLog(@"Can't begin receive: %@", error);
        return;
    }
}

- (void)stop
{
    [_socket close];
}

#pragma mark -
#pragma mark Messages

- (void)sendDiscoveryRequestWithPayload:(LunarNetBuffer *)payload
{
    LunarIPEndPoint *multicastEp = _configuration.multicastAddress;

    __weak id weakSelf = self;
    [_outgoingMessages dispatchAsync:^(LunarConcurrentQueue *queue)
    {
        LunarNetMulticastSocket *strongSelf = weakSelf;
        if (strongSelf)
        {
            LunarNetMessage *msg = [[LunarNetMessage alloc] initWithType:LUNetMessageTypeDiscoveryRequest];
            if (payload)
            {
                [msg writeBuffer:payload];
            }
            
            LunarNetTuple *tuple = [[LunarNetTuple alloc] initWithEndPoint:multicastEp andMessage:msg];
            [queue enqueueObject:tuple];
            
            [strongSelf dequeueAndSendMessage];
        }
    }];
}

- (void)sendDiscoveryResponseToRemote:(LunarIPEndPoint *)remoteEp
{
    [self sendDiscoveryResponseToRemote:remoteEp withPayloadMessage:nil];
}

- (void)sendDiscoveryResponseToRemote:(LunarIPEndPoint *)remoteEp withPayloadMessage:(LunarNetMessage *)payload
{
    __weak id weakSelf = self;
    [_outgoingMessages dispatchAsync:^(LunarConcurrentQueue *queue) {
        LunarNetMulticastSocket *strongSelf = weakSelf;
        if (strongSelf)
        {
            LunarNetMessage *msg = [[LunarNetMessage alloc] initWithType:LUNetMessageTypeDiscoveryResponse];
            if (payload)
            {
                [msg writeBuffer:payload];
            }
            
            LunarNetTuple *tuple = [[LunarNetTuple alloc] initWithEndPoint:remoteEp andMessage:msg];
            [queue enqueueObject:tuple];
            
            [strongSelf dequeueAndSendMessage];
        }
    }];
}

- (void)dequeueAndSendMessage
{
    assert(_outgoingMessages.isCurrentQueue);
    
    if (!_sendBusy)
    {
        LunarNetTuple *tuple = [_outgoingMessages dequeueObject];
        if (tuple)
        {
            _sendBusy = YES;
            [self sendMulticastMessage:tuple.message toHost:tuple.address];
        }
    }
}

- (void)sendMulticastMessage:(LunarNetMessage *)msg toHost:(LunarIPEndPoint *)address
{
    LunarNetBuffer *buffer = [LunarNetBuffer new];
    
    [self writeMulticastHeaderToBuffer:buffer];
    [buffer writeBytes:[msg serializeData]];
    
    [_socket sendData:buffer.data toHost:address.host port:address.port withTimeout:-1 tag:0];
}

#pragma mark -
#pragma mark LUGCDAsyncUdpSocketDelegate

/**
 * Called when the datagram with the given tag has been sent.
 **/
- (void)udpSocket:(LUGCDAsyncUdpSocket *)sock didSendDataWithTag:(long)tag
{
    assert(_outgoingMessages.isCurrentQueue);
    assert(_sendBusy);
    
    _sendBusy = NO;
    [self dequeueAndSendMessage];
}

/**
 * Called if an error occurs while trying to send a datagram.
 * This could be due to a timeout, or something more serious such as the data being too large to fit in a sigle packet.
 **/
- (void)udpSocket:(LUGCDAsyncUdpSocket *)sock didNotSendDataWithTag:(long)tag dueToError:(NSError *)error
{
    assert(_outgoingMessages.isCurrentQueue);
    assert(_sendBusy);
    
    _sendBusy = NO;
    [self dequeueAndSendMessage];
}

/**
 * Called when the socket has received the requested datagram.
 **/
- (void)udpSocket:(LUGCDAsyncUdpSocket *)sock didReceiveData:(NSData *)data
      fromAddress:(NSData *)address
withFilterContext:(id)filterContext
{
    LunarIPEndPoint *remoteEndPoint = [LunarIPEndPoint endPointWithAddressData:address];
    if (remoteEndPoint == nil)
    {
        logd(kLogTag, @"Can't parse remote end point from data");
        return;
    }
    
    LunarNetMessage *msg = [self readMessageFromData:data endPoint:remoteEndPoint];
    if (msg != nil)
    {
        [self notifySocketDidReceiveMessage:msg];
    }
}

- (LunarNetMessage *)readMessageFromData:(NSData *)data endPoint:(LunarIPEndPoint *)remoteEndPoint
{
    LunarNetBuffer *buffer = [[LunarNetBuffer alloc] initWithData:data];

    if (![self readMulticastHeaderFromBuffer:buffer])
    {
        return nil;
    }
    
    LunarNetMessageHeader *header = [LunarNetMessageHeader readFromBuffer:buffer];
    if (header == nil)
    {
        logv(kLogTag, @"Can't receive multicast message");
        return nil;
    }
    
    LUNetMessageType messageType = header.messageType;
    if (messageType != LUNetMessageTypeDiscoveryResponse &&
        messageType != LUNetMessageTypeDiscoveryRequest)
    {
        logv(kLogTag, @"Can't receive multicast message: unexpected message type: %d", messageType);
        return nil;
    }
    
    if (buffer.available != header.messageLength)
    {
        logv(kLogTag, @"Can't receive multicast message: not enough data %ld expected %ld", buffer.available, header.messageLength);
        return nil;
    }
    
    
    if (messageType == LUNetMessageTypeDiscoveryResponse)
    {
        uint16_t listeningPort;
        if (![buffer readUInt16:&listeningPort])
        {
            logv(kLogTag, @"Can't receive multicast message: can't read listening port");
            return nil;
        }
        
        remoteEndPoint = [[LunarIPEndPoint alloc] initWithHost:remoteEndPoint.host port:listeningPort];
    }

    NSData *messageData;
    if (![buffer readBytes:&messageData numberOfBytes:buffer.available])
    {
        logv(kLogTag, @"Can't receive multicast message: can't read payload bytes %ld", buffer.available);
        return nil;
    }
    
    LunarNetMessage *msg = [[LunarNetMessage alloc] initWithType:messageType andData:messageData];
    msg.remoteEndPoint = remoteEndPoint;
    return msg;
}

/**
 * Called when the socket is closed.
 **/
- (void)udpSocketDidClose:(LUGCDAsyncUdpSocket *)sock withError:(NSError *)error
{
    [self notifySocketDidCloseWithError:error];
}

#pragma mark -
#pragma mark Delegate notifications

- (void)notifySocketDidCloseWithError:(NSError *)error
{
    if ([_delegate respondsToSelector:@selector(multicastSocket:didCloseWithError:)])
    {
        [_delegate multicastSocket:self didCloseWithError:error];
    }
}

- (void)notifySocketDidReceiveMessage:(LunarNetMessage *)msg
{
    if ([_delegate respondsToSelector:@selector(multicastSocket:didReceiveMessage:)])
    {
        [_delegate multicastSocket:self didReceiveMessage:msg];
    }
}

#pragma mark -
#pragma mark IO

- (BOOL)readMulticastHeaderFromBuffer:(LunarNetBuffer *)buffer
{
    int8_t protocolVersion;
    if (![buffer readInt8:&protocolVersion])
    {
        logv(kLogTag, @"Can't receive multicast message: can't read protocol version");
        return NO;
    }
    
    if (protocolVersion != kProtocolVersion)
    {
        logv(kLogTag, @"Can't receive multicast message: unsupported protocol version %d", protocolVersion);
        return NO;
    }
    
    int8_t appIdBytesLen;
    if (![buffer readInt8:&appIdBytesLen])
    {
        logv(kLogTag, @"Can't receive multicast message: can't read app id bytes length");
        return NO;
    }
    
    NSData *appIdBytes;
    if (![buffer readBytes:&appIdBytes numberOfBytes:appIdBytesLen])
    {
        logv(kLogTag, @"Can't receive multicast message: can't read app id");
        return NO;
    }
    
    NSString *appId = [[NSString alloc] initWithData:appIdBytes encoding:NSUTF8StringEncoding];
    if (![appId isEqualToString:_configuration.appId])
    {
        logv(kLogTag, @"Can't receive multicast message: wrong app id '%@'", appId);
        return NO;
    }
    
    return YES;
}

- (void)writeMulticastHeaderToBuffer:(LunarNetBuffer *)buffer
{
    [buffer writeInt8:kProtocolVersion];
    
    NSData *appIdData = [_configuration.appId dataUsingEncoding:NSUTF8StringEncoding];
    [buffer writeInt8:appIdData.length];
    [buffer writeBytes:appIdData];
}

#pragma mark -
#pragma mark Helpers

- (BOOL)isDelegateQueue
{
    return _outgoingMessages.isCurrentQueue;
}

@end
