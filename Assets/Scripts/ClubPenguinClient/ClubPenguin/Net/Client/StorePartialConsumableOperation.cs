using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[HttpAccept("application/json")]
	[HttpContentType("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/consumable/v1/partial")]
	public class StorePartialConsumableOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public SignedResponse<UsedConsumable> Partial;

		[HttpResponseJsonBody]
		public SignedResponse<ClubPenguin.Net.Domain.ConsumableInventory> SignedConsumableInventory;

		public StorePartialConsumableOperation(SignedResponse<UsedConsumable> partial)
		{
			Partial = partial;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.ConsumableInventory value = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>();
			if (!value.Inventory.ContainsKey(Partial.Data.type))
			{
				value.Inventory[Partial.Data.type] = new InventoryItemStock();
			}
			value.Inventory[Partial.Data.type].partialCount = Partial.Data.partialCount;
			offlineDatabase.Write(value);
			SignedConsumableInventory = new SignedResponse<ClubPenguin.Net.Domain.ConsumableInventory>
			{
				Data = new ClubPenguin.Net.Domain.ConsumableInventory
				{
					inventoryMap = value.Inventory
				}
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.ConsumableInventory value = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>();
			value.Inventory = SignedConsumableInventory.Data.inventoryMap;
			offlineDatabase.Write(value);
		}
	}
}
