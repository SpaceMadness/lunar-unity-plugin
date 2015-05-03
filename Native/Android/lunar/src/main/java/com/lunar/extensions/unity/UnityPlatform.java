package com.lunar.extensions.unity;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.content.DialogInterface;
import android.util.Log;

import com.lunar.util.ServiceUtils;
import com.lunar.util.ThreadUtils;
import com.unity3d.player.UnityPlayer;

public class UnityPlatform
{
    private static final String TAG = "LunarAndroid";

    private static final int[] PRIORITY_LOOKUP =
    {
        Log.VERBOSE, // LogLevel.Verbose
        Log.DEBUG,   // LogLevel.Debug
        Log.INFO,    // LogLevel.Info
        Log.WARN,    // LogLevel.Warn
        Log.ERROR,   // LogLevel.Error
        Log.ASSERT,  // LogLevel.Exception
    };

    public static void nativeMessage(int logLevel, String message)
    {
        Log.println(lookupLogPriority(logLevel), TAG, message);
    }

    public static void assertMessage(final String message, final String stackTrace)
    {
        if (ThreadUtils.isMainThread())
        {
            Context context = getContext();
            showAssertMessage(context, message, stackTrace);
        }
        else
        {
            ThreadUtils.runOnMainThread(new Runnable()
            {
                @Override
                public void run()
                {
                    Context context = getContext();
                    showAssertMessage(context, message, stackTrace);
                }
            });
        }
    }

    private static void showAssertMessage(final Context context, String message, String stackTrace)
    {
        final String fullMessage = message + "\n\n" + stackTrace;

        AlertDialog alertDialog = new AlertDialog.Builder(context).create();
        alertDialog.setTitle("Assertion");
        alertDialog.setMessage(fullMessage);

        alertDialog.setButton(Dialog.BUTTON_NEUTRAL, "Copy", new DialogInterface.OnClickListener()
        {
            public void onClick(DialogInterface dialog, int whichButton)
            {
                ClipboardManager clipboardManager = ServiceUtils.getClipboardManager(context);
                ClipData clipData = ClipData.newPlainText("Assertion", fullMessage);
                clipboardManager.setPrimaryClip(clipData);
            }
        });

        alertDialog.setButton(Dialog.BUTTON_NEGATIVE, "Abort", new DialogInterface.OnClickListener()
        {
            public void onClick(DialogInterface dialog, int whichButton)
            {
                System.exit(1);
            }
        });

        alertDialog.setButton(Dialog.BUTTON_POSITIVE, "Continue", new DialogInterface.OnClickListener()
        {
            public void onClick(DialogInterface dialog, int whichButton)
            {
                dialog.dismiss();
            }
        });
        alertDialog.show();
    }

    public static Context getContext()
    {
        return UnityPlayer.currentActivity;
    }

    private static int lookupLogPriority(int logLevel)
    {
        return logLevel >= 0 && logLevel < PRIORITY_LOOKUP.length ?
                PRIORITY_LOOKUP[logLevel] : Log.DEBUG;
    }
}
