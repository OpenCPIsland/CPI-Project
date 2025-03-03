using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	public struct UserStatsResponse
	{
		public long totalItemsSold;

		public long totalItemsPurchased;

		public CatalogItemData mostPopularItem;

		public List<CatalogItemData> currentItems;
	}
}
