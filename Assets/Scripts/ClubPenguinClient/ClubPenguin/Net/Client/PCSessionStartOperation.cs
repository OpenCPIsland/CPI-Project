using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/iap-pc/v1/session")]
	[HttpAccept("application/json")]
	public class PCSessionStartOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public PCSessionStartRequest PCSessionStartRequest;

		[HttpResponseJsonBody]
		public PCSessionStartResponse PCSessionStartResponse;

		public PCSessionStartOperation(PCSessionStartRequest startSessionRequest)
		{
			PCSessionStartRequest = startSessionRequest;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			PCSessionStartResponse = default(PCSessionStartResponse);
		}
	}
}
