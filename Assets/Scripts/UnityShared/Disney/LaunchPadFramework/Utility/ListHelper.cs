using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.LaunchPadFramework.Utility
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

		public static void AddNullChecked<T>(this List<T> list, T item)
		{
			if (item == null)
			{
				throw new NullReferenceException("Item being added to a null checked list is null");
			}
			list.Add(item);
		}

		public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
		{
			return listToClone.Select((T item) => (T)item.Clone()).ToList();
		}
	}
}
