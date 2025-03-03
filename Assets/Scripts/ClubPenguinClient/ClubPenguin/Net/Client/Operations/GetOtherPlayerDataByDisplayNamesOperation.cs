using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpContentType("application/json")]
	[HttpPath("cp-api-base-uri", "/player/v1/names")]
	public class GetOtherPlayerDataByDisplayNamesOperation : GetOtherPlayerDatasOperation
	{
		[HttpRequestJsonBody]
		public IList<string> RequestBody;

		public GetOtherPlayerDataByDisplayNamesOperation(IList<string> displayNames)
		{
			RequestBody = displayNames;
		}
	}
}
