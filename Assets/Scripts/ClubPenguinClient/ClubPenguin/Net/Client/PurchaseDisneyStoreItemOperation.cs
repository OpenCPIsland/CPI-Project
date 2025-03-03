using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/disneystore/v1/purchase/{$type}/{$count}")]
	[HttpAccept("application/json")]
	[HttpPUT]
	public class PurchaseDisneyStoreItemOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("type")]
		public int ItemId;

		[HttpUriSegment("count")]
		public int Count;

		[HttpResponseJsonBody]
		public PurchaseDisneyStoreItemResponse Response;

		public PurchaseDisneyStoreItemOperation(int itemId, int count)
		{
			ItemId = itemId;
			Count = count;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			Response = Purchase(ItemId, Count, offlineDatabase, offlineDefinitions);
		}

		public static PurchaseDisneyStoreItemResponse Purchase(int itemId, int count, OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			PurchaseDisneyStoreItemResponse purchaseDisneyStoreItemResponse = new PurchaseDisneyStoreItemResponse();
			Reward disneyStoreItemReward = offlineDefinitions.GetDisneyStoreItemReward(itemId, count);
			offlineDefinitions.AddReward(disneyStoreItemReward, purchaseDisneyStoreItemResponse);
			offlineDefinitions.SubtractDisneyStoreItemCost(itemId, count);
			purchaseDisneyStoreItemResponse.assets = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>().Assets;
			purchaseDisneyStoreItemResponse.inventory = new SignedResponse<ClubPenguin.Net.Domain.ConsumableInventory>
			{
				Data = new ClubPenguin.Net.Domain.ConsumableInventory
				{
					inventoryMap = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>().Inventory
				}
			};
			DecorationInventory decorationInventory = new DecorationInventory();
			decorationInventory.items = new List<DecorationInventoryItem>();
			foreach (KeyValuePair<string, int> item in offlineDatabase.Read<DecorationInventoryEntity>().inventory)
			{
				decorationInventory.items.Add(new DecorationInventoryItem
				{
					count = item.Value,
					decorationId = DecorationId.FromString(item.Key)
				});
			}
			purchaseDisneyStoreItemResponse.decorationInventory = new SignedResponse<DecorationInventory>
			{
				Data = decorationInventory
			};
			return purchaseDisneyStoreItemResponse;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			value.Assets = Response.assets;
			offlineDatabase.Write(value);
			DecorationInventoryEntity value2 = offlineDatabase.Read<DecorationInventoryEntity>();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (DecorationInventoryItem item in Response.decorationInventory.Data.items)
			{
				dictionary.Add(item.decorationId.ToString(), item.count);
			}
			value2.inventory = dictionary;
			offlineDatabase.Write(value2);
			ClubPenguin.Net.Offline.ConsumableInventory value3 = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>();
			value3.Inventory = Response.inventory.Data.inventoryMap;
			offlineDatabase.Write(value3);
		}
	}
}
