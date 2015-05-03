//
//  unity.c
//  Lunar
//
//  Created by Alex Lementuev on 1/19/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//
//
#import <Foundation/Foundation.h>

#import "Lunar.h"

#import "unity.h"
#import "LunarUnityNetClientPeer.h"

#define CHECK_PEER_HANDLE(X) { \
    if (_peerInstance == nil) { NSLog(@"Peer instance is not initialized"); return -1; } \
    if (_peerInstance.handle != peerHandle) { NSLog(@"Unexpected peer handle: %d", peerHandle); return -1; } \
}

static NSString * const kLogTag = @"LunarUnityWrapper";

static LunarUnityNetClientPeer * _peerInstance;
static PeerHandle _peerInstanceHandle;

int __lunar_unity_peer_create(const struct __lunar_net_configuration config)
{
    [_peerInstance stop];
    
    NSString *appId = [NSString stringWithUTF8String:config.appId];
    uint32_t multicastAddress = config.multicastAddress;
    uint16_t multicastPort = config.multicastPort;

    LunarNetConfiguration *peerConfig = [[LunarNetConfiguration alloc] initWithAppId:appId];
    peerConfig.multicastHost = lunarNetAddressToHost(multicastAddress);
    peerConfig.multicastPort = multicastPort;
    
    NSLog(@"Creating lunar peer with app id %@", appId);
    
    _peerInstance = [[LunarUnityNetClientPeer alloc] initWithConfiguration:peerConfig];
    _peerInstanceHandle = _peerInstance.handle;
    
    return _peerInstanceHandle;
}

int __lunar_unity_peer_connect(PeerHandle peerHandle, uint32_t address, uint16_t port)
{
    CHECK_PEER_HANDLE(peerHandle);
    
    LunarIPEndPoint *ep = [LunarIPEndPoint endPointWithAddress:address port:port];
    [_peerInstance connectTo:ep];
    
    return 0;
}

int __lunar_unity_peer_read_msg_header(PeerHandle peerHandle, uint32_t *headerPtr)
{
    CHECK_PEER_HANDLE(peerHandle)
    
    if (headerPtr == NULL)
    {
        NSLog(@"Header ptr is NULL");
        return -1;
    }
    
    LunarNetMessage *msg = [_peerInstance peekMessage];
    if (msg != nil)
    {
        *headerPtr = (uint32_t)((msg.messageType << 24) | (msg.length & 0xffffff));
    }
    else
    {
        *headerPtr = -1;
    }
    
    return 0;
}

int __lunar_unity_peer_read_msg_payload(PeerHandle peerHandle, void* buffer, size_t buffer_length)
{
    CHECK_PEER_HANDLE(peerHandle)
    
    if (buffer == NULL)
    {
        NSLog(@"Buffer is NULL");
        return -1;
    }
    
    LunarNetMessage *msg = [_peerInstance takeMessage];
    if (msg == nil)
    {
        NSLog(@"No message to read");
        return -1;
    }
    
    NSData *msgData = msg.data;
    NSUInteger msgDataLength = msg.length;
    
    if (msg.messageType == LUNetMessageTypeDiscoveryResponse)
    {
        LunarIPEndPoint *remoteEp = msg.remoteEndPoint;
        
        uint32_t remoteAddress = remoteEp.address;
        uint16_t remotePort = remoteEp.port;
        
        LunarNetBuffer *addressBuffer = [[LunarNetBuffer alloc] init];
        [addressBuffer writeInt32:remoteAddress];
        [addressBuffer writeInt16:remotePort];
        
        NSMutableData *data = [[NSMutableData alloc] initWithCapacity:addressBuffer.length + msgDataLength];
        [data appendData:addressBuffer.data];
        [data appendData:msgData];
        
        msgData = data;
        msgDataLength = data.length;
    }
    
    if (msgData.length > buffer_length)
    {
        NSLog(@"Buffer (%ld) is too small to fit message (%ld)", buffer_length, msg.length);
        return -1;
    }
    
    memcpy(buffer, msgData.bytes, msgDataLength);
    return 0;
}

int __lunar_unity_peer_send_msg(PeerHandle peerHandle, uint8_t type, const void* buffer, size_t buffer_length)
{
    CHECK_PEER_HANDLE(peerHandle);
    
    LUNetMessageType msgType = (LUNetMessageType)type;
    
    NSData *data = [[NSData alloc] initWithBytes:buffer length:buffer_length];
    LunarNetMessage *msg = [[LunarNetMessage alloc] initWithType:msgType andData:data];
    
    [_peerInstance sendMessage:msg];
    return 0;
}

int __lunar_unity_peer_send_discovery_request(PeerHandle peerHandle, const void* buffer, size_t buffer_length)
{
    CHECK_PEER_HANDLE(peerHandle);
    
    LunarNetBuffer *payloadBuffer = nil;
    if (buffer_length > 0)
    {
        NSData *payloadData = [[NSData alloc] initWithBytes:buffer length:buffer_length];
        payloadBuffer = [[LunarNetBuffer alloc] initWithData:payloadData];
    }
    
    [_peerInstance sendDiscoveryRequestWithPayload:payloadBuffer];
    return 0;
}

int __lunar_unity_peer_start(PeerHandle peerHandle)
{
    CHECK_PEER_HANDLE(peerHandle);
    
    [_peerInstance start];
    return 0;
}

int __lunar_unity_peer_stop(PeerHandle peerHandle)
{
    CHECK_PEER_HANDLE(peerHandle)
    
    [_peerInstance stop];
    return 0;
}

int __lunar_unity_peer_destroy(PeerHandle peerHandle)
{
    CHECK_PEER_HANDLE(peerHandle)
    
    [_peerInstance stop];
    _peerInstance = nil;
    
    return 0;
}