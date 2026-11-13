namespace ClubPenguin.Net.Client
{
	public class DisneyStoreApi
	{
		private ClubPenguinClient clubPenguinClient;

		public DisneyStoreApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<PurchaseDisneyStoreItemOperation> PurchaseDisneyStoreItem(int itemId, int count)
		{
			PurchaseDisneyStoreItemOperation operation = new PurchaseDisneyStoreItemOperation(itemId, count);
			return new APICall<PurchaseDisneyStoreItemOperation>(clubPenguinClient, operation);
		}
	}
}
