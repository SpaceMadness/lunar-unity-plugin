package com.lunar.core;

public class FastListNodeTyped<T extends FastListNode> extends FastListNode
{
	@SuppressWarnings("unchecked")
	@Override
	public T ListNodePrev()
    {
        return (T) m_listPrev;
    }

	@SuppressWarnings("unchecked")
	@Override
    public T ListNodeNext()
    {
        return (T) m_listNext;
    }
}
