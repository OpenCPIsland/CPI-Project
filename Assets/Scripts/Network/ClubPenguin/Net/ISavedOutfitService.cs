using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public interface ISavedOutfitService : INetworkService
	{
		void SaveSavedOutfit(SavedOutfit savedOutfit);

		void GetSavedOutfits();

		void DeleteSavedOutfit(int outfitSlot);
	}
}
