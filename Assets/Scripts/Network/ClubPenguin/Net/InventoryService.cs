using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	internal class InventoryService : BaseNetworkService, IInventoryService, INetworkService
	{
		protected override void setupListeners()
		{
		}

		public void CreateCustomEquipment(CustomEquipment equipmentRequest)
		{
			APICall<CreateCustomEquipmentOperation> aPICall = clubPenguinClient.InventoryApi.CreateCustomEquipment(equipmentRequest);
			aPICall.OnResponse += equipmentCreated;
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += onEquipmentCreateError;
			aPICall.Execute();
		}

		public void GetEquipmentInventory()
		{
			APICall<GetInventoryOperation> inventory = clubPenguinClient.InventoryApi.GetInventory();
			inventory.OnResponse += inventoryLoaded;
			inventory.OnError += handleCPResponseError;
			inventory.Execute();
		}

		public void DeleteCustomEquipment(long equipmentId)
		{
			APICall<DeleteCustomEquipmentOperation> aPICall = clubPenguinClient.InventoryApi.DeleteCustomEquipment(equipmentId);
			aPICall.OnResponse += equipmentDeleted;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void equipmentCreated(CreateCustomEquipmentOperation operation, HttpResponse httpResponse)
		{
			CreateEquipmentResponse customEquipmentResponse = operation.CustomEquipmentResponse;
			Service.Get<EventDispatcher>().DispatchEvent(new InventoryServiceEvents.EquipmentCreated(customEquipmentResponse.equipmentId));
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyAssetsReceived(customEquipmentResponse.assets));
			handleCPResponse(customEquipmentResponse);
		}

		private void onEquipmentCreateError(CreateCustomEquipmentOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(InventoryServiceErrors.EquipmentCreationError));
		}

		private void inventoryLoaded(GetInventoryOperation operation, HttpResponse httpResponse)
		{
			List<CustomEquipment> customEquipmentResponses = operation.CustomEquipmentResponses;
			Service.Get<EventDispatcher>().DispatchEvent(new InventoryServiceEvents.InventoryLoaded(customEquipmentResponses));
		}

		private void equipmentDeleted(DeleteCustomEquipmentOperation operation, HttpResponse httpResponse)
		{
			CustomEquipmentId customEquipmentId = operation.CustomEquipmentId;
			Service.Get<EventDispatcher>().DispatchEvent(new InventoryServiceEvents.EquipmentDeleted(customEquipmentId.equipmentId));
			handleCPResponse(customEquipmentId);
		}
	}
}
