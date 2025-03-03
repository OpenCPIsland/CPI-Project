using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using hg.ApiWebKit;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.core.http;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpContentType("application/json")]
	[HttpPUT]
	[HttpPath("cp-api-base-uri", "/reward/v1/claim/{$type}")]
	public class ClaimRewardOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("type")]
		public int RewardId;

		[HttpResponseJsonBody]
		public ClaimRewardResponse ResponseBody;

		private bool claimed;

		public ClaimRewardOperation(int rewardId)
		{
			RewardId = rewardId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			claimed = ClaimReward(RewardId, out ResponseBody, offlineDatabase, offlineDefinitions);
		}

		protected override HttpResponse CreateOfflineResponse()
		{
			HttpResponse httpResponse = base.CreateOfflineResponse();
			if (!claimed)
			{
				httpResponse.StatusCode = HttpStatusCode.Gone;
			}
			return httpResponse;
		}

		internal static bool ClaimReward(int rewardId, out ClaimRewardResponse responseBody, OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			responseBody = new ClaimRewardResponse();
			ClaimableRewardData value = offlineDatabase.Read<ClaimableRewardData>();
			if (value.ClimedRewards.Contains(rewardId))
			{
				return false;
			}
			Reward claimableReward = offlineDefinitions.GetClaimableReward(rewardId);
			if (claimableReward != null)
			{
				offlineDefinitions.AddReward(claimableReward, responseBody);
				value.ClimedRewards.Add(rewardId);
				offlineDatabase.Write(value);
				JsonService jsonService = Service.Get<JsonService>();
				responseBody.reward = jsonService.Deserialize<RewardJsonReader>(jsonService.Serialize(RewardJsonWritter.FromReward(claimableReward)));
				return true;
			}
			return false;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClaimRewardResponse responseBody = new ClaimRewardResponse();
			ClaimReward(RewardId, out responseBody, offlineDatabase, offlineDefinitions);
		}
	}
}
