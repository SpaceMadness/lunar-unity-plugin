package com.lunar.network;

public enum NetMessageType
{
    Data,
    Error,
    StatusChanged,
    ConnectionRequest,
    ConnectionResponse,
    DiscoveryRequest,
    DiscoveryResponse;

    public byte byteValue()
    {
        return (byte) ordinal();
    }

    public static NetMessageType valueOf(byte value)
    {
        NetMessageType[] values = values();
        return value >= 0 && value < values.length ? values[value] : null;
    }
}
