package com.lunar.util;

import android.os.Handler;
import android.os.Looper;

/**
 * Created by alementuev on 2/16/15.
 */
public class ThreadUtils
{
    public static boolean isMainThread()
    {
        return Looper.myLooper() == Looper.getMainLooper();
    }

    public static void runOnMainThread(Runnable r)
    {
        getMainHandler().post(r);
    }

    private static Handler getMainHandler()
    {
        return HandlerInstance.handler;
    }

    private static class HandlerInstance
    {
        public static final Handler handler = new Handler(Looper.getMainLooper());
    }
}
