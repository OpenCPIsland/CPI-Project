namespace ClubPenguin.Net.Domain
{
	public class PurchaseConsumableResponse : CPResponse
	{
		public PlayerAssets assets;

		public SignedResponse<ConsumableInventory> inventory;
	}
}
