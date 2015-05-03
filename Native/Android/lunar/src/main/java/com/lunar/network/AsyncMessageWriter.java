package com.lunar.network;

import java.io.IOException;
import java.io.OutputStream;

public abstract class AsyncMessageWriter extends AsyncMessageHandler
{
    protected abstract NetMessage dequeueOutgoingMessage() throws Exception;

    protected abstract void sendMessage(NetMessage msg) throws IOException;

    @Override
    protected void runLoop() throws Exception
    {
        NetMessage msg = dequeueOutgoingMessage();
        sendMessage(msg);
        msg.recycle();
    }

    protected final void writeMessage(OutputStream stream, NetMessage msg) throws IOException
    {
        // header
        int payloadLength = msg.getLength();
        headerBuffer[0] = msg.getMessageType().byteValue();
        headerBuffer[1] = (byte) ((payloadLength >> 24) & 0xff);
        headerBuffer[2] = (byte) ((payloadLength >> 16) & 0xff);
        headerBuffer[3] = (byte) ((payloadLength >> 8) & 0xff);
        headerBuffer[4] = (byte) (payloadLength & 0xff);

        stream.write(headerBuffer);

        // payload
        stream.write(msg.data, 0, msg.getLength());
    }
}
