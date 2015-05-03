package com.lunar.util;

import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

/**
 * Created by alementuev on 1/23/15.
 */
public class ThreadPool
{
    private static int NUMBER_OF_CORES = Runtime.getRuntime().availableProcessors();

    private static final int KEEP_ALIVE_TIME = 1;

    private static final TimeUnit KEEP_ALIVE_TIME_UNIT = TimeUnit.SECONDS;

    private final BlockingQueue<Runnable> runnableQueue;

    private final ThreadPoolExecutor threadPool;

    public ThreadPool()
    {
        runnableQueue = new LinkedBlockingQueue<Runnable>();
        threadPool = new ThreadPoolExecutor(
                NUMBER_OF_CORES,       // Initial instance size
                NUMBER_OF_CORES,       // Max instance size
                KEEP_ALIVE_TIME,
                KEEP_ALIVE_TIME_UNIT,
                runnableQueue);
    }

    public void execute(Runnable r)
    {
        if (r == null)
        {
            throw new NullPointerException("Runnable is null");
        }

        threadPool.execute(r);
    }

    public static ThreadPool defaultPool()
    {
        return ThreadPoolHandler.instance;
    }
}

class ThreadPoolHandler
{
    static final ThreadPool instance = new ThreadPool();
}
