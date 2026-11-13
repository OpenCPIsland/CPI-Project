using System;

namespace Tweaker.Core
{
	public static class PrimitiveHelper
	{
		public static object Add(object a, object b)
		{
			Type type = a.GetType();
			if (type != b.GetType() || !type.IsPrimitive)
			{
				return null;
			}
			if (type == typeof(short))
			{
				return (short)a + (short)b;
			}
			if (type == typeof(ushort))
			{
				return (ushort)a + (ushort)b;
			}
			if (type == typeof(int))
			{
				return (int)a + (int)b;
			}
			if (type == typeof(uint))
			{
				return (uint)a + (uint)b;
			}
			if (type == typeof(long))
			{
				return (long)a + (long)b;
			}
			if (type == typeof(ulong))
			{
				return (ulong)a + (ulong)b;
			}
			if (type == typeof(float))
			{
				return (float)a + (float)b;
			}
			if (type == typeof(double))
			{
				return (double)a + (double)b;
			}
			if (type == typeof(byte))
			{
				return (byte)a + (byte)b;
			}
			if (type == typeof(sbyte))
			{
				return (sbyte)a + (sbyte)b;
			}
			if (type == typeof(char))
			{
				return (char)a + (char)b;
			}
			return null;
		}

		public static object Subtract(object a, object b)
		{
			Type type = a.GetType();
			if (type != b.GetType() || !type.IsPrimitive)
			{
				return null;
			}
			if (type == typeof(short))
			{
				return (short)a - (short)b;
			}
			if (type == typeof(ushort))
			{
				return (ushort)a - (ushort)b;
			}
			if (type == typeof(int))
			{
				return (int)a - (int)b;
			}
			if (type == typeof(uint))
			{
				return (uint)a - (uint)b;
			}
			if (type == typeof(long))
			{
				return (long)a - (long)b;
			}
			if (type == typeof(ulong))
			{
				return (ulong)a - (ulong)b;
			}
			if (type == typeof(float))
			{
				return (float)a - (float)b;
			}
			if (type == typeof(double))
			{
				return (double)a - (double)b;
			}
			if (type == typeof(byte))
			{
				return (byte)a - (byte)b;
			}
			if (type == typeof(sbyte))
			{
				return (sbyte)a - (sbyte)b;
			}
			if (type == typeof(char))
			{
				return (char)a - (char)b;
			}
			return null;
		}
	}
}
