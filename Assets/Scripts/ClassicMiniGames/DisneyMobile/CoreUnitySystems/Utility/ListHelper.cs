using System;
using System.Collections.Generic;
using System.Linq;

namespace DisneyMobile.CoreUnitySystems.Utility
{
	public static class ListHelper
	{
		public static void AddIfUnique<T>(this List<T> list, T item)
		{
			if (list.IndexOf(item) == -1)
			{
				list.Add(item);
			}
		}

		public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
		{
			return listToClone.Select((T item) => (T)item.Clone()).ToList();
		}
	}
}
