using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpGET]
	[HttpPath("cp-api-base-uri", "/igloo/v1/decorations")]
	[HttpAccept("application/json")]
	public class GetDecorationsOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public DecorationInventory DecorationInventory;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			DecorationInventory = new DecorationInventory();
			DecorationInventory.items = new List<DecorationInventoryItem>();
			foreach (KeyValuePair<string, int> item in offlineDatabase.Read<DecorationInventoryEntity>().inventory)
			{
				DecorationInventory.items.Add(new DecorationInventoryItem
				{
					decorationId = DecorationId.FromString(item.Key),
					count = item.Value
				});
			}
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			DecorationInventoryEntity value = offlineDatabase.Read<DecorationInventoryEntity>();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (DecorationInventoryItem item in DecorationInventory.items)
			{
				dictionary.Add(item.decorationId.ToString(), item.count);
			}
			value.inventory = dictionary;
			offlineDatabase.Write(value);
		}
	}
}
