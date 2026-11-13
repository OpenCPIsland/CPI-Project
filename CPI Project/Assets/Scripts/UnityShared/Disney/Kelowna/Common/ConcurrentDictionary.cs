using System.Collections.Generic;

namespace Disney.Kelowna.Common
{
	public class ConcurrentDictionary<TKey, TValue>
	{
		private object sync = new object();

		private IDictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

		public TValue GetOrAdd(TKey key, TValue value)
		{
			TValue result = value;
			lock (sync)
			{
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, value);
				}
				else
				{
					result = dictionary[key];
				}
			}
			return result;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			bool result = false;
			lock (sync)
			{
				result = dictionary.TryGetValue(key, out value);
			}
			return result;
		}
	}
}
