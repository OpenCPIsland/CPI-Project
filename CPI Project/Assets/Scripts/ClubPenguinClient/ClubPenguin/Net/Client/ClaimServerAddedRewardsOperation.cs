using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/reward/v1/claimServerAdded")]
	public class ClaimServerAddedRewardsOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public ClaimServerAddedRewardsResponse ResponseBody;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new ClaimServerAddedRewardsResponse
			{
				claimedRewards = new List<ClaimedServerAddedReward>()
			};
		}
	}
}
