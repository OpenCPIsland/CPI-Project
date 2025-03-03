using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.core.http;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/reward/v1/qa/claim/{$type}/{$time}")]
	[HttpContentType("application/json")]
	[HttpPUT]
	public class QAClaimRewardOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("type")]
		public int RewardId;

		[HttpUriSegment("time")]
		public long TimeInSeconds;

		[HttpResponseJsonBody]
		public ClaimRewardResponse ResponseBody;

		private bool claimed;

		public QAClaimRewardOperation(int rewardId, long timeInSeconds)
		{
			RewardId = rewardId;
			TimeInSeconds = timeInSeconds;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			claimed = ClaimRewardOperation.ClaimReward(RewardId, out ResponseBody, offlineDatabase, offlineDefinitions);
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

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClaimRewardResponse responseBody = new ClaimRewardResponse();
			ClaimRewardOperation.ClaimReward(RewardId, out responseBody, offlineDatabase, offlineDefinitions);
		}
	}
}
