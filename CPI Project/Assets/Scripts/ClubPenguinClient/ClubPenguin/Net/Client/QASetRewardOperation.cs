using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/reward/v1/qa")]
	[HttpContentType("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	public class QASetRewardOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public Reward RequestBody;

		[HttpResponseJsonBody]
		public RewardGrantedResponse ResponseBody;

		public QASetRewardOperation(Reward reward)
		{
			RequestBody = reward;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new RewardGrantedResponse();
			offlineDefinitions.SetReward(RequestBody, ResponseBody);
			ResponseBody.assets = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>().Assets;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			value.Assets = ResponseBody.assets;
			offlineDatabase.Write(value);
		}
	}
}
