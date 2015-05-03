package com.lunar.test;

import android.test.AndroidTestCase;

import com.lunar.network.NetBuffer;

import java.io.IOException;

import static com.lunar.test.AssertEx.*;

public class NetBufferTest extends AndroidTestCase
{
    public void testWriteByte() throws IOException
    {
        byte expected1 = 0;
        byte expected2 = Byte.MIN_VALUE;
        byte expected3 = Byte.MAX_VALUE;

        NetBuffer buffer = new NetBuffer();
        buffer.Write(expected1);
        buffer.Write(expected2);
        buffer.Write(expected3);

        assertEquals(expected1, buffer.ReadByte());
        assertEquals(expected2, buffer.ReadByte());
        assertEquals(expected3, buffer.ReadByte());
    }

    public void testWriteInt16() throws IOException
    {
        short expected1 = 0;
        short expected2 = Short.MIN_VALUE;
        short expected3 = Short.MAX_VALUE;
        short expected4 = 0x1234;

        NetBuffer buffer = new NetBuffer();
        buffer.Write(expected1);
        buffer.Write(expected2);
        buffer.Write(expected3);
        buffer.Write(expected4);

        assertEquals(expected1, buffer.ReadInt16());
        assertEquals(expected2, buffer.ReadInt16());
        assertEquals(expected3, buffer.ReadInt16());
        assertEquals(expected4, buffer.ReadInt16());
    }

    public void testWriteInt32() throws IOException
    {
        int expected1 = 0;
        int expected2 = Integer.MIN_VALUE;
        int expected3 = Integer.MAX_VALUE;
        int expected4 = 0x12345678;

        NetBuffer buffer = new NetBuffer();
        buffer.Write(expected1);
        buffer.Write(expected2);
        buffer.Write(expected3);
        buffer.Write(expected4);

        assertEquals(expected1, buffer.ReadInt32());
        assertEquals(expected2, buffer.ReadInt32());
        assertEquals(expected3, buffer.ReadInt32());
        assertEquals(expected4, buffer.ReadInt32());
    }

    public void testWriteInt64() throws IOException
    {
        long expected1 = 0;
        long expected2 = Long.MIN_VALUE;
        long expected3 = Long.MAX_VALUE;
        long expected4 = 0x123456780abcdeffL;

        NetBuffer buffer = new NetBuffer();
        buffer.Write(expected1);
        buffer.Write(expected2);
        buffer.Write(expected3);
        buffer.Write(expected4);

        assertEquals(expected1, buffer.ReadInt64());
        assertEquals(expected2, buffer.ReadInt64());
        assertEquals(expected3, buffer.ReadInt64());
        assertEquals(expected4, buffer.ReadInt64());
    }

    public void testWriteBytes() throws IOException
    {
        byte[] expected = {1, 2, 3, 4, 5, 6, 7, 8, 9, 0};

        NetBuffer buffer = new NetBuffer();
        buffer.Write(expected);

        byte[] actual = buffer.ReadBytes(expected.length);
        assertArrayEquals(expected, actual);
    }

    public void testWritePartialBytes() throws IOException
    {
        byte[] bytes = {1, 2, 3, 4, 5, 6, 7, 8, 9, 0};

        NetBuffer buffer = new NetBuffer();
        buffer.Write(bytes, 3, 5);

        byte[] expected = subarray(bytes, 3, 5);
        assertArrayEquals(expected, buffer.ReadBytes(5));
    }

    public void testWriteString() throws IOException
    {
        NetBuffer buffer = new NetBuffer();

        String expected1 = null;
        String expected2 = "";
        String expected3 = "Some string";
        String expected4 = "Строка";

        char[] chars = new char[100000];
        for (int i = 0; i < chars.length; ++i)
        {
            chars[i] = (char) (i % Short.MAX_VALUE);
        }
        String expected5 = new String(chars);

        buffer.Write(expected1);
        buffer.Write(expected2);
        buffer.Write(expected3);
        buffer.Write(expected4);
        buffer.Write(expected5);

        assertEquals(expected1, buffer.ReadString());
        assertEquals(expected2, buffer.ReadString());
        assertEquals(expected3, buffer.ReadString());
        assertEquals(expected4, buffer.ReadString());
        assertEquals(expected5, buffer.ReadString());
    }

    public void testReadEmptyByte() throws IOException
    {
        NetBuffer buffer = new NetBuffer();
        try
        {
            buffer.ReadByte();
            assertFail("Should have thrown an IOException");
        }
        catch (IOException e)
        {
        }
    }

    public void testReadEmptyInt16() throws IOException
    {
        NetBuffer buffer = new NetBuffer();
        buffer.Write((byte)0);
        try
        {
            buffer.ReadInt16();
            assertFail("Should have thrown an IOException");
        }
        catch (IOException e)
        {
        }
    }

    public void testReadEmptyInt32() throws IOException
    {
        NetBuffer buffer = new NetBuffer();
        buffer.Write((byte)0);
        buffer.Write((short)0);
        try
        {
            buffer.ReadInt32();
            assertFail("Should have thrown an IOException");
        }
        catch (IOException e)
        {
        }
    }

    public void testReadEmptyInt64() throws IOException
    {
        NetBuffer buffer = new NetBuffer();
        buffer.Write((byte)0);
        buffer.Write((short)0);
        buffer.Write(0);
        try
        {
            buffer.ReadInt64();
            assertFail("Should have thrown an IOException");
        }
        catch (IOException e)
        {
        }
    }

    public void testReadEmptyString() throws IOException
    {
        NetBuffer buffer = new NetBuffer();
        try
        {
            buffer.ReadString();
            assertFail("Should have thrown an IOException");
        }
        catch (IOException e)
        {
        }
    }

    private static byte[] subarray(byte[] arr, int off, int len)
    {
        byte[] subarr = new byte[len];
        System.arraycopy(arr, off, subarr, 0, subarr.length);
        return subarr;
    }
}
