using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpGET]
	[HttpPath("cp-website-api-base-uri", "/wp-json/snowflake/v1/social")]
	[HttpAccept("application/json")]
	public class GetLatestPostTimeOperation : CPAPIHttpOperation
	{
		[HttpQueryString("lang")]
		public string Language;

		[HttpResponseJsonBody]
		public LatestPostTime LatestPostTimeResponse;

		public GetLatestPostTimeOperation(string language)
		{
			Language = language;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			LatestPostTimeResponse = new LatestPostTime
			{
				timestamp = 0
			};
		}
	}
}
