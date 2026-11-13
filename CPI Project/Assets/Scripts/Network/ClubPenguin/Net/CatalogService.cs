using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;

namespace ClubPenguin.Net
{
	public class CatalogService : BaseNetworkService, ICatalogService, INetworkService
	{
		public void GetCatalogItemsByCategory(string category, long scheduledThemeChallengeId, string cursor = "")
		{
			APICall<GetCatalogCategoryOperation> catalogCategory = clubPenguinClient.CatalogApi.GetCatalogCategory(category, scheduledThemeChallengeId, cursor);
			catalogCategory.OnResponse += onCatalogItemsByCategoryResponse;
			catalogCategory.OnError += onCatalogItemsByCategoryError;
			catalogCategory.Execute();
		}

		public void GetCatalogItemsByFriends(long scheduledThemeChallengeId, string cursor = "")
		{
			APICall<GetCatalogFriendsOperation> catalogFriends = clubPenguinClient.CatalogApi.GetCatalogFriends(scheduledThemeChallengeId, cursor);
			catalogFriends.OnResponse += onCatalogItemsByFriendsResponse;
			catalogFriends.OnError += onCatalogItemsByFriendsError;
			catalogFriends.Execute();
		}

		public void GetCatalogItemsByPopularity(long scheduledThemeChallengeId, string cursor = "")
		{
			APICall<GetCatalogPopularOperation> catalogPopular = clubPenguinClient.CatalogApi.GetCatalogPopular(scheduledThemeChallengeId, cursor);
			catalogPopular.OnResponse += onCatalogItemsByPopularityResponse;
			catalogPopular.OnError += onCatalogItemsByPopularityError;
			catalogPopular.Execute();
		}

		public void GetCatalogItemsByRecent(long scheduledThemeChallengeId, string cursor = "")
		{
			APICall<GetCatalogRecentOperation> catalogRecent = clubPenguinClient.CatalogApi.GetCatalogRecent(scheduledThemeChallengeId, cursor);
			catalogRecent.OnResponse += onCatalogItemsByRecentResponse;
			catalogRecent.OnError += onCatalogItemsByRecentError;
			catalogRecent.Execute();
		}

		public void GetCurrentThemes()
		{
			APICall<GetCurrentThemeOperation> currentTheme = clubPenguinClient.CatalogApi.GetCurrentTheme();
			currentTheme.OnResponse += onCurrentThemesResponse;
			currentTheme.OnError += onCurrentThemesError;
			currentTheme.Execute();
		}

		public void GetPlayerCatalogStats()
		{
			APICall<GetUserStatsOperation> userStats = clubPenguinClient.CatalogApi.GetUserStats();
			userStats.OnResponse += onPlayerCatalogStatsResponse;
			userStats.OnError += onPlayerCatalogStatsError;
			userStats.Execute();
		}

		public void PurchaseCatalogItem(long clothingCatalogItemId)
		{
			APICall<ItemPurchaseOperation> aPICall = clubPenguinClient.CatalogApi.ItemPurchase(clothingCatalogItemId);
			aPICall.OnResponse += onPurchaseItemResponse;
			aPICall.OnError += onPurchaseItemError;
			aPICall.Execute();
		}

		public void SubmitCatalogThemeItem(long scheduledThemeChallengeId, CustomEquipment equipment)
		{
			APICall<ItemSubmissionOperation> aPICall = clubPenguinClient.CatalogApi.ItemSubmission(scheduledThemeChallengeId, equipment);
			aPICall.OnResponse += onSubmitItemResponse;
			aPICall.OnError += onSubmitItemError;
			aPICall.Execute();
		}

		protected override void setupListeners()
		{
		}

		private void onGetItemIds(object sender, string[] itemIds)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceEvents.ItemListResponse(itemIds));
		}

		private void onPurchaseItemById(object sender, bool success, string itemId)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceEvents.ItemPurchased(success, itemId));
		}

		private void onCatalogItemsByCategoryResponse(GetCatalogCategoryOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.CatalogItemsByCategoryRetrievedEvent evt = new CatalogServiceEvents.CatalogItemsByCategoryRetrievedEvent(operation.Response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onCatalogItemsByFriendsResponse(GetCatalogFriendsOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.CatalogItemsByFriendsRetrievedEvent evt = new CatalogServiceEvents.CatalogItemsByFriendsRetrievedEvent(operation.Response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onCatalogItemsByPopularityResponse(GetCatalogPopularOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.CatalogItemsByPopularityRetrievedEvent evt = new CatalogServiceEvents.CatalogItemsByPopularityRetrievedEvent(operation.Response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onCatalogItemsByRecentResponse(GetCatalogRecentOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.CatalogItemsByRecentRetrievedEvent evt = new CatalogServiceEvents.CatalogItemsByRecentRetrievedEvent(operation.Response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onCurrentThemesResponse(GetCurrentThemeOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.CurrentThemesRetrievedEvent evt = new CatalogServiceEvents.CurrentThemesRetrievedEvent(operation.Response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onPlayerCatalogStatsResponse(GetUserStatsOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.UserStatsRetrievedEvent evt = new CatalogServiceEvents.UserStatsRetrievedEvent(operation.Response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onPurchaseItemResponse(ItemPurchaseOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.ItemPurchaseCompleteEvent evt = new CatalogServiceEvents.ItemPurchaseCompleteEvent(operation.Response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onSubmitItemResponse(ItemSubmissionOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.ItemSubmissionCompleteEvent evt = new CatalogServiceEvents.ItemSubmissionCompleteEvent(operation.Response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
			handleCPResponse(operation.Response);
		}

		private void onCatalogItemsByCategoryError(GetCatalogCategoryOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.CatalogItemsByCategoryErrorEvent evt = new CatalogServiceEvents.CatalogItemsByCategoryErrorEvent(response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onCatalogItemsByFriendsError(GetCatalogFriendsOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.CatalogItemsByFriendsErrorEvent evt = new CatalogServiceEvents.CatalogItemsByFriendsErrorEvent(response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onCatalogItemsByPopularityError(GetCatalogPopularOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.CatalogItemsByPopularityErrorEvent evt = new CatalogServiceEvents.CatalogItemsByPopularityErrorEvent(response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onCatalogItemsByRecentError(GetCatalogRecentOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.CatalogItemsByRecentErrorEvent evt = new CatalogServiceEvents.CatalogItemsByRecentErrorEvent(response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onCurrentThemesError(GetCurrentThemeOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.CurrentThemesErrorEvent evt = new CatalogServiceEvents.CurrentThemesErrorEvent(response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onPlayerCatalogStatsError(GetUserStatsOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.UserStatsErrorEvent evt = new CatalogServiceEvents.UserStatsErrorEvent(response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onPurchaseItemError(ItemPurchaseOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.ItemPurchaseErrorEvent evt = new CatalogServiceEvents.ItemPurchaseErrorEvent(response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void onSubmitItemError(ItemSubmissionOperation operation, HttpResponse response)
		{
			CatalogServiceEvents.ItemSubmissionErrorEvent evt = new CatalogServiceEvents.ItemSubmissionErrorEvent(response);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}
	}
}
