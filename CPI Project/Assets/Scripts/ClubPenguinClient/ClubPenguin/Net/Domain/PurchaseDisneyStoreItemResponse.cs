using ClubPenguin.Net.Domain.Decoration;

namespace ClubPenguin.Net.Domain
{
	public class PurchaseDisneyStoreItemResponse : CPResponse
	{
		public PlayerAssets assets;

		public SignedResponse<ConsumableInventory> inventory;

		public SignedResponse<DecorationInventory> decorationInventory;
	}
}
