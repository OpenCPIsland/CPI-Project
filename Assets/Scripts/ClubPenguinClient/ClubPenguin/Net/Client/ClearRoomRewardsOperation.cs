using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/reward/v1/qa/rooms")]
	[HttpAccept("application/json")]
	[HttpDELETE]
	public class ClearRoomRewardsOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public ClubPenguin.Net.Domain.PlayerAssets ResponseBody;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.InRoomRewards value = default(ClubPenguin.Net.Offline.InRoomRewards);
			value.Init();
			offlineDatabase.Write(value);
		}
	}
}
