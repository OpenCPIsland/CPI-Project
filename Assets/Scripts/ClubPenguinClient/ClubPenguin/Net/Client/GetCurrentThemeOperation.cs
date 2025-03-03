using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpPath("cp-api-base-uri", "/catalog/v1/clothing/themes/stats")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpGET]
	public class GetCurrentThemeOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public CurrentThemeResponse Response;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			Response = new CurrentThemeResponse
			{
				themes = new List<CurrentThemeData>()
			};
		}
	}
}
