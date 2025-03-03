using Disney.LaunchPadFramework;
using LitJson;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public abstract class AbstractDictionaryReward<TKey, TValue> : IRewardable
	{
		protected Dictionary<TKey, TValue> data;

		public abstract string RewardType
		{
			get;
		}

		public object Reward
		{
			get
			{
				return data;
			}
		}

		protected abstract TValue combineValues(TValue val1, TValue val2);

		public AbstractDictionaryReward()
		{
			data = new Dictionary<TKey, TValue>();
		}

		public AbstractDictionaryReward(TKey key, TValue value)
			: this()
		{
			if (data.ContainsKey(key))
			{
				data[key] = combineValues(data[key], value);
			}
			else
			{
				data.Add(key, value);
			}
		}

		public void FromJson(JsonData jsonData)
		{
			if (typeof(TKey) == typeof(string))
			{
				data = JsonMapper.ToObject<Dictionary<TKey, TValue>>(jsonData.ToJson());
				return;
			}
			Dictionary<string, TValue> dictionary = JsonMapper.ToObject<Dictionary<string, TValue>>(jsonData.ToJson());
			data = new Dictionary<TKey, TValue>();
			foreach (KeyValuePair<string, TValue> item in dictionary)
			{
				try
				{
					data.Add((TKey)Convert.ChangeType(item.Key, typeof(TKey)), item.Value);
				}
				catch (Exception ex)
				{
					Log.LogErrorFormatted(this, "Failed to convert reward key from string to {0}, {1}.", typeof(TKey), ex);
				}
			}
		}

		public void Add(IRewardable reward)
		{
			data = combineMap(data, ((AbstractDictionaryReward<TKey, TValue>)reward).data, combineValues);
		}

		public bool IsEmpty()
		{
			return data == null || data.Count == 0;
		}

		private static Dictionary<K, V> combineMap<K, V>(Dictionary<K, V> map1, Dictionary<K, V> map2, Func<V, V, V> operation)
		{
			if (map1 == null && map2 == null)
			{
				return null;
			}
			if (map1 == null)
			{
				return new Dictionary<K, V>(map2);
			}
			if (map2 == null)
			{
				return new Dictionary<K, V>(map1);
			}
			Dictionary<K, V> dictionary = new Dictionary<K, V>(map1);
			foreach (KeyValuePair<K, V> item in map2)
			{
				if (dictionary.ContainsKey(item.Key))
				{
					dictionary[item.Key] = operation(dictionary[item.Key], item.Value);
				}
				else
				{
					dictionary.Add(item.Key, item.Value);
				}
			}
			return dictionary;
		}
	}
}
