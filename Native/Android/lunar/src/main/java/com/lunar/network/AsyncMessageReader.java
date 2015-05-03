package com.lunar.network;

import java.io.IOException;
import java.io.InputStream;

import static com.lunar.network.NetConstants.*;

public abstract class AsyncMessageReader extends AsyncMessageHandler
{
    protected abstract NetMessage readMessage() throws IOException;

    protected abstract void receiveIncomingMessage(NetMessage msg);

    @Override
    protected void runLoop() throws Exception
    {
        NetMessage msg = readMessage();
        if (msg != null)
        {
            receiveIncomingMessage(msg);
        }
    }

    protected NetMessage readMessage(InputStream stream) throws IOException
    {
        // header
        readBuffer(stream, headerBuffer);

        NetMessageType type = NetMessageType.valueOf(headerBuffer[0]);
        if (type == null)
        {
            throw new IOException("Unexpected message type: " + headerBuffer[0]);   
        }

        int payloadLength = ((headerBuffer[1] & 0xff) << 24) |
                            ((headerBuffer[2] & 0xff) << 16) |
                            ((headerBuffer[3] & 0xff) << 8) |
                             (headerBuffer[4] & 0xff);

        if (payloadLength > MAX_PAYLOAD_LENGTH)
        {
            throw new IOException("Message is too big: " + payloadLength);
        }

        NetMessage msg = createMessage(type);
        msg.setLength(payloadLength);

        // payload
        readBuffer(stream, msg.data, 0, payloadLength);

        return msg;
    }

    protected NetMessage createMessage(NetMessageType type)
    {
        return NetMessagePool.getMessage(type);
    }

    protected void readBuffer(InputStream stream, byte[] buffer) throws IOException
    {
        readBuffer(stream, buffer, 0, buffer.length);
    }

    protected void readBuffer(InputStream stream, byte[] buffer, int off, int len) throws IOException
    {
        int bytesRemain = len;
        while (bytesRemain > 0)
        {
            int bytesReceived = stream.read(buffer, off, bytesRemain);
            if (bytesReceived == -1)
            {
                throw new IOException("End of stream reached");
            }

            off += bytesReceived;
            bytesRemain -= bytesReceived;
        }
    }
}
