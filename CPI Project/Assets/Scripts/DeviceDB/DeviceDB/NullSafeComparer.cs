using System;

namespace DeviceDB
{
	internal static class NullSafeComparer
	{
		public static int Compare<T>(IComparable<T> a, T b)
		{
			return (a == null) ? ((b != null) ? (-1) : 0) : ((b == null) ? 1 : a.CompareTo(b));
		}

		public static int Compare(IComparable a, object b)
		{
			return (a == null) ? ((b != null) ? (-1) : 0) : ((b == null) ? 1 : a.CompareTo(b));
		}
	}
}
