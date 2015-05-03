package com.lunar.util;

import java.io.ByteArrayInputStream;

/**
 * Created by alementuev on 1/23/15.
 */
public class HackByteArrayInputStream extends ByteArrayInputStream
{
    public HackByteArrayInputStream(int capacity)
    {
        super(new byte[capacity]);
    }

    public byte[] innerBuffer()
    {
        return buf;
    }
}
