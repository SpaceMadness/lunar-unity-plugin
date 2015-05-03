package com.lunar.network;

import java.io.IOException;
import java.io.OutputStream;

/**
 * Created by alementuev on 1/23/15.
 */
public abstract class AsyncMessageStreamWriter extends AsyncMessageWriter
{
    private final OutputStream stream;

    public AsyncMessageStreamWriter(OutputStream stream)
    {
        if (stream == null)
        {
            throw new NullPointerException("Stream is null");
        }

        this.stream = stream;
    }

    @Override
    protected void sendMessage(NetMessage msg) throws IOException
    {
        writeMessage(stream, msg);
    }
}
