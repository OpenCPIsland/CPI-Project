namespace ClubPenguin.Net
{
	public interface IDisneyStoreService : INetworkService
	{
		void PurchaseDisneyStoreItem(int itemId, int count);
	}
}
