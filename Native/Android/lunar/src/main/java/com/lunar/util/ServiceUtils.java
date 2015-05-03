package com.lunar.util;

import android.app.Activity;
import android.content.ClipboardManager;
import android.content.Context;

import com.lunar.debug.Log;

/**
 * Created by alementuev on 2/16/15.
 */
public class ServiceUtils
{
    public static ClipboardManager getClipboardManager(Context context)
    {
        return getSystemService(context, Activity.CLIPBOARD_SERVICE, ClipboardManager.class);
    }

    private static <T> T getSystemService(Context context, String name, Class<? extends T> clazz)
    {
        if (context == null)
        {
            throw new NullPointerException("Context is null");
        }

        if (name == null)
        {
            throw new NullPointerException("Name is null");
        }

        if (clazz == null)
        {
            throw new NullPointerException("Class is null");
        }

        try
        {
            return ClassUtils.cast(context.getSystemService(name), clazz);
        }
        catch (Exception e)
        {
            Log.logException(e, "Unable to get system service: %s", name);
        }

        return null;
    }
}
