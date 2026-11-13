using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class SavedOutfitApi
	{
		private ClubPenguinClient clubPenguinClient;

		public SavedOutfitApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<SaveSavedOutfitOperation> SaveSavedOutfit(SavedOutfit savedOutfit)
		{
			SaveSavedOutfitOperation operation = new SaveSavedOutfitOperation(savedOutfit);
			return new APICall<SaveSavedOutfitOperation>(clubPenguinClient, operation);
		}

		public APICall<GetSavedOutfitsOperation> GetSavedOutfits()
		{
			GetSavedOutfitsOperation operation = new GetSavedOutfitsOperation();
			return new APICall<GetSavedOutfitsOperation>(clubPenguinClient, operation);
		}

		public APICall<DeleteSavedOutfitOperation> DeleteSavedOutfit(int outfitSlot)
		{
			DeleteSavedOutfitOperation operation = new DeleteSavedOutfitOperation(outfitSlot);
			return new APICall<DeleteSavedOutfitOperation>(clubPenguinClient, operation);
		}
	}
}
