using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/igloo/v1/qa/decorations")]
	[HttpAccept("application/json")]
	[HttpDELETE]
	public class QADeleteAllDecorationsOperation : CPAPIHttpOperation
	{
		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			DecorationInventoryEntity value = offlineDatabase.Read<DecorationInventoryEntity>();
			value.Init();
			offlineDatabase.Write(value);
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			PerformOfflineAction(offlineDatabase, offlineDefinitions);
		}
	}
}
