using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class CatalogServiceProxyEvents
	{
		public struct ChallengesReponse
		{
			public List<CurrentThemeData> Themes;

			public ChallengesReponse(List<CurrentThemeData> themes)
			{
				Themes = themes;
			}
		}

		public struct StatsReponse
		{
			public CatalogStats Stats;

			public StatsReponse(CatalogStats stats)
			{
				Stats = stats;
			}
		}

		public struct ShopItemsReponse
		{
			public List<CatalogItemData> Items;

			public ShopItemsReponse(List<CatalogItemData> items)
			{
				Items = items;
			}
		}

		public struct OnCatalogItemsByCategory
		{
			public string CacheId;

			public CatalogServiceEvents.CatalogItemsByCategoryRetrievedEvent Request;

			public OnCatalogItemsByCategory(string cacheId, CatalogServiceEvents.CatalogItemsByCategoryRetrievedEvent request)
			{
				CacheId = cacheId;
				Request = request;
			}
		}

		public struct OnCatalogItemsByFriend
		{
			public string CacheId;

			public CatalogServiceEvents.CatalogItemsByFriendsRetrievedEvent Request;

			public OnCatalogItemsByFriend(string cacheId, CatalogServiceEvents.CatalogItemsByFriendsRetrievedEvent request)
			{
				CacheId = cacheId;
				Request = request;
			}
		}

		public struct OnCatalogItemsByPopular
		{
			public string CacheId;

			public CatalogServiceEvents.CatalogItemsByPopularityRetrievedEvent Request;

			public OnCatalogItemsByPopular(string cacheId, CatalogServiceEvents.CatalogItemsByPopularityRetrievedEvent request)
			{
				CacheId = cacheId;
				Request = request;
			}
		}

		public struct OnCatalogItemsByRecent
		{
			public string CacheId;

			public CatalogServiceEvents.CatalogItemsByRecentRetrievedEvent Request;

			public OnCatalogItemsByRecent(string cacheId, CatalogServiceEvents.CatalogItemsByRecentRetrievedEvent request)
			{
				CacheId = cacheId;
				Request = request;
			}
		}
	}
}
