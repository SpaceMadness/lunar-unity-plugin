//
//  unity.h
//  Lunar
//
//  Created by Alex Lementuev on 1/20/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

#ifndef Lunar_unity_h
#define Lunar_unity_h

#define PeerHandle int32_t

struct __lunar_net_configuration {
    const char *appId;
    uint32_t multicastAddress;
    uint16_t multicastPort;
};

OBJC_EXTERN int __lunar_unity_peer_create(const struct __lunar_net_configuration config);
OBJC_EXTERN int __lunar_unity_peer_connect(PeerHandle peerHandle, uint32_t address, uint16_t port);
OBJC_EXTERN int __lunar_unity_peer_read_msg_header(PeerHandle peerHandle, uint32_t *headerPtr);
OBJC_EXTERN int __lunar_unity_peer_read_msg_payload(PeerHandle peerHandle, void* buffer, size_t buffer_length);
OBJC_EXTERN int __lunar_unity_peer_send_msg(PeerHandle peerHandle, uint8_t type, const void* buffer, size_t buffer_length);
OBJC_EXTERN int __lunar_unity_peer_send_discovery_request(PeerHandle peerHandle, const void* buffer, size_t buffer_length);
OBJC_EXTERN int __lunar_unity_peer_start(PeerHandle peerHandle);
OBJC_EXTERN int __lunar_unity_peer_stop(PeerHandle peerHandle);
OBJC_EXTERN int __lunar_unity_peer_destroy(PeerHandle peerHandle);

#endif /* Lunar_unity_h */
