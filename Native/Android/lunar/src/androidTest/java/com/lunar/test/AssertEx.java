package com.lunar.test;

import junit.framework.Assert;
import junit.framework.AssertionFailedError;

/**
 * Created by alementuev on 1/17/15.
 */
public class AssertEx
{
    public static void assertFail(String message)
    {
        throw new AssertionFailedError(message);
    }

    public static void assertArrayEquals(byte[] expected, byte... actual)
    {
        Assert.assertEquals(expected.length, actual.length);
        for (int i = 0; i < expected.length; ++i)
        {
            Assert.assertEquals(expected[i], actual[i]);
        }
    }
}
