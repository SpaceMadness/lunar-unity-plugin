package com.lunar.core;

/**
 * Created by alementuev on 1/10/15.
 */
public class ObjectsPoolObjectEntry<T> extends ObjectsPoolEntry
{
    private T object;

    public ObjectsPoolObjectEntry<T> init(T object)
    {
        this.object = object;
        return this;
    }

    @Override
    protected void onRecycleObject()
    {
        this.object = null;
        super.onRecycleObject();
    }

    protected T getObject()
    {
        return object;
    }
}
