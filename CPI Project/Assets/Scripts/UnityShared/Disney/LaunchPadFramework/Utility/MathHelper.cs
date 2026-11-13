using System;

namespace Disney.LaunchPadFramework.Utility
{
	public static class MathHelper
	{
		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0)
			{
				return min;
			}
			if (val.CompareTo(max) > 0)
			{
				return max;
			}
			return val;
		}

		public static int LCM(int a, int b)
		{
			return a / GCF(a, b) * b;
		}

		public static int GCF(int a, int b)
		{
			while (b != 0)
			{
				int num = b;
				b = a % b;
				a = num;
			}
			return a;
		}
	}
}
