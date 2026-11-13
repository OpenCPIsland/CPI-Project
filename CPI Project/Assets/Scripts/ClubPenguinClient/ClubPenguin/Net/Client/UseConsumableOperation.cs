using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpDELETE]
	[HttpPath("cp-api-base-uri", "/consumable/v1/{$type}")]
	public class UseConsumableOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("type")]
		public string Type;

		[HttpResponseJsonBody]
		public SignedResponse<UsedConsumable> SignedUsedConsumable;

		public UseConsumableOperation(string type)
		{
			Type = type;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			int partialCount = updateInventory(offlineDatabase);
			SignedUsedConsumable = new SignedResponse<UsedConsumable>
			{
				Data = new UsedConsumable
				{
					partialCount = partialCount,
					type = Type
				}
			};
		}

		private int updateInventory(OfflineDatabase offlineDatabase)
		{
			ClubPenguin.Net.Offline.ConsumableInventory value = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>();
			if (!value.Inventory.ContainsKey(Type))
			{
				value.Inventory[Type] = new InventoryItemStock();
			}
			int partialCount = value.Inventory[Type].partialCount;
			if (partialCount > 0)
			{
				value.Inventory[Type].partialCount = 0;
			}
			else
			{
				value.Inventory[Type].itemCount--;
				if (value.Inventory[Type].itemCount <= 0)
				{
					value.Inventory.Remove(Type);
				}
			}
			offlineDatabase.Write(value);
			return partialCount;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			updateInventory(offlineDatabase);
		}
	}
}
