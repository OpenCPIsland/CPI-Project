using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using Disney.Manimal.Common.Util;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpPUT]
	[HttpPath("cp-api-base-uri", "/consumable/v1/{$type}/{$count}")]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class PurchaseConsumableOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("type")]
		public string Type;

		[HttpUriSegment("count")]
		public int Count;

		[HttpResponseJsonBody]
		public PurchaseConsumableResponse Response;

		public PurchaseConsumableOperation(string type, int count)
		{
			Type = type;
			Count = count;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.ConsumableInventory value = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>();
			if (value.Inventory.ContainsKey(Type))
			{
				value.Inventory[Type].itemCount += Count;
			}
			else
			{
				value.Inventory[Type] = new InventoryItemStock
				{
					itemCount = Count
				};
			}
			value.Inventory[Type].lastPurchaseTimestamp = DateTime.UtcNow.GetTimeInMilliseconds();
			offlineDatabase.Write(value);
			offlineDefinitions.SubtractConsumableCost(Type, Count);
			Response = new PurchaseConsumableResponse
			{
				assets = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>().Assets,
				inventory = new SignedResponse<ClubPenguin.Net.Domain.ConsumableInventory>
				{
					Data = new ClubPenguin.Net.Domain.ConsumableInventory
					{
						inventoryMap = value.Inventory
					}
				}
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.ConsumableInventory value = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>();
			value.Inventory = Response.inventory.Data.inventoryMap;
			offlineDatabase.Write(value);
			ClubPenguin.Net.Offline.PlayerAssets value2 = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			value2.Assets = Response.assets;
			offlineDatabase.Write(value2);
		}
	}
}
