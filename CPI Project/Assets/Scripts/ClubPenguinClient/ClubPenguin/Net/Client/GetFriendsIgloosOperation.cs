using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/igloo/v1/igloos/friends")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpGET]
	[HttpAccept("application/json")]
	[RequestQueue("FriendsIgloos")]
	public class GetFriendsIgloosOperation : CPAPIHttpOperation
	{
		[HttpQueryString("language")]
		public string Language;

		[HttpResponseJsonBody]
		public List<IglooListItem> IglooListItems;

		public GetFriendsIgloosOperation(string language)
		{
			Language = language;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			IglooListItems = new List<IglooListItem>();
		}
	}
}
