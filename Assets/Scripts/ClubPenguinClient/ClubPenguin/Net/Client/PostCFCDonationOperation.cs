using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain.ScheduledEvent;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/event/v1/cfc/{$coins}")]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class PostCFCDonationOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("coins")]
		public int Coins;

		[HttpResponseJsonBody]
		public DonationResult ResponseBody;

		public PostCFCDonationOperation(int coins)
		{
			Coins = coins;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
