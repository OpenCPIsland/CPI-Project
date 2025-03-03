using Disney.LaunchPadFramework;
using Foundation.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class ResourceCache<TKey, TValue> where TValue : UnityEngine.Object
	{
		private class Item
		{
			public readonly TValue Value;

			public readonly bool Permanent;

			public int RefCount;

			public Item(TValue value, bool permanent)
			{
				Value = value;
				Permanent = permanent;
				RefCount = 0;
			}

			public override string ToString()
			{
				return string.Format("[Item] Value='{0}', Permanent={1}, Refcount={2}", Value, Permanent, RefCount);
			}
		}

		public readonly Func<TKey, TValue, bool> ValueReleaser;

		private readonly Dictionary<TKey, Item> cache = new Dictionary<TKey, Item>();

		public int Count
		{
			get
			{
				return cache.Count;
			}
		}

		public ResourceCache()
		{
			ValueReleaser = ((TKey key, TValue value) => true);
		}

		public ResourceCache(Func<TKey, TValue, bool> valueReleaser)
		{
			ValueReleaser = valueReleaser;
		}

		public bool Contains(TKey key)
		{
			return cache.ContainsKey(key);
		}

		public bool InUse(TKey key)
		{
			Item value;
			if (cache.TryGetValue(key, out value))
			{
				return value.RefCount > 0;
			}
			return false;
		}

		public TValue AddGet(TKey key, TValue value, bool permanent = false)
		{
			Item item = new Item(value, permanent);
			cache.Add(key, item);
			item.RefCount++;
			return value;
		}

		public TValue Get(TKey key)
		{
			Item item = cache[key];
			item.RefCount++;
			return item.Value;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			Item value2;
			if (cache.TryGetValue(key, out value2))
			{
				value = value2.Value;
				if ((UnityEngine.Object)value == (UnityEngine.Object)null)
				{
					Log.LogErrorFormatted(this, "Value existed in the cache but was null. Key {0}", key);
				}
				value2.RefCount++;
				return true;
			}
			value = null;
			return false;
		}

		public void Release(TKey key)
		{
			Item item = cache[key];
			item.RefCount--;
			if (!item.Permanent && item.RefCount <= 0 && ValueReleaser(key, item.Value))
			{
				ComponentExtensions.DestroyResource(item.Value);
				cache.Remove(key);
			}
		}

		public void ReleaseAll()
		{
			List<TKey> list = new List<TKey>();
			foreach (KeyValuePair<TKey, Item> item in cache)
			{
				if (!item.Value.Permanent && item.Value.RefCount <= 0 && ValueReleaser(item.Key, item.Value.Value))
				{
					ComponentExtensions.DestroyResource(item.Value.Value);
					list.Add(item.Key);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				cache.Remove(list[i]);
			}
		}
	}
}
