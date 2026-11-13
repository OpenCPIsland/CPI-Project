using hg.ApiWebKit.core.attributes;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpPath("cp-api-base-uri", "/player/v1/name/{$displayName}")]
	public class GetOtherPlayerDataByDisplayNameOperation : GetOtherPlayerDataOperation
	{
		[HttpUriSegment("displayName")]
		public string DisplayName;

		public GetOtherPlayerDataByDisplayNameOperation(string displayName)
		{
			DisplayName = displayName;
		}
	}
}
