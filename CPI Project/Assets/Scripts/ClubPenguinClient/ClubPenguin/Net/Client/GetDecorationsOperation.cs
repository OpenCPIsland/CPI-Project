using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Domain.Scene;
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
			DecorationInventoryEntity decorationInventoryEntity = offlineDatabase.Read<DecorationInventoryEntity>();
			Dictionary<string, int> inventory = new Dictionary<string, int>();
			if (decorationInventoryEntity.inventory != null)
			{
				foreach (KeyValuePair<string, int> item in decorationInventoryEntity.inventory)
				{
					inventory[item.Key] = item.Value;
				}
			}
			AddPlacedDecorations(inventory, offlineDatabase);
			foreach (KeyValuePair<string, int> item in inventory)
			{
				DecorationInventory.items.Add(new DecorationInventoryItem
				{
					decorationId = DecorationId.FromString(item.Key),
					count = item.Value
				});
			}
		}

		private void AddPlacedDecorations(Dictionary<string, int> inventory, OfflineDatabase offlineDatabase)
		{
			SceneLayoutEntity sceneLayoutEntity = offlineDatabase.Read<SceneLayoutEntity>();
			if (sceneLayoutEntity.Layouts == null)
			{
				return;
			}
			foreach (SavedSceneLayout layout in sceneLayoutEntity.Layouts)
			{
				if (layout.decorationsLayout == null)
				{
					continue;
				}
				foreach (DecorationLayout decoration in layout.decorationsLayout)
				{
					DecorationId decorationId = new DecorationId((int)decoration.definitionId, decoration.type);
					string key = decorationId.ToString();
					if (inventory.ContainsKey(key))
					{
						inventory[key]++;
					}
					else
					{
						inventory[key] = 1;
					}
				}
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
