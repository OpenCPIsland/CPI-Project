using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using LitJson;
using System.Collections.Generic;
using System.Linq;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[HttpPath("cp-api-base-uri", "/minigame/v1/fishing/catch")]
	[HttpPOST]
	public class FishingCatchOperation : CPAPIHttpOperation
	{
		private ClubPenguinClient client;

		[HttpRequestJsonBody]
		public FishingCatchRequest FishingCatchRequest;

		[HttpResponseJsonBody]
		public FishingCatchResponse Response;

		public FishingCatchOperation(ClubPenguinClient clubPenguinClient, FishingCatchRequest request)
		{
			client = clubPenguinClient;
			FishingCatchRequest = request;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			Response = new FishingCatchResponse();
			Reward fishingReward = offlineDefinitions.GetFishingReward(FishingCatchRequest.winningRewardName);
			offlineDefinitions.AddReward(fishingReward, Response);
			JsonService jsonService = Service.Get<JsonService>();
			Dictionary<string, RewardJsonReader> dictionary = new Dictionary<string, RewardJsonReader>();
			dictionary.Add(client.PlayerSessionId.ToString(), jsonService.Deserialize<RewardJsonReader>(jsonService.Serialize(RewardJsonWritter.FromReward(fishingReward))));
			Response.rewards = new SignedResponse<RewardedUserCollectionJsonHelper>
			{
				Data = new RewardedUserCollectionJsonHelper
				{
					rewards = dictionary,
					source = RewardSource.MINI_GAME,
					sourceId = "fishing"
				}
			};
			if (Response.wsEvents == null)
			{
				Response.wsEvents = new List<SignedResponse<WebServiceEvent>>();
			}
			Response.wsEvents.Add(new SignedResponse<WebServiceEvent>
			{
				Data = new WebServiceEvent
				{
					type = 1,
					details = new JsonData(FishingCatchRequest.winningRewardName)
				}
			});
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			if (Response.rewards.Data.rewards != null && Response.rewards.Data.rewards.Count > 0)
			{
				FishingCatchResponse responseBody = new FishingCatchResponse();
				offlineDefinitions.AddReward(Response.rewards.Data.rewards.First().Value.ToReward(), responseBody);
			}
		}
	}
}
