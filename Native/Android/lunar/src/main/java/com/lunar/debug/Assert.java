package com.lunar.debug;

public class Assert
{
    public static void assertTrue(boolean condition, String message)
    {
        if (!condition)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertTrue(boolean condition)
    {
        if (!condition)
        {
            throw new AssertionError("Assertion failed");
        }
    }

    public static void assertFalse(boolean condition, String message)
    {
        if (condition)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertFalse(boolean condition)
    {
        if (condition)
        {
            throw new AssertionError("Assertion failed");
        }
    }

    public static void fail(String message)
    {
        throw new AssertionError(message);
    }

    public static void fail()
    {
        throw new AssertionError("Assertion failed");
    }

    public static void assertEquals(Object expected, Object actual, String message)
    {
        if (expected == null && actual != null || expected != null && actual == null || !expected.equals(actual))
        {
            throw new AssertionError(message);
        }
    }

    public static void assertEquals(Object expected, Object actual)
    {
        if (expected == null && actual != null || expected != null && actual == null || !expected.equals(actual))
        {
            throw new AssertionError("Expected " + expected + " but was " + actual);
        }
    }

    public static void assertEquals(double expected, double actual, double delta, String message)
    {
        if (Math.abs(expected - actual) > delta)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertEquals(double expected, double actual, double delta)
    {
        if (Math.abs(expected - actual) > delta)
        {
            throw new AssertionError("Expected " + expected + " but was " + actual);
        }
    }

    public static void assertEquals(float expected, float actual, float delta, String message)
    {
        if (Math.abs(expected - actual) > delta)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertEquals(float expected, float actual, float delta)
    {
        if (Math.abs(expected - actual) > delta)
        {
            throw new AssertionError("Expected " + expected + " but was " + actual);
        }
    }

    public static void assertEquals(long expected, long actual, String message)
    {
        if (expected != actual)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertEquals(long expected, long actual)
    {
        if (expected != actual)
        {
            throw new AssertionError("Expected " + expected + " but was " + actual);
        }
    }

    public static void assertEquals(boolean expected, boolean actual, String message)
    {
        if (expected != actual)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertEquals(boolean expected, boolean actual)
    {
        if (expected != actual)
        {
            throw new AssertionError("Expected " + expected + " but was " + actual);
        }
    }

    public static void assertEquals(byte expected, byte actual, String message)
    {
        if (expected != actual)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertEquals(byte expected, byte actual)
    {
        if (expected != actual)
        {
            throw new AssertionError("Expected " + expected + " but was " + actual);
        }
    }

    public static void assertEquals(char expected, char actual, String message)
    {
        if (expected != actual)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertEquals(char expected, char actual)
    {
        if (expected != actual)
        {
            throw new AssertionError("Expected " + expected + " but was " + actual);
        }
    }

    public static void assertEquals(short expected, short actual, String message)
    {
        if (expected != actual)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertEquals(short expected, short actual)
    {
        if (expected != actual)
        {
            throw new AssertionError("Expected " + expected + " but was " + actual);
        }
    }

    public static void assertEquals(int expected, int actual, String message)
    {
        if (expected != actual)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertEquals(int expected, int actual)
    {
        if (expected != actual)
        {
            throw new AssertionError("Expected " + expected + " but was " + actual);
        }
    }

    public static void assertNotNull(Object object)
    {
        if (object == null)
        {
            throw new AssertionError("Object is null");
        }
    }

    public static void assertNotNull(Object object, String message)
    {
        if (object == null)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertNull(Object object)
    {
        {
            if (object != null)
            {
                throw new AssertionError("Object is not null");
            }
        }
    }

    public static void assertNull(Object object, String message)
    {
        if (object != null)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertSame(Object expected, Object actual, String message)
    {
        if (expected != actual)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertSame(Object expected, Object actual)
    {
        if (expected != actual)
        {
            throw new AssertionError("Objects are not same");
        }
    }

    public static void assertNotSame(Object expected, Object actual, String message)
    {
        if (expected == actual)
        {
            throw new AssertionError(message);
        }
    }

    public static void assertNotSame(Object expected, Object actual)
    {
        if (expected == actual)
        {
            throw new AssertionError("Objects are same");
        }
    }

    public static void True(boolean condition)
    {
        if (!condition)
        {
            throw new AssertionError();
        }
    }

    public static void True(boolean condition, String format, Object... args)
    {

    }

    public static void range(int value, int min, int max)
    {
        if (value < min && value > max)
        {
            throw new AssertionError(String.format("Value %d is out of range [%d..%d]", value, min, max));
        }
    }
}
