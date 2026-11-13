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
	[HttpPUT]
	[HttpAccept("application/json")]
	[HttpPath("cp-api-base-uri", "/disneystore/v1/qa/purchase/{$type}/{$count}/{$time}")]
	public class QAPurchaseDisneyStoreItemOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("type")]
		public int ItemId;

		[HttpUriSegment("count")]
		public int Count;

		[HttpUriSegment("time")]
		public long TimeInSeconds;

		[HttpResponseJsonBody]
		public PurchaseDisneyStoreItemResponse Response;

		public QAPurchaseDisneyStoreItemOperation(int itemId, int count, long timeInSeconds)
		{
			ItemId = itemId;
			Count = count;
			TimeInSeconds = timeInSeconds;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			Response = PurchaseDisneyStoreItemOperation.Purchase(ItemId, Count, offlineDatabase, offlineDefinitions);
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
