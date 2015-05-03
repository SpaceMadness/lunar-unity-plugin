package com.lunar.network;

public abstract class AsyncMessageHandler
{
    private Thread thread;
    protected final byte[] headerBuffer;

    public AsyncMessageHandler()
    {
        headerBuffer = new byte[NetConstants.HEADER_BYTE_SIZE];
    }

    public void start()
    {
        thread = new Thread(createRunnable());
        thread.setPriority(Thread.MIN_PRIORITY);
        thread.start();
    }

    public void stop()
    {
        if (thread != null)
        {
            thread.interrupt();
            thread = null;
        }
    }

    private Runnable createRunnable()
    {
        return new Runnable()
        {
            @Override
            public void run()
            {
                try
                {
                    guardedRun();
                } catch (Exception e)
                {
                    e.printStackTrace();
                    handlerMessageError(e.getMessage());
                }
            }
        };
    }

    private void guardedRun() throws Exception
    {
        while (!Thread.currentThread().isInterrupted())
        {
            runLoop();
        }
    }

    protected abstract void runLoop() throws Exception;

    protected abstract void handlerMessageError(String message);
}
