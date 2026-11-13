using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.http;

namespace ClubPenguin.Net
{
	public class CatalogServiceEvents
	{
		public struct ItemListResponse
		{
			public readonly string[] ItemIds;

			public ItemListResponse(string[] itemIds)
			{
				ItemIds = itemIds;
			}
		}

		public struct ItemPurchased
		{
			public readonly bool Success;

			public readonly string ItemId;

			public ItemPurchased(bool success, string itemId)
			{
				Success = success;
				ItemId = itemId;
			}
		}

		public struct CatalogItemsByCategoryRetrievedEvent
		{
			public readonly CatalogSectionResponse Response;

			public CatalogItemsByCategoryRetrievedEvent(CatalogSectionResponse response)
			{
				Response = response;
			}
		}

		public struct CatalogItemsByFriendsRetrievedEvent
		{
			public readonly CatalogSectionResponse Response;

			public CatalogItemsByFriendsRetrievedEvent(CatalogSectionResponse response)
			{
				Response = response;
			}
		}

		public struct CatalogItemsByPopularityRetrievedEvent
		{
			public readonly CatalogSectionResponse Response;

			public CatalogItemsByPopularityRetrievedEvent(CatalogSectionResponse response)
			{
				Response = response;
			}
		}

		public struct CatalogItemsByRecentRetrievedEvent
		{
			public readonly CatalogSectionResponse Response;

			public CatalogItemsByRecentRetrievedEvent(CatalogSectionResponse response)
			{
				Response = response;
			}
		}

		public struct CurrentThemesRetrievedEvent
		{
			public readonly CurrentThemeResponse Response;

			public CurrentThemesRetrievedEvent(CurrentThemeResponse response)
			{
				Response = response;
			}
		}

		public struct UserStatsRetrievedEvent
		{
			public readonly UserStatsResponse Response;

			public UserStatsRetrievedEvent(UserStatsResponse response)
			{
				Response = response;
			}
		}

		public struct ItemPurchaseCompleteEvent
		{
			public readonly ItemPurchaseResponse Response;

			public ItemPurchaseCompleteEvent(ItemPurchaseResponse response)
			{
				Response = response;
			}
		}

		public struct ItemSubmissionCompleteEvent
		{
			public readonly ItemSubmissionResponse Response;

			public ItemSubmissionCompleteEvent(ItemSubmissionResponse response)
			{
				Response = response;
			}
		}

		public struct CatalogItemsByCategoryErrorEvent
		{
			public readonly HttpResponse Response;

			public CatalogItemsByCategoryErrorEvent(HttpResponse response)
			{
				Response = response;
			}
		}

		public struct CatalogItemsByFriendsErrorEvent
		{
			public readonly HttpResponse Response;

			public CatalogItemsByFriendsErrorEvent(HttpResponse response)
			{
				Response = response;
			}
		}

		public struct CatalogItemsByPopularityErrorEvent
		{
			public readonly HttpResponse Response;

			public CatalogItemsByPopularityErrorEvent(HttpResponse response)
			{
				Response = response;
			}
		}

		public struct CatalogItemsByRecentErrorEvent
		{
			public readonly HttpResponse Response;

			public CatalogItemsByRecentErrorEvent(HttpResponse response)
			{
				Response = response;
			}
		}

		public struct CurrentThemesErrorEvent
		{
			public readonly HttpResponse Response;

			public CurrentThemesErrorEvent(HttpResponse response)
			{
				Response = response;
			}
		}

		public struct UserStatsErrorEvent
		{
			public readonly HttpResponse Response;

			public UserStatsErrorEvent(HttpResponse response)
			{
				Response = response;
			}
		}

		public struct ItemPurchaseErrorEvent
		{
			public readonly HttpResponse Response;

			public ItemPurchaseErrorEvent(HttpResponse response)
			{
				Response = response;
			}
		}

		public struct ItemSubmissionErrorEvent
		{
			public readonly HttpResponse Response;

			public ItemSubmissionErrorEvent(HttpResponse response)
			{
				Response = response;
			}
		}
	}
}
