using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpGET]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/igloo/v1/igloos/popular")]
	[RequestQueue("PopularIgloos")]
	public class GetPopularIgloosOperation : CPAPIHttpOperation
	{
		[HttpQueryString("language")]
		public string Language;

		[HttpResponseJsonBody]
		public List<IglooListItem> IglooListItems;

		public GetPopularIgloosOperation(string language)
		{
			Language = language;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			IglooListItems = new List<IglooListItem>();
		}
	}
}
