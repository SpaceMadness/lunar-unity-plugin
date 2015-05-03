package com.lunar.util;

import java.nio.charset.Charset;

public class UTF8
{
	private static Charset CHARSET;
	
	static
	{
		CHARSET = Charset.forName("UTF-8");
	}

	public static String GetString(byte[] data, int off, int len)
	{
		return new String(data, off, len, CHARSET);
	}

	public static byte[] GetBytes(String source)
	{
		return source.getBytes(CHARSET);
	}
}
