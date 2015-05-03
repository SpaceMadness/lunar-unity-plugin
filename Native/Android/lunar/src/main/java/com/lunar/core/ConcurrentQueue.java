package com.lunar.core;

/**
 * Created by alementuev on 1/11/15.
 */
public class ConcurrentQueue<T extends FastListNode>
{
    private FastList<T> list;

    public ConcurrentQueue()
    {
        list = new FastList<T>();
    }

    public synchronized void enqueue(T e)
    {
        list.AddLastItem(e);
        notifyAll();
    }

    public synchronized T dequeue()
    {
        return list.RemoveFirstItem();
    }

    public synchronized T peek()
    {
        return list.ListFirst();
    }

    public synchronized T take() throws InterruptedException
    {
        T t;
        while ((t = dequeue()) == null)
        {
            wait();
        }

        return t;
    }

    public synchronized boolean isEmpty()
    {
        return list.Count() == 0;
    }
}
