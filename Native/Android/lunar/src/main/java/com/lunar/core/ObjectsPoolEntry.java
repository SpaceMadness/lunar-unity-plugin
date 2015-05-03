package com.lunar.core;

public class ObjectsPoolEntry extends FastListNode
{
	protected IObjectsPool pool;
	
	public void recycle()
	{   
		if (pool != null)
		{
			pool.Recycle(this);
		}
		
		onRecycleObject();
	}
	
	protected void onRecycleObject()
	{
	}
}