package com.lunar.network;

import com.lunar.debug.Assert;

public class NetBitWriter
{
    /// <summary>
    /// Read 1-8 bits from a buffer into a byte
    /// </summary>
    public static byte ReadByte(byte[] fromBuffer, int numberOfBits, int readBitOffset)
    {
        NetException.Assert(((numberOfBits > 0) && (numberOfBits < 9)), "Read() can only read between 1 and 8 bits");

        int bytePtr = readBitOffset >> 3;
        int startReadAtIndex = readBitOffset - (bytePtr * 8); // (readBitOffset % 8);

        if (startReadAtIndex == 0 && numberOfBits == 8)
            return fromBuffer[bytePtr];

        // mask away unused bits lower than (right of) relevant bits in first byte
        byte returnValue = (byte) (fromBuffer[bytePtr] >> startReadAtIndex);

        int numberOfBitsInSecondByte = numberOfBits - (8 - startReadAtIndex);

        if (numberOfBitsInSecondByte < 1)
        {
            // we don't need to read from the second byte, but we DO need
            // to mask away unused bits higher than (left of) relevant bits
            return (byte) (returnValue & (255 >> (8 - numberOfBits)));
        }

        byte second = fromBuffer[bytePtr + 1];

        // mask away unused bits higher than (left of) relevant bits in second byte
        second &= (byte) (255 >> (8 - numberOfBitsInSecondByte));

        return (byte) (returnValue | (byte) (second << (numberOfBits - numberOfBitsInSecondByte)));
    }

    /// <summary>
    /// Read several bytes from a buffer
    /// </summary>
    public static void ReadBytes(byte[] fromBuffer, int numberOfBytes, int readBitOffset, byte[] destination, int destinationByteOffset)
    {
        int readPtr = readBitOffset >> 3;
        int startReadAtIndex = readBitOffset - (readPtr * 8); // (readBitOffset % 8);

        if (startReadAtIndex == 0)
        {
            System.arraycopy(fromBuffer, readPtr, destination, destinationByteOffset, numberOfBytes);
            return;
        }

        int secondPartLen = 8 - startReadAtIndex;
        int secondMask = 255 >> secondPartLen;

        for (int i = 0; i < numberOfBytes; i++)
        {
            // mask away unused bits lower than (right of) relevant bits in byte
            int b = fromBuffer[readPtr] >> startReadAtIndex;

            readPtr++;

            // mask away unused bits higher than (left of) relevant bits in second byte
            int second = fromBuffer[readPtr] & secondMask;

            destination[destinationByteOffset++] = (byte) (b | (second << secondPartLen));
        }

        return;
    }

    /// <summary>
    /// Write 0-8 bits of data to buffer
    /// </summary>
    public static void WriteByte(byte source, int numberOfBits, byte[] destination, int destBitOffset)
    {
        if (numberOfBits == 0)
            return;

        NetException.Assert(((numberOfBits >= 0) && (numberOfBits <= 8)), "Must write between 0 and 8 bits!");

        // Mask out all the bits we dont want
        source = (byte) (source & (0xFF >> (8 - numberOfBits)));

        int p = destBitOffset >> 3;
        int bitsUsed = destBitOffset & 0x7; // mod 8
        int bitsFree = 8 - bitsUsed;
        int bitsLeft = bitsFree - numberOfBits;

        // Fast path, everything fits in the first byte
        if (bitsLeft >= 0)
        {
            int mask = (0xFF >> bitsFree) | (0xFF << (8 - bitsLeft));

            destination[p] = (byte) (
                    // Mask out lower and upper bits
                    (destination[p] & mask) |

                            // Insert new bits
                            (source << bitsUsed)
            );

            return;
        }

        destination[p] = (byte) (
                // Mask out upper bits
                (destination[p] & (0xFF >> bitsFree)) |

                        // Write the lower bits to the upper bits in the first byte
                        (source << bitsUsed)
        );

        p += 1;

        destination[p] = (byte) (
                // Mask out lower bits
                (destination[p] & (0xFF << (numberOfBits - bitsFree))) |

                        // Write the upper bits to the lower bits of the second byte
                        (source >> bitsFree)
        );
    }

    /// <summary>
    /// Write several whole bytes
    /// </summary>
    public static void WriteBytes(byte[] source, int sourceByteOffset, int numberOfBytes, byte[] destination, int destBitOffset)
    {
        int dstBytePtr = destBitOffset >> 3;
        int firstPartLen = (destBitOffset % 8);

        if (firstPartLen == 0)
        {
            System.arraycopy(source, sourceByteOffset, destination, dstBytePtr, numberOfBytes);
            return;
        }

        int lastPartLen = 8 - firstPartLen;

        for (int i = 0; i < numberOfBytes; i++)
        {
            byte src = source[sourceByteOffset + i];

            // write last part of this byte
            destination[dstBytePtr] &= (byte) (255 >> lastPartLen); // clear before writing
            destination[dstBytePtr] |= (byte) (src << firstPartLen); // write first half

            dstBytePtr++;

            // write first part of next byte
            destination[dstBytePtr] &= (byte) (255 << firstPartLen); // clear before writing
            destination[dstBytePtr] |= (byte) (src >> lastPartLen); // write second half
        }

        return;
    }

    public static short ReadUInt16(byte[] fromBuffer, int numberOfBits, int readBitOffset)
    {
        Assert.True(((numberOfBits > 0) && (numberOfBits <= 16)), "ReadUInt16() can only read between 1 and 16 bits");
        short returnValue;
        if (numberOfBits <= 8)
        {
            returnValue = ReadByte(fromBuffer, numberOfBits, readBitOffset);
            return returnValue;
        }
        returnValue = ReadByte(fromBuffer, 8, readBitOffset);
        numberOfBits -= 8;
        readBitOffset += 8;

        if (numberOfBits <= 8)
        {
            returnValue |= (short) (ReadByte(fromBuffer, numberOfBits, readBitOffset) << 8);
        }

        return returnValue;
    }

    public static int ReadUInt32(byte[] fromBuffer, int numberOfBits, int readBitOffset)
    {
        NetException.Assert(((numberOfBits > 0) && (numberOfBits <= 32)), "ReadUInt32() can only read between 1 and 32 bits");
        int returnValue;
        if (numberOfBits <= 8)
        {
            returnValue = ReadByte(fromBuffer, numberOfBits, readBitOffset);
            return returnValue;
        }
        returnValue = ReadByte(fromBuffer, 8, readBitOffset);
        numberOfBits -= 8;
        readBitOffset += 8;

        if (numberOfBits <= 8)
        {
            returnValue |= (int) (ReadByte(fromBuffer, numberOfBits, readBitOffset) << 8);
            return returnValue;
        }
        returnValue |= (int) (ReadByte(fromBuffer, 8, readBitOffset) << 8);
        numberOfBits -= 8;
        readBitOffset += 8;

        if (numberOfBits <= 8)
        {
            int r = ReadByte(fromBuffer, numberOfBits, readBitOffset);
            r <<= 16;
            returnValue |= r;
            return returnValue;
        }
        returnValue |= (int) (ReadByte(fromBuffer, 8, readBitOffset) << 16);
        numberOfBits -= 8;
        readBitOffset += 8;

        returnValue |= (int) (ReadByte(fromBuffer, numberOfBits, readBitOffset) << 24);

        return returnValue;
    }

    public static void WriteUInt16(short source, int numberOfBits, byte[] destination, int destinationBitOffset)
    {
        if (numberOfBits == 0)
            return;

        NetException.Assert((numberOfBits >= 0 && numberOfBits <= 16), "numberOfBits must be between 0 and 16");
        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) source, numberOfBits, destination, destinationBitOffset);
            return;
        }

        NetBitWriter.WriteByte((byte) source, 8, destination, destinationBitOffset);

        numberOfBits -= 8;
        if (numberOfBits > 0)
            NetBitWriter.WriteByte((byte) (source >> 8), numberOfBits, destination, destinationBitOffset + 8);
    }

    public static int WriteUInt32(int source, int numberOfBits, byte[] destination, int destinationBitOffset)
    {
        int returnValue = destinationBitOffset + numberOfBits;
        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) source, numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }
        NetBitWriter.WriteByte((byte) source, 8, destination, destinationBitOffset);
        destinationBitOffset += 8;
        numberOfBits -= 8;

        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) (source >> 8), numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }
        NetBitWriter.WriteByte((byte) (source >> 8), 8, destination, destinationBitOffset);
        destinationBitOffset += 8;
        numberOfBits -= 8;

        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) (source >> 16), numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }
        NetBitWriter.WriteByte((byte) (source >> 16), 8, destination, destinationBitOffset);
        destinationBitOffset += 8;
        numberOfBits -= 8;

        NetBitWriter.WriteByte((byte) (source >> 24), numberOfBits, destination, destinationBitOffset);
        return returnValue;
    }

    public static int WriteUInt64(long source, int numberOfBits, byte[] destination, int destinationBitOffset)
    {
        int returnValue = destinationBitOffset + numberOfBits;
        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) source, numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }
        NetBitWriter.WriteByte((byte) source, 8, destination, destinationBitOffset);
        destinationBitOffset += 8;
        numberOfBits -= 8;

        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) (source >> 8), numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }
        NetBitWriter.WriteByte((byte) (source >> 8), 8, destination, destinationBitOffset);
        destinationBitOffset += 8;
        numberOfBits -= 8;

        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) (source >> 16), numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }
        NetBitWriter.WriteByte((byte) (source >> 16), 8, destination, destinationBitOffset);
        destinationBitOffset += 8;
        numberOfBits -= 8;

        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) (source >> 24), numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }
        NetBitWriter.WriteByte((byte) (source >> 24), 8, destination, destinationBitOffset);
        destinationBitOffset += 8;
        numberOfBits -= 8;

        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) (source >> 32), numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }
        NetBitWriter.WriteByte((byte) (source >> 32), 8, destination, destinationBitOffset);
        destinationBitOffset += 8;
        numberOfBits -= 8;

        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) (source >> 40), numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }
        NetBitWriter.WriteByte((byte) (source >> 40), 8, destination, destinationBitOffset);
        destinationBitOffset += 8;
        numberOfBits -= 8;

        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) (source >> 48), numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }
        NetBitWriter.WriteByte((byte) (source >> 48), 8, destination, destinationBitOffset);
        destinationBitOffset += 8;
        numberOfBits -= 8;

        if (numberOfBits <= 8)
        {
            NetBitWriter.WriteByte((byte) (source >> 56), numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }
        NetBitWriter.WriteByte((byte) (source >> 56), 8, destination, destinationBitOffset);
        destinationBitOffset += 8;
        numberOfBits -= 8;

        return returnValue;
    }
}
