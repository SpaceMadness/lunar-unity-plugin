package com.lunar.util;

import com.lunar.debug.Assert;
import com.lunar.debug.Log;

import java.lang.reflect.Constructor;

public class ClassUtils
{
    /**
     * Tries to cast the object to the specified class. Fires an assertion and
     * return null if class cast is impossible.
     */
    public static <T> T cast(Object obj, Class<T> clazz)
    {
        if (obj != null)
        {
            T casted = as(obj, clazz);
            Assert.True(casted != null, "Can't cast from '%s' to '%s'", obj.getClass(), clazz);

            return casted;
        }
        return null;
    }

    /**
     * Tries to cast the object to the specified class. Returns null if class
     * cast is impossible.
     */
    @SuppressWarnings("unchecked")
    public static <T> T as(Object obj, Class<T> clazz)
    {
        if (obj != null && clazz.isInstance(obj))
        {
            return (T) obj;
        }

        return null;
    }

    /**
     * Tries to create an instance of the class by calling default constructor.
     * Returns null is instantiation fails or passed class object is null.
     */
    public static <T> T tryNewInstance(Class<? extends T> cls)
    {
        try
        {
            if (cls != null)
            {
                Constructor<? extends T> defaultConstructor = cls.getConstructor();
                return as(defaultConstructor.newInstance(), cls);
            }
        }
        catch (Exception e)
        {
            Log.logCrit("Unable to instantiate class %s: %s", cls, e.getMessage());
        }
        return null;
    }
}