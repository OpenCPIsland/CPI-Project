using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpContentType("application/json")]
	[HttpPath("cp-api-base-uri", "/player/v1/id")]
	public class GetOtherPlayerDataBySwidsOperation : GetOtherPlayerDatasOperation
	{
		[HttpRequestJsonBody]
		public IList<string> RequestBody;

		public GetOtherPlayerDataBySwidsOperation(IList<string> swids)
		{
			RequestBody = swids;
		}
	}
}
