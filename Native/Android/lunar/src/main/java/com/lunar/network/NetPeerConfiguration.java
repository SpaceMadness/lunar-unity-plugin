package com.lunar.network;

import java.net.Inet4Address;
import java.net.InetSocketAddress;
import java.net.SocketAddress;
import java.net.UnknownHostException;

public class NetPeerConfiguration
{
    private static final int MAX_APP_ID_LENGTH = 128;

    public final String appIdentifier;
    public String networkThreadName;
    public int port;

    public int receiveBufferSize;
    public int sendBufferSize;

    public int backlog;
    public SocketAddress multicastAddress;

    public NetPeerConfiguration(String appIdentifier)
    {
        if (appIdentifier == null || appIdentifier.length() == 0)
        {
            throw new IllegalArgumentException("App identifier is null or empty");
        }

        if (appIdentifier.length() > MAX_APP_ID_LENGTH)
        {
            throw new IllegalArgumentException("App identifier is too long " + appIdentifier.length() +
                    " max length " + MAX_APP_ID_LENGTH);
        }

        this.appIdentifier = appIdentifier;

        networkThreadName = "Lidgren network thread";

        port = 0;
        receiveBufferSize = 131071;
        sendBufferSize = 131071;

        try
        {
            multicastAddress = new InetSocketAddress(Inet4Address.getByName("239.0.0.222"), 10600);
        }
        catch (UnknownHostException e)
        {
            e.printStackTrace();
        }

        backlog = 16;
    }
}