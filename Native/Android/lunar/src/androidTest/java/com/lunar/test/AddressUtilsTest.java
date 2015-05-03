package com.lunar.test;

import android.test.AndroidTestCase;

import com.lunar.network.AddressUtils;

import java.net.InetSocketAddress;

public class AddressUtilsTest extends AndroidTestCase
{
    public void testAddressToHostString()
    {
        int address = packAddress(192, 168, 1, 11);
        assertEquals("192.168.1.11", AddressUtils.toHostString(address));
    }

    public void testAddressParse()
    {
        int address = packAddress(192, 168, 1, 11);
        int port = 10500;

        InetSocketAddress addr = (InetSocketAddress) AddressUtils.createAddress(address, port);
        assertEquals("192.168.1.11", addr.getHostName());
        assertEquals(port, addr.getPort());
    }

    private static int packAddress(int o1, int o2, int o3, int o4)
    {
        return ((o1 & 0xff) << 24) | ((o2 & 0xff) << 16) | ((o3 & 0xff) << 8) | ((o4 & 0xff));
    }
}
