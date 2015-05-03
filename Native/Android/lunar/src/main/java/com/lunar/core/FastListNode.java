package com.lunar.core;

public class FastListNode
{
    protected FastListNode m_listPrev;
    protected FastListNode m_listNext;
    protected IFastList m_list;

    protected FastListNode ListNodePrev()
    {
        return m_listPrev;
    }

    protected FastListNode ListNodeNext()
    {
        return m_listNext;
    }
}