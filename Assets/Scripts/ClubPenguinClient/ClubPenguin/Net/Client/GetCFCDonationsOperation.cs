using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain.ScheduledEvent;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpGET]
	[HttpPath("cp-api-base-uri", "/event/v1/cfc")]
	public class GetCFCDonationsOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public CFCDonations ResponseBody;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
