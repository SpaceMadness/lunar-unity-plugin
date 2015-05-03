package com.lunar.network;

/** Status for a NetConnection instance */
public enum NetConnectionStatus
{
    /** No connection, or attempt, in place */
    None,

    /** TCP-socket connection established: handshaking */
    Connecting,

    /** Connected */
    Connected,

    /** In the process of disconnecting */
    Disconnecting,

    /** Disconnected */
    Disconnected;

    public byte byteValue()
    {
        return (byte) ordinal();
    }

    public static NetConnectionStatus valueOf(byte value)
    {
        NetConnectionStatus[] values = values();
        return value >= 0 && value < values.length ? values[value] : null;
    }

}