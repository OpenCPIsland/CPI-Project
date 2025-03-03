using hg.ApiWebKit.core.attributes;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpPath("cp-api-base-uri", "/player/v1/session/{$sessionId}")]
	public class GetOtherPlayerDataBySessionIdOperation : GetOtherPlayerDataOperation
	{
		[HttpUriSegment("sessionId")]
		public long SessionId;

		public GetOtherPlayerDataBySessionIdOperation(long sessionId)
		{
			SessionId = sessionId;
		}
	}
}
