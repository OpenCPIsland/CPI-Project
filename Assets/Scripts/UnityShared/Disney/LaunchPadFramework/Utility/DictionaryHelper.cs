using System.Collections.Generic;

namespace Disney.LaunchPadFramework.Utility
{
	public static class DictionaryHelper
	{
		public static bool SafeTryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
		{
			if (dictionary.ContainsKey(key))
			{
				value = dictionary[key];
				return true;
			}
			value = default(TValue);
			return false;
		}
	}
}
