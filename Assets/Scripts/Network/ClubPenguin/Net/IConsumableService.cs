namespace ClubPenguin.Net
{
	public interface IConsumableService : INetworkService
	{
		void GetMyInventory();

		void EquipItem(string type);

		void PurchaseConsumable(string type, int count);

		void UseConsumable(string type, object properties);

		void ReuseConsumable(string type, object properties);

		void QA_SetTypeCount(string type, int count);
	}
}
