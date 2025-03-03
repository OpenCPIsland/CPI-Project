using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;

namespace ClubPenguin.Net
{
	public class DisneyStoreService : BaseNetworkService, IDisneyStoreService, INetworkService
	{
		protected override void setupListeners()
		{
		}

		public void PurchaseDisneyStoreItem(int itemId, int count)
		{
			APICall<PurchaseDisneyStoreItemOperation> aPICall = clubPenguinClient.DisneyStoreApi.PurchaseDisneyStoreItem(itemId, count);
			aPICall.OnResponse += onPurchaseDisneyStoreItemReceived;
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += onPurchaseDisneyStoreItemError;
			aPICall.Execute();
		}

		private void onPurchaseDisneyStoreItemReceived(PurchaseDisneyStoreItemOperation operation, HttpResponse httpResponse)
		{
			awardDisneyStoreItem(operation.Response);
			handleCPResponse(operation.Response);
			Service.Get<EventDispatcher>().DispatchEvent(new DisneyStoreServiceEvents.DisneyStorePurchaseComplete(DisneyStoreServiceEvents.DisneyStorePurchaseResult.Success));
		}

		private void onPurchaseDisneyStoreItemError(CPAPIHttpOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new DisneyStoreServiceEvents.DisneyStorePurchaseComplete(DisneyStoreServiceEvents.DisneyStorePurchaseResult.Error));
		}

		private void awardDisneyStoreItem(PurchaseDisneyStoreItemResponse response)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyAssetsReceived(response.assets));
			Service.Get<EventDispatcher>().DispatchEvent(new ConsumableServiceEvents.InventoryRecieved(response.inventory.Data));
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.DecorationsLoaded(response.decorationInventory.Data));
			clubPenguinClient.GameServer.SetConsumableInventory(response.inventory);
		}
	}
}
