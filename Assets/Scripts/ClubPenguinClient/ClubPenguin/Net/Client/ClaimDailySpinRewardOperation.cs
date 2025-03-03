using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpContentType("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/reward/v1/claimDailySpin")]
	public class ClaimDailySpinRewardOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public ClaimDailySpinRewardResponse ResponseBody;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new ClaimDailySpinRewardResponse();
			Reward reward = new Reward();
			Reward reward2 = new Reward();
			int spinResult = offlineDefinitions.GetSpinResult(reward, reward2);
			offlineDefinitions.AddReward(reward, ResponseBody);
			offlineDefinitions.AddReward(reward2, ResponseBody);
			JsonService jsonService = Service.Get<JsonService>();
			ResponseBody.spinOutcomeId = spinResult;
			ResponseBody.reward = jsonService.Deserialize<RewardJsonReader>(jsonService.Serialize(RewardJsonWritter.FromReward(reward)));
			ResponseBody.chestReward = jsonService.Deserialize<RewardJsonReader>(jsonService.Serialize(RewardJsonWritter.FromReward(reward2)));
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClaimDailySpinRewardResponse responseBody = new ClaimDailySpinRewardResponse();
			if (ResponseBody.reward != null)
			{
				offlineDefinitions.AddReward(ResponseBody.reward.ToReward(), responseBody);
			}
			if (ResponseBody.chestReward != null)
			{
				offlineDefinitions.AddReward(ResponseBody.chestReward.ToReward(), responseBody);
			}
		}
	}
}
