using ClubPenguin.Net.Client.Mappers;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpPath("cp-api-base-uri", "/moderation/v1/report/name/{$displayName}")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpContentType("application/json")]
	public class ReportPlayerOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public string RequestBody;

		[HttpUriSegment("displayName")]
		public string DisplayName;

		public ReportPlayerOperation(string displayName, string reason)
		{
			RequestBody = reason;
			DisplayName = displayName;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
		}
	}
}
