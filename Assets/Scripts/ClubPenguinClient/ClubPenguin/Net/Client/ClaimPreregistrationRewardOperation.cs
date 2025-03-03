using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpContentType("application/json")]
	[HttpGET]
	[HttpPath("cp-api-base-uri", "/reward/v1/preregistration")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class ClaimPreregistrationRewardOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public ClaimPreregistrationRewardResponse ResponseBody;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new ClaimPreregistrationRewardResponse
			{
				reward = new RewardJsonReader(),
				wsEvents = new List<SignedResponse<WebServiceEvent>>()
			};
		}
	}
}
