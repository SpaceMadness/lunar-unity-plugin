package com.lunar.network;

import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.SocketAddress;

public class AddressUtils
{
    private AddressUtils()
    {
    }

    public static SocketAddress createAddress(int address, int port)
    {
        if (port < 0 || port > 65535)
        {
            throw new IllegalArgumentException("Port is out of range: " + port);
        }

        String host = toHostString(address);
        return new InetSocketAddress(host, port);
    }

    public static String toHostString(int address)
    {
        int o1 = (address >> 24) & 0xff;
        int o2 = (address >> 16) & 0xff;
        int o3 = (address >> 8) & 0xff;
        int o4 = address & 0xff;
        return String.format("%d.%d.%d.%d", o1, o2, o3, o4);
    }
}
