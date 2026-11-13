using System;

namespace ClubPenguin
{
	public struct CatalogCacheData
	{
		public int CacheTime;

		public DateTime Time;

		public object Data;

		public CatalogCacheData(int cacheTime, DateTime time, object obj)
		{
			CacheTime = cacheTime;
			Time = time;
			Data = obj;
		}
	}
}
