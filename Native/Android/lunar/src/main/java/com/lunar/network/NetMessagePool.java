package com.lunar.network;

import com.lunar.core.ObjectsPool;
import com.lunar.core.ObjectsPoolConcurrent;

/**
 * Created by alementuev on 1/23/15.
 */
public class NetMessagePool
{
    private final ObjectsPool<NetMessage> pool;

    NetMessagePool()
    {
        pool = new ObjectsPoolConcurrent<NetMessage>(NetMessage.class);
    }

    public static NetMessage getMessage(NetMessageType type)
    {
        return NetMessagePoolHandler.instance.getMessage0(type);
    }

    private NetMessage getMessage0(NetMessageType type)
    {
        return pool.NextObject().init(type);
    }
}

class NetMessagePoolHandler
{
    static final NetMessagePool instance = new NetMessagePool();
}