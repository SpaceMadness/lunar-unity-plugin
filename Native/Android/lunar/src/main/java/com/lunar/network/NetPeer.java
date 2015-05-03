package com.lunar.network;

import java.net.SocketAddress;

public abstract class NetPeer
{
    private final NetPeerConfiguration configuration;
    private SocketAddress remoteAddress;

    public NetPeer(NetPeerConfiguration config)
    {
        if (config == null)
        {
            throw new NullPointerException("Configuration is null");
        }

        this.configuration = config;
    }

    public abstract void start();

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Connection

    public abstract void connect(SocketAddress address);

    public abstract void close();

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Messages

    public abstract NetMessage readMessage();

    public abstract void sendMessage(NetMessage msg);

    public abstract NetMessage createMessage();

    public abstract void sendDiscoveryRequest(NetMessage payload);

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Helpers

    public void sendDiscoveryRequest()
    {
        sendDiscoveryRequest(null);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Getters/Setters

    protected NetPeerConfiguration getConfiguration()
    {
        return configuration;
    }

    public SocketAddress getRemoteAddress()
    {
        return remoteAddress;
    }

    protected void setRemoteAddress(SocketAddress remoteAddress)
    {
        this.remoteAddress = remoteAddress;
    }
}