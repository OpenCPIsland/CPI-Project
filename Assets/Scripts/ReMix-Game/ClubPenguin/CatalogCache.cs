using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class CatalogCache
	{
		public const string CATALOG_STATS_ID = "catalogStatsCacheId";

		public const string CATALOG_THEME_ID = "CatalogThemeCacheId";

		public int defaultCacheTime = 5;

		public int MaxCachedItems = 20;

		private Dictionary<string, CatalogCacheData> cache = new Dictionary<string, CatalogCacheData>();

		private List<string> cacheIndices = new List<string>();

		public void DisableCache()
		{
			ClearCache();
			MaxCachedItems = 1;
		}

		public void EnableCache()
		{
			MaxCachedItems = 20;
		}

		public CatalogCache()
		{
			cache = new Dictionary<string, CatalogCacheData>();
		}

		public void ClearCache()
		{
			cache = new Dictionary<string, CatalogCacheData>();
			cacheIndices = new List<string>();
		}

		public void RemoveCacheItemById(string id)
		{
			if (cache.ContainsKey(id))
			{
				cache.Remove(id);
				int index = cacheIndices.IndexOf(id);
				cacheIndices.RemoveAt(index);
			}
		}

		public void RemoveOldestItem()
		{
			while (cacheIndices.Count > MaxCachedItems)
			{
				string id = cacheIndices[0];
				RemoveCacheItemById(id);
			}
		}

		public void MoveCacheItemToFront(string id)
		{
			int index = cacheIndices.IndexOf(id);
			cacheIndices.RemoveAt(index);
			cacheIndices.Add(id);
		}

		public CatalogCacheData GetCacheDataById(string id)
		{
			if (!cache.ContainsKey(id))
			{
				cache.Add(id, new CatalogCacheData(defaultCacheTime, DateTime.Now, null));
				cacheIndices.Add(id);
			}
			else
			{
				MoveCacheItemToFront(id);
			}
			RemoveOldestItem();
			return cache[id];
		}

		public void SetCatalogCacheData(string id, CatalogCacheData data)
		{
			cache[id] = data;
		}

		public bool isShouldUseCache(string id)
		{
			bool result = false;
			if (cache.ContainsKey(id))
			{
				CatalogCacheData catalogCacheData = cache[id];
				if ((DateTime.Now - catalogCacheData.Time).Seconds > catalogCacheData.CacheTime)
				{
					RemoveCacheItemById(id);
				}
				else
				{
					MoveCacheItemToFront(id);
					result = true;
				}
			}
			return result;
		}
	}
}
