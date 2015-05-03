package com.lunar.network;

import com.lunar.core.FastListNode;

import java.net.SocketAddress;

/**
 * Created by alementuev on 1/28/15.
 */
public class NetTuple extends FastListNode
{
    public final SocketAddress address;
    public final NetMessage message;

    public NetTuple(SocketAddress address, NetMessage message)
    {
        if (address == null)
        {
            throw new NullPointerException("Address is null");
        }

        if (message == null)
        {
            throw new NullPointerException("Message is null");
        }

        this.address = address;
        this.message = message;
    }
}
