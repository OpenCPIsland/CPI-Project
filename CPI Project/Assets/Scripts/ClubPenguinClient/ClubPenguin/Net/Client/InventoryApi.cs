using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class InventoryApi
	{
		private ClubPenguinClient clubPenguinClient;

		public InventoryApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<CreateCustomEquipmentOperation> CreateCustomEquipment(CustomEquipment equipment)
		{
			CreateCustomEquipmentOperation operation = new CreateCustomEquipmentOperation(equipment);
			return new APICall<CreateCustomEquipmentOperation>(clubPenguinClient, operation);
		}

		public APICall<GetInventoryOperation> GetInventory()
		{
			GetInventoryOperation operation = new GetInventoryOperation();
			return new APICall<GetInventoryOperation>(clubPenguinClient, operation);
		}

		public APICall<DeleteCustomEquipmentOperation> DeleteCustomEquipment(long equipmentId)
		{
			DeleteCustomEquipmentOperation operation = new DeleteCustomEquipmentOperation(equipmentId);
			return new APICall<DeleteCustomEquipmentOperation>(clubPenguinClient, operation);
		}
	}
}
