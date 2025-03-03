using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public static class DictionaryUtils
	{
		public static TValue TryGetValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
		{
			if (dictionary == null)
			{
				return default(TValue);
			}
			TValue value;
			dictionary.TryGetValue(key, out value);
			return value;
		}
	}
}
