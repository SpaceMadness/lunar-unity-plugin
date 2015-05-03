package com.lunar.network;

import java.io.IOException;
import java.io.InputStream;

/**
 * Created by alementuev on 1/23/15.
 */
public abstract class AsyncMessageStreamReader extends AsyncMessageReader
{
    private final InputStream stream;

    public AsyncMessageStreamReader(InputStream stream)
    {
        if (stream == null)
        {
            throw new NullPointerException("Stream is null");
        }

        this.stream = stream;
    }

    @Override
    protected NetMessage readMessage() throws IOException
    {
        return readMessage(stream);
    }
}
