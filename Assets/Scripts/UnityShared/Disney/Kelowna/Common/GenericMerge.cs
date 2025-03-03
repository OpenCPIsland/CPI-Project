using System.Collections.Generic;
using System.Linq;

namespace Disney.Kelowna.Common
{
	public class GenericMerge
	{
		public delegate TKey GetKeyDelegate<TKey, TValue>(TValue value);

		public static IEnumerable<TValue> MergeLists<TKey, TValue>(GetKeyDelegate<TKey, TValue> getKey, params IEnumerable<TValue>[] listsToMerge)
		{
			if (listsToMerge.Length <= 0)
			{
				return null;
			}
			if (listsToMerge.Length == 1)
			{
				return listsToMerge[0];
			}
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			foreach (IEnumerable<TValue> enumerable in listsToMerge)
			{
				foreach (TValue item in enumerable)
				{
					TKey key = getKey(item);
					dictionary[key] = item;
				}
			}
			return dictionary.Values;
		}

		public static IEnumerable<TValue> MergeLists<TValue>(IEqualityComparer<TValue> comparer, params IEnumerable<TValue>[] listsToMerge)
		{
			if (listsToMerge.Length <= 0)
			{
				return null;
			}
			if (listsToMerge.Length == 1)
			{
				return listsToMerge[0];
			}
			HashSet<TValue> hashSet = new HashSet<TValue>(comparer);
			foreach (IEnumerable<TValue> item in listsToMerge.Reverse())
			{
				hashSet.UnionWith(item);
			}
			return hashSet;
		}
	}
}
