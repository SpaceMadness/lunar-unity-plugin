package com.lunar.core;

import com.lunar.debug.Assert;
import com.lunar.util.ClassUtils;

/**
 * Created by alementuev on 1/11/15.
 */
public class ObjectsPoolConcurrent<T extends ObjectsPoolEntry> extends ObjectsPool<T>
{
    public ObjectsPoolConcurrent(Class<? extends T> cls)
    {
        super(cls);
    }

    @Override
    public synchronized T NextObject()
    {
        return super.NextObject();
    }

    @Override
    public synchronized void Recycle(ObjectsPoolEntry e)
    {
        super.Recycle(e);
    }

    @Override
    public synchronized void Destroy()
    {
        super.Destroy();
    }
}
