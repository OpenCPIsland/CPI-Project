using ClubPenguin.Net.Client.Mappers;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpPUT]
	[HttpContentType("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/player/v1/referral")]
	public class SetReferralOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public string RequestBody;

		public SetReferralOperation(string referrerDisplayName)
		{
			RequestBody = referrerDisplayName;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
		}
	}
}
