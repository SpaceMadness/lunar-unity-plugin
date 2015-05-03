package com.lunar.network;

public class ByteBuffer
{
    byte[] buffer;
    private int length;

    public ByteBuffer(int capacity)
    {
        if (capacity <= 0)
        {
            throw new IllegalArgumentException("Invalid capacity: " + capacity);
        }

        buffer = new byte[capacity];
    }

    public void append(byte[] data) // TODO: java naming conversions
    {
        append(data, 0, data.length);
    }

    public void append(byte[] data, int off, int len)
    {
        if (len > Available())
        {
            throw new IllegalArgumentException("Length " + len + " is more than available " + Available());
        }

        System.arraycopy(data, off, buffer, length, len);
        length += len;
    }

    public void Reset() // TODO: java naming conversions
    {
        length = 0;
    }

    public int Length() // TODO: java naming conversions
    {
        return length;
    }

    public int Available() // TODO: java naming conversions
    {
        return buffer.length - length;
    }

    public int Capacity() // TODO: java naming conversions
    {
        return buffer.length;
    }
}