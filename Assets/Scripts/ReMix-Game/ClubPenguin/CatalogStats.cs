using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin
{
	public struct CatalogStats
	{
		public CatalogItemData StatsItem;

		public List<CatalogItemData> StatsData;

		public long TotalItemsPurchased;

		public long TotalItemsSold;

		public CatalogStats(CatalogItemData statsItem, List<CatalogItemData> statsData, long totalItemsPurchased, long totalItemsSold)
		{
			StatsItem = statsItem;
			StatsData = statsData;
			TotalItemsPurchased = totalItemsPurchased;
			TotalItemsSold = totalItemsSold;
		}
	}
}
