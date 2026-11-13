using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpContentType("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPUT]
	[HttpPath("cp-api-base-uri", "/reward/v1")]
	public class AddRewardOperation : CPAPIHttpOperation
	{
		private ClubPenguinClient clubPenguinClient;

		[HttpRequestJsonBody]
		public SignedResponse<RewardedUserCollectionJsonHelper> RequestBody;

		[HttpResponseJsonBody]
		public RewardGrantedResponse ResponseBody;

		public AddRewardOperation(ClubPenguinClient clubPenguinClient, SignedResponse<RewardedUserCollectionJsonHelper> rewards)
		{
			RequestBody = rewards;
			this.clubPenguinClient = clubPenguinClient;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new RewardGrantedResponse();
			foreach (KeyValuePair<string, RewardJsonReader> reward in RequestBody.Data.rewards)
			{
				if (reward.Key == clubPenguinClient.PlayerSessionId.ToString())
				{
					offlineDefinitions.AddReward(reward.Value.ToReward(), ResponseBody);
				}
			}
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
