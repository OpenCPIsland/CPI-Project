using System;
using System.Collections;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public static class IListInsertIntoSortedListExtensions
	{
		public static void InsertIntoSortedList<T>(this IList<T> list, T value) where T : IComparable<T>
		{
			list.InsertIntoSortedList(value, (T a, T b) => a.CompareTo(b));
		}

		public static void InsertIntoSortedList<T>(this IList<T> list, T value, Comparison<T> comparison)
		{
			int num = 0;
			int num2 = list.Count;
			while (num2 > num)
			{
				int num3 = num2 - num;
				int num4 = num + num3 / 2;
				T x = list[num4];
				int num5 = comparison(x, value);
				if (num5 == 0)
				{
					list.Insert(num4, value);
					return;
				}
				if (num5 < 0)
				{
					num = num4 + 1;
				}
				else
				{
					num2 = num4;
				}
			}
			list.Insert(num, value);
		}

		public static void InsertIntoSortedList(this IList list, IComparable value)
		{
			list.InsertIntoSortedList(value, (IComparable a, IComparable b) => a.CompareTo(b));
		}

		public static void InsertIntoSortedList(this IList list, IComparable value, Comparison<IComparable> comparison)
		{
			int num = 0;
			int num2 = list.Count;
			while (num2 > num)
			{
				int num3 = num2 - num;
				int num4 = num + num3 / 2;
				IComparable x = (IComparable)list[num4];
				int num5 = comparison(x, value);
				if (num5 == 0)
				{
					list.Insert(num4, value);
					return;
				}
				if (num5 < 0)
				{
					num = num4 + 1;
				}
				else
				{
					num2 = num4;
				}
			}
			list.Insert(num, value);
		}
	}
}
