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
	[HttpPath("cp-api-base-uri", "/reward/v1/claimQuickNotification")]
	public class ClaimQuickNotificationRewardOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public ClaimRewardResponse ResponseBody;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new ClaimRewardResponse();
			Reward quickNotificationReward = offlineDefinitions.GetQuickNotificationReward();
			offlineDefinitions.AddReward(quickNotificationReward, ResponseBody);
			JsonService jsonService = Service.Get<JsonService>();
			ResponseBody.reward = jsonService.Deserialize<RewardJsonReader>(jsonService.Serialize(RewardJsonWritter.FromReward(quickNotificationReward)));
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClaimRewardResponse responseBody = new ClaimRewardResponse();
			if (ResponseBody.reward != null)
			{
				offlineDefinitions.AddReward(ResponseBody.reward.ToReward(), responseBody);
			}
		}
	}
}
