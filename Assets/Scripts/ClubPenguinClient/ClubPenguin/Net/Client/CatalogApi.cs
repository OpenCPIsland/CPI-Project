using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class CatalogApi
	{
		private ClubPenguinClient clubPenguinClient;

		public CatalogApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<GetCatalogCategoryOperation> GetCatalogCategory(string equipmentCategory, long scheduledThemeChallengeId, string cursor = "")
		{
			CatalogCategoryRequest categoryRequest = default(CatalogCategoryRequest);
			categoryRequest.scheduledThemeChallengeId = scheduledThemeChallengeId;
			categoryRequest.equipmentCategory = equipmentCategory;
			categoryRequest.cursor = cursor;
			GetCatalogCategoryOperation operation = new GetCatalogCategoryOperation(categoryRequest);
			return new APICall<GetCatalogCategoryOperation>(clubPenguinClient, operation);
		}

		public APICall<GetCatalogFriendsOperation> GetCatalogFriends(long scheduledThemeChallengeId, string cursor = "")
		{
			CatalogSectionRequest sectionRequest = default(CatalogSectionRequest);
			sectionRequest.scheduledThemeChallengeId = scheduledThemeChallengeId;
			sectionRequest.cursor = cursor;
			GetCatalogFriendsOperation operation = new GetCatalogFriendsOperation(sectionRequest);
			return new APICall<GetCatalogFriendsOperation>(clubPenguinClient, operation);
		}

		public APICall<GetCatalogPopularOperation> GetCatalogPopular(long scheduledThemeChallengeId, string cursor = "")
		{
			CatalogSectionRequest sectionRequest = default(CatalogSectionRequest);
			sectionRequest.scheduledThemeChallengeId = scheduledThemeChallengeId;
			sectionRequest.cursor = cursor;
			GetCatalogPopularOperation operation = new GetCatalogPopularOperation(sectionRequest);
			return new APICall<GetCatalogPopularOperation>(clubPenguinClient, operation);
		}

		public APICall<GetCatalogRecentOperation> GetCatalogRecent(long scheduledThemeChallengeId, string cursor = "")
		{
			CatalogSectionRequest sectionRequest = default(CatalogSectionRequest);
			sectionRequest.scheduledThemeChallengeId = scheduledThemeChallengeId;
			sectionRequest.cursor = cursor;
			GetCatalogRecentOperation operation = new GetCatalogRecentOperation(sectionRequest);
			return new APICall<GetCatalogRecentOperation>(clubPenguinClient, operation);
		}

		public APICall<GetCurrentThemeOperation> GetCurrentTheme()
		{
			GetCurrentThemeOperation operation = new GetCurrentThemeOperation();
			return new APICall<GetCurrentThemeOperation>(clubPenguinClient, operation);
		}

		public APICall<GetUserStatsOperation> GetUserStats()
		{
			GetUserStatsOperation operation = new GetUserStatsOperation();
			return new APICall<GetUserStatsOperation>(clubPenguinClient, operation);
		}

		public APICall<ItemPurchaseOperation> ItemPurchase(long clothingCatalogItemId)
		{
			ItemPurchaseOperation operation = new ItemPurchaseOperation(clothingCatalogItemId);
			return new APICall<ItemPurchaseOperation>(clubPenguinClient, operation);
		}

		public APICall<ItemSubmissionOperation> ItemSubmission(long scheduledThemeChallengeId, CustomEquipment equipment)
		{
			ItemSubmissionOperation operation = new ItemSubmissionOperation(scheduledThemeChallengeId, equipment);
			return new APICall<ItemSubmissionOperation>(clubPenguinClient, operation);
		}
	}
}
