using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using Disney.Manimal.Common.Util;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpContentType("text/plain")]
	[HttpAccept("application/json")]
	[HttpPath("cp-api-base-uri", "/consumable/v1/qa/{$type}")]
	[HttpPOST]
	public class QASetTypeCountOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("type")]
		public string Type;

		[HttpRequestTextBody]
		public string Count;

		[HttpResponseJsonBody]
		public SignedResponse<ClubPenguin.Net.Domain.ConsumableInventory> SignedConsumableInventory;

		public QASetTypeCountOperation(string type, int count)
		{
			Type = type;
			Count = count.ToString();
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.ConsumableInventory value = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>();
			if (!value.Inventory.ContainsKey(Type))
			{
				value.Inventory[Type] = new InventoryItemStock();
			}
			value.Inventory[Type].itemCount = int.Parse(Count);
			value.Inventory[Type].lastPurchaseTimestamp = DateTime.UtcNow.GetTimeInMilliseconds();
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
