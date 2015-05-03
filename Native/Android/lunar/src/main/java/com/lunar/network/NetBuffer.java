package com.lunar.network;

import com.lunar.core.ObjectsPoolEntry;
import com.lunar.util.ArrayUtil;
import com.lunar.util.UTF8;

import java.io.IOException;

public class NetBuffer extends ObjectsPoolEntry
{
    private static final byte[] EMPTY_ARRAY = new byte[0];
    private static final int c_overAllocateAmount = 4;

    protected byte[] data;
    private int byteLength;
    private int readPosition;

    public NetBuffer()
    {
        data = EMPTY_ARRAY;
    }

    @Override
    protected void onRecycleObject()
    {
        byteLength = 0;
        readPosition = 0;
    }

    /// <summary>
    /// Reads a boolean value (stored as a single bit) written using Write(boolean)
    /// </summary>
    public boolean ReadBoolean() throws IOException
    {
        return ReadByte() != 0;
    }

    /// <summary>
    /// Reads a byte
    /// </summary>
    public byte ReadByte() throws IOException
    {
        checkReadAvailable(1);

        byte retval = data[readPosition];
        readPosition += 1;
        return retval;
    }

    /// <summary>
    /// Reads the specified number of bytes
    /// </summary>
    public byte[] ReadBytes(int numberOfBytes) throws IOException
    {
        if (numberOfBytes < 0)
        {
            throw new IllegalArgumentException("Invalid numberOfBytes value: " + numberOfBytes);
        }

        checkReadAvailable(numberOfBytes);

        byte[] retval = new byte[numberOfBytes];
        System.arraycopy(data, readPosition, retval, 0, numberOfBytes);
        readPosition += numberOfBytes;
        return retval;
    }

    /// <summary>
    /// Reads the specified number of bytes into a preallocated array
    /// </summary>
    /// <param name="into">The destination array</param>
    /// <param name="offset">The offset where to start writing in the destination array</param>
    /// <param name="numberOfBytes">The number of bytes to read</param>
    public void ReadBytes(byte[] buffer, int offset, int numberOfBytes) throws IOException
    {
        if (buffer == null)
        {
            throw new NullPointerException("Buffer is null");
        }

        if (offset < 0)
        {
            throw new IllegalArgumentException("Invalid offset value: " + offset);
        }

        if (numberOfBytes < 0)
        {
            throw new IllegalArgumentException("Invalid numberOfBytes value: " + numberOfBytes);
        }

        if (offset + numberOfBytes > buffer.length)
        {
            throw new IllegalArgumentException("Invalid offset and numberOfBytes values");
        }

        checkReadAvailable(numberOfBytes);

        System.arraycopy(data, readPosition, buffer, offset, numberOfBytes);
        readPosition += numberOfBytes;
    }

    /// <summary>
    /// Reads a 16 bit signed integer written using Write(short)
    /// </summary>
    public short ReadInt16() throws IOException
    {
        checkReadAvailable(2);

        int b1 = data[readPosition++] & 0xff;
        int b2 = data[readPosition++] & 0xff;

        return (short) ((b1 << 8) | b2);
    }

    /// <summary>
    /// Reads a 32 bit signed integer written using Write(int)
    /// </summary>
    public int ReadInt32() throws IOException
    {
        checkReadAvailable(4);

        int b1 = data[readPosition++] & 0xff;
        int b2 = data[readPosition++] & 0xff;
        int b3 = data[readPosition++] & 0xff;
        int b4 = data[readPosition++] & 0xff;

        return (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;
    }

    /// <summary>
    /// Reads a 64 bit signed integer written using Write(long)
    /// </summary>
    public long ReadInt64() throws IOException
    {
        checkReadAvailable(8);

        long b1 = data[readPosition++] & 0xff;
        long b2 = data[readPosition++] & 0xff;
        long b3 = data[readPosition++] & 0xff;
        long b4 = data[readPosition++] & 0xff;
        long b5 = data[readPosition++] & 0xff;
        long b6 = data[readPosition++] & 0xff;
        long b7 = data[readPosition++] & 0xff;
        long b8 = data[readPosition++] & 0xff;

        return (b1 << 56) | (b2 << 48) | (b3 << 40) | (b4 << 32) | (b5 << 24) | (b6 << 16) | (b7 << 8) | b8;
    }

    public long ReadVariableUInt32() throws IOException
    {
        int num1 = 0;
        int num2 = 0;
        for (int i = 0; i < 4; ++i) // 4 bytes max
        {
            int num3 = ReadByte();
            num1 |= (num3 & 0x7f) << num2;
            num2 += 7;
            if ((num3 & 0x80) == 0)
            {
                return ((long) num1) & 0xffffffffL;
            }
        }

        throw new IOException("Can't read variable length int");
    }

    public int WriteVariableUInt32(int value)
    {
        int retval = 1;
        int num1 = (int) value;
        while (num1 >= 0x80)
        {
            this.Write((byte) (num1 | 0x80));
            num1 = num1 >> 7;
            retval++;
        }
        this.Write((byte) num1);
        return retval;
    }

    /// <summary>
    /// Reads a String written using Write(String)
    /// </summary>
    public String ReadString() throws IOException
    {
        boolean notNull = ReadBoolean();
        if (notNull)
        {
            int byteLen = (int) ReadVariableUInt32();
            if (byteLen <= 0)
            {
                return "";
            }

            checkReadAvailable(byteLen);

            byte[] bytes = ReadBytes(byteLen);
            try
            {
                return UTF8.GetString(bytes, 0, bytes.length);
            } catch (Exception e)
            {
                throw new IOException(e);
            }
        }

        return null;
    }

    /// <summary>
    /// Writes a boolean value using 1 bit
    /// </summary>
    public void Write(boolean value)
    {
        Write(value ? (byte) 1 : (byte) 0);
    }

    /// <summary>
    /// Write a byte
    /// </summary>
    public void Write(byte value)
    {
        ensureWriteAvailable(1);
        data[byteLength++] = value;
    }

    /// <summary>
    /// Writes all bytes in an array
    /// </summary>
    public void Write(byte[] buffer)
    {
        if (buffer == null)
        {
            throw new NullPointerException("Buffer is null");
        }

        ensureWriteAvailable(buffer.length);
        System.arraycopy(buffer, 0, data, byteLength, buffer.length);
        byteLength += buffer.length;
    }

    /// <summary>
    /// Writes the specified number of bytes from an array
    /// </summary>
    public void Write(byte[] buffer, int offset, int length)
    {
        if (buffer == null)
        {
            throw new NullPointerException("Buffer is null");
        }

        if (offset < 0)
        {
            throw new IllegalArgumentException("Invalid offset value");
        }

        if (length < 0)
        {
            throw new IllegalArgumentException("Invalid length value");
        }

        if (offset + length > buffer.length)
        {
            throw new IllegalArgumentException("Invalid offset and length values");
        }

        ensureWriteAvailable(length);
        System.arraycopy(buffer, offset, data, byteLength, length);
        byteLength += length;
    }

    /// <summary>
    /// Writes a signed 16 bit integer
    /// </summary>
    public void Write(short source)
    {
        ensureWriteAvailable(2);
        data[byteLength++] = (byte) (source >> 8);
        data[byteLength++] = (byte) (source);
    }

    /// <summary>
    /// Writes a 32 bit signed integer
    /// </summary>
    public void Write(int source)
    {
        ensureWriteAvailable(4);
        data[byteLength++] = (byte) (source >> 24);
        data[byteLength++] = (byte) (source >> 16);
        data[byteLength++] = (byte) (source >> 8);
        data[byteLength++] = (byte) (source);
    }

    /// <summary>
    /// Writes a 64 bit signed integer
    /// </summary>
    public void Write(long source)
    {
        ensureWriteAvailable(8);
        data[byteLength++] = (byte) (source >> 56);
        data[byteLength++] = (byte) (source >> 48);
        data[byteLength++] = (byte) (source >> 40);
        data[byteLength++] = (byte) (source >> 32);
        data[byteLength++] = (byte) (source >> 24);
        data[byteLength++] = (byte) (source >> 16);
        data[byteLength++] = (byte) (source >> 8);
        data[byteLength++] = (byte) (source);
    }

    /// <summary>
    /// Write a String
    /// </summary>
    public void Write(String source)
    {
        boolean notNull = source != null;
        Write(notNull);

        if (notNull)
        {
            if (source.length() == 0)
            {
                WriteVariableUInt32(0);
                return;
            }

            byte[] bytes = UTF8.GetBytes(source);
            WriteVariableUInt32(bytes.length);
            Write(bytes);
        }
    }

    private void checkReadAvailable(int amount) throws IOException
    {
        if (readAvailable() < amount)
        {
            throw new IOException("Requested read amount: " + amount + " available: " + readAvailable());
        }
    }

    private void ensureWriteAvailable(int amount)
    {
        int available = writeAvailable();
        if (available < amount)
        {
            int newLength = data.length + (amount - available) + c_overAllocateAmount;
            data = ArrayUtil.Resize(data, newLength);
        }
    }

    public int getLength()
    {
        return byteLength;
    }

    public void setLength(int length)
    {
        int oldLength = this.byteLength;
        this.byteLength = length;
        if (oldLength < length)
        {
            data = ArrayUtil.Resize(data, length);
        }
    }

    private int readAvailable()
    {
        return byteLength - readPosition;
    }

    private int writeAvailable()
    {
        return data.length - byteLength;
    }
}