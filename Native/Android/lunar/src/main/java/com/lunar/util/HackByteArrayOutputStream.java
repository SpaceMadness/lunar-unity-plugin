package com.lunar.util;

import java.io.ByteArrayOutputStream;

/**
 * Created by alementuev on 1/23/15.
 */
public class HackByteArrayOutputStream extends ByteArrayOutputStream
{
    public byte[] internalBuffer()
    {
        return this.buf;
    }
}
