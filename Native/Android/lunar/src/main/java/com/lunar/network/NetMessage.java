package com.lunar.network;

import java.net.InetSocketAddress;

public class NetMessage extends NetBuffer
{
    private NetMessageType messageType;
    private InetSocketAddress remoteAddress;

    public NetMessage()
    {
    }

    public NetMessage init(NetMessageType type)
    {
        this.messageType = type;
        return this;
    }

    @Override
    protected void onRecycleObject()
    {
        messageType = null;
        remoteAddress = null;
        super.onRecycleObject();
    }

    public NetMessageType getMessageType()
    {
        return messageType;
    }

    public InetSocketAddress getRemoteAddress()
    {
        return remoteAddress;
    }

    public void setRemoteAddress(InetSocketAddress remoteAddress)
    {
        this.remoteAddress = remoteAddress;
    }

    public void setPayload(byte[] buffer, int length)
    {
        setLength(length);
        System.arraycopy(buffer, 0, data, 0, length);
    }

    public byte[] getPayload()
    {
        return data;
    }
}
