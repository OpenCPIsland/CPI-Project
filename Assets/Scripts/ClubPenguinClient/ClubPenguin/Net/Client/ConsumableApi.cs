using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class ConsumableApi
	{
		private ClubPenguinClient clubPenguinClient;

		public ConsumableApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<GetConsumableInventoryOperation> GetConsumableInventory()
		{
			GetConsumableInventoryOperation operation = new GetConsumableInventoryOperation();
			return new APICall<GetConsumableInventoryOperation>(clubPenguinClient, operation);
		}

		public APICall<PurchaseConsumableOperation> PurchaseConsumable(string type, int count)
		{
			PurchaseConsumableOperation operation = new PurchaseConsumableOperation(type, count);
			return new APICall<PurchaseConsumableOperation>(clubPenguinClient, operation);
		}

		public APICall<StorePartialConsumableOperation> StorePartialConsumable(SignedResponse<UsedConsumable> partial)
		{
			StorePartialConsumableOperation operation = new StorePartialConsumableOperation(partial);
			return new APICall<StorePartialConsumableOperation>(clubPenguinClient, operation);
		}

		public APICall<UseConsumableOperation> UseConsumable(string type)
		{
			UseConsumableOperation operation = new UseConsumableOperation(type);
			return new APICall<UseConsumableOperation>(clubPenguinClient, operation);
		}

		public APICall<QASetTypeCountOperation> QA_SetTypeCount(string type, int count)
		{
			QASetTypeCountOperation operation = new QASetTypeCountOperation(type, count);
			return new APICall<QASetTypeCountOperation>(clubPenguinClient, operation);
		}
	}
}
