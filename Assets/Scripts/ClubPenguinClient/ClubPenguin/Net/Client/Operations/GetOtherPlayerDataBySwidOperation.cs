using hg.ApiWebKit.core.attributes;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpPath("cp-api-base-uri", "/player/v1/id/{$swid}")]
	public class GetOtherPlayerDataBySwidOperation : GetOtherPlayerDataOperation
	{
		[HttpUriSegment("swid")]
		public string Swid;

		public GetOtherPlayerDataBySwidOperation(string swid)
		{
			Swid = swid;
		}
	}
}
