package com.lunar.util;

public class ArrayUtil
{
	public static byte[] Resize(byte[] arr, int length)
	{
		int oldLength = arr.length;
		if (oldLength != length)
		{
			byte[] temp = new byte[length];
			System.arraycopy(arr, 0, temp, 0, Math.min(oldLength, length));
			return temp;
		}
		
		return arr;
	}
}
