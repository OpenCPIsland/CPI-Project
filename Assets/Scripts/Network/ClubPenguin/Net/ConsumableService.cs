using ClubPenguin.Net.Client;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;

namespace ClubPenguin.Net
{
	internal class ConsumableService : BaseNetworkService, IConsumableService, INetworkService
	{
		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.CONSUMABLE_EQUIPPED, onConsumableEquipped);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.CONSUMABLE_USED, onConsumableUsed);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.CONSUMABLE_REUSE_FAILED, onConsumableReuseFailed);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.CONSUMABLE_MMO_DEPLOYED, onConsumableMMODeployed);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.COMSUMBLE_PARTIAL_COUNT_SET, onConsumablePartialCountSet);
		}

		public void GetMyInventory()
		{
			APICall<GetConsumableInventoryOperation> consumableInventory = clubPenguinClient.ConsumableApi.GetConsumableInventory();
			consumableInventory.OnResponse += onGetConsumableInventoryResponse;
			consumableInventory.OnError += handleCPResponseError;
			consumableInventory.Execute();
		}

		public void EquipItem(string type)
		{
			clubPenguinClient.GameServer.EquipConsumable(type);
		}

		public void PurchaseConsumable(string type, int count)
		{
			APICall<PurchaseConsumableOperation> aPICall = clubPenguinClient.ConsumableApi.PurchaseConsumable(type, count);
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += onPurchaseConsumableError;
			aPICall.OnResponse += onPurchaseConsumableResponse;
			aPICall.Execute();
		}

		public void ReuseConsumable(string type, object properties)
		{
			clubPenguinClient.GameServer.ReuseConsumable(type, properties);
		}

		public void UseConsumable(string type, object properties)
		{
			APICall<UseConsumableOperation> aPICall = clubPenguinClient.ConsumableApi.UseConsumable(type);
			aPICall.OnResponse += delegate(UseConsumableOperation op, HttpResponse httpResponse)
			{
				SignedResponse<UsedConsumable> signedUsedConsumable = op.SignedUsedConsumable;
				clubPenguinClient.GameServer.UseConsumable(signedUsedConsumable, properties);
			};
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		public void QA_SetTypeCount(string type, int count)
		{
			APICall<QASetTypeCountOperation> aPICall = clubPenguinClient.ConsumableApi.QA_SetTypeCount(type, count);
			aPICall.OnResponse += onQASetTypeCountResponse;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onPurchaseConsumableError(PurchaseConsumableOperation operation, HttpResponse httpResponse)
		{
			ErrorResponse errorResponse = Service.Get<JsonService>().Deserialize<ErrorResponse>(httpResponse.Text);
			if (errorResponse.code == 707)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(ConsumableServiceErrors.NotEnoughCoins));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(ConsumableServiceErrors.Unknown));
			}
		}

		private void onPurchaseConsumableResponse(PurchaseConsumableOperation operation, HttpResponse httpResponse)
		{
			PurchaseConsumableResponse response = operation.Response;
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyAssetsReceived(response.assets));
			inventoryDataReturned(response.inventory);
			handleCPResponse(response);
		}

		private void onGetConsumableInventoryResponse(GetConsumableInventoryOperation operation, HttpResponse httpResponse)
		{
			inventoryDataReturned(operation.SignedConsumableInventory);
		}

		private void onStorePartialConsumableComplete(StorePartialConsumableOperation operation, HttpResponse httpResponse)
		{
			inventoryDataReturned(operation.SignedConsumableInventory);
		}

		private void onQASetTypeCountResponse(QASetTypeCountOperation operation, HttpResponse httpResponse)
		{
			inventoryDataReturned(operation.SignedConsumableInventory);
		}

		private void inventoryDataReturned(SignedResponse<ConsumableInventory> data)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new ConsumableServiceEvents.InventoryRecieved(data.Data));
			clubPenguinClient.GameServer.SetConsumableInventory(data);
		}

		private void onConsumableEquipped(GameServerEvent gameServerEvent, object data)
		{
			HeldObjectEvent heldObjectEvent = (HeldObjectEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.ConsumableEquipped(heldObjectEvent.SessionId, heldObjectEvent.Type));
		}

		private void onConsumableUsed(GameServerEvent gameServerEvent, object data)
		{
			ConsumableEvent consumableEvent = (ConsumableEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new ConsumableServiceEvents.ConsumableUsed(consumableEvent.SessionId, consumableEvent.Type));
		}

		private void onConsumableReuseFailed(GameServerEvent gameServerEvent, object data)
		{
			ConsumableUseFailureEvent consumableUseFailureEvent = (ConsumableUseFailureEvent)data;
			UseConsumable(consumableUseFailureEvent.Type, consumableUseFailureEvent.Properties);
		}

		private void onConsumableMMODeployed(GameServerEvent gameServerEvent, object data)
		{
			ConsumableMMODeployedEvent consumableMMODeployedEvent = (ConsumableMMODeployedEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new ConsumableServiceEvents.ConsumableMMODeployed(consumableMMODeployedEvent.SessionId, consumableMMODeployedEvent.ExperienceId));
		}

		private void onConsumablePartialCountSet(GameServerEvent gameServerEvent, object data)
		{
			SignedResponse<UsedConsumable> partial = (SignedResponse<UsedConsumable>)data;
			APICall<StorePartialConsumableOperation> aPICall = clubPenguinClient.ConsumableApi.StorePartialConsumable(partial);
			aPICall.OnResponse += onStorePartialConsumableComplete;
			aPICall.Execute();
		}
	}
}
