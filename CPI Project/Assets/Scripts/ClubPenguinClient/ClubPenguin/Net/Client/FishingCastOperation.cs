using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/minigame/v1/fishing/cast")]
	[HttpContentType("application/json")]
	public class FishingCastOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public SignedResponse<InWorldState> SignedInWorldState;

		[HttpResponseJsonBody]
		public SignedResponse<FishingResult> SignedFishingResult;

		public FishingCastOperation(SignedResponse<InWorldState> state)
		{
			SignedInWorldState = state;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			SignedFishingResult = new SignedResponse<FishingResult>
			{
				Data = new FishingResult
				{
					availablePrizeMap = offlineDefinitions.GetRandomFishingPrizes()
				}
			};
		}
	}
}
