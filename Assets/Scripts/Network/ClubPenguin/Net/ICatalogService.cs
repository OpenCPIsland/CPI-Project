using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public interface ICatalogService : INetworkService
	{
		void GetCatalogItemsByCategory(string category, long scheduledThemeChallengeId, string cursor = "");

		void GetCatalogItemsByFriends(long scheduledThemeChallengeId, string cursor = "");

		void GetCatalogItemsByPopularity(long scheduledThemeChallengeId, string cursor = "");

		void GetCatalogItemsByRecent(long scheduledThemeChallengeId, string cursor = "");

		void GetCurrentThemes();

		void GetPlayerCatalogStats();

		void PurchaseCatalogItem(long clothingCatalogItemId);

		void SubmitCatalogThemeItem(long scheduledThemeChallengeId, CustomEquipment equipment);
	}
}
