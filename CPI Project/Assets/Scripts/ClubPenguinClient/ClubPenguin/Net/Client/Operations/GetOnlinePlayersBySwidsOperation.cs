using ClubPenguin.Net.Client.Mappers;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/player/v1/id/online")]
	public class GetOnlinePlayersBySwidsOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public IList<string> RequestBody;

		[HttpResponseJsonBody]
		public List<string> ResponseBody;

		public GetOnlinePlayersBySwidsOperation(IList<string> swids)
		{
			RequestBody = swids;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new List<string>();
		}
	}
}
