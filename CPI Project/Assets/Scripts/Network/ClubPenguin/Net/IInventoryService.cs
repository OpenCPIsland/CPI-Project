using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public interface IInventoryService : INetworkService
	{
		void CreateCustomEquipment(CustomEquipment equipmentRequest);

		void GetEquipmentInventory();

		void DeleteCustomEquipment(long equipmentId);
	}
}
