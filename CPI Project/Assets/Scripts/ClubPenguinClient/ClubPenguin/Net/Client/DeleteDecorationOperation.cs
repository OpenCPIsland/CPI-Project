using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/igloo/v1/decorations/{$decorationId}")]
	[HttpDELETE]
	public class DeleteDecorationOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("decorationId")]
		public string DecorationId;

		public DeleteDecorationOperation(string decorationId)
		{
			DecorationId = decorationId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			DecorationInventoryEntity value = offlineDatabase.Read<DecorationInventoryEntity>();
			if (value.inventory.ContainsKey(DecorationId))
			{
				value.inventory.Remove(DecorationId);
				offlineDatabase.Write(value);
			}
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			PerformOfflineAction(offlineDatabase, offlineDefinitions);
		}
	}
}
