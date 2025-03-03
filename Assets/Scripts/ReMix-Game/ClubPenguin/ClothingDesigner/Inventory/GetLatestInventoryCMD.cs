using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class GetLatestInventoryCMD
	{
		public delegate void CallbackDelegate();

		private CallbackDelegate callback;

		public GetLatestInventoryCMD(CallbackDelegate callback)
		{
			this.callback = callback;
		}

		public GetLatestInventoryCMD()
		{
		}

		public void Execute()
		{
			requestInventory();
		}

		private void requestInventory()
		{
			DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			if (!localPlayerHandle.IsNull)
			{
				Service.Get<CPDataEntityCollection>().RemoveComponent<InventoryData>(localPlayerHandle);
				Service.Get<EventDispatcher>().AddListener<InventoryServiceEvents.InventoryLoaded>(onInventoryRetrieved);
				Service.Get<INetworkServicesManager>().InventoryService.GetEquipmentInventory();
			}
		}

		private bool onInventoryRetrieved(InventoryServiceEvents.InventoryLoaded evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<InventoryServiceEvents.InventoryLoaded>(onInventoryRetrieved);
			DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			if (!localPlayerHandle.IsNull)
			{
				InventoryData inventoryData = Service.Get<CPDataEntityCollection>().AddComponent<InventoryData>(localPlayerHandle);
				inventoryData.Inventory = new Dictionary<long, InventoryIconModel<DCustomEquipment>>();
				inventoryData.CurrentAvatarEquipment = new List<long>();
				for (int i = 0; i < evt.Inventory.Count; i++)
				{
					try
					{
						DCustomEquipment data = CustomEquipmentResponseAdaptor.ConvertResponseToCustomEquipment(evt.Inventory[i]);
						InventoryIconModel<DCustomEquipment> value = new InventoryIconModel<DCustomEquipment>(data.Id, data, false, true);
						inventoryData.Inventory.Add(data.Id, value);
					}
					catch (KeyNotFoundException)
					{
					}
				}
			}
			else
			{
				Log.LogError(this, "Unable to find the LocalPlayerHandle.");
			}
			if (callback != null)
			{
				callback();
				callback = null;
			}
			return false;
		}
	}
}
