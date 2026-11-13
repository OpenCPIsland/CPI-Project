using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpPath("cp-api-base-uri", "/igloo/v1/qa/layouts")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpDELETE]
	public class QADeleteAllIgloosOperation : CPAPIHttpOperation
	{
		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			IglooEntity value = offlineDatabase.Read<IglooEntity>();
			value.Init();
			offlineDatabase.Write(value);
			SceneLayoutEntity value2 = offlineDatabase.Read<SceneLayoutEntity>();
			value2.Init();
			offlineDatabase.Write(value2);
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			PerformOfflineAction(offlineDatabase, offlineDefinitions);
		}
	}
}
