using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpAccept("application/json")]
	[HttpGET]
	[HttpPath("cp-api-base-uri", "/game/v1/roomPopulation/{$world}")]
	public class GetRoomPopulationOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("world")]
		public string World;

		[HttpQueryString("language")]
		public string Language;

		[HttpResponseJsonBody]
		public GetRoomPopulationResponse Response;

		public GetRoomPopulationOperation(string world, string language)
		{
			World = world;
			Language = language;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			Response = new GetRoomPopulationResponse
			{
				roomPopulations = new List<RoomPopulation>()
			};
		}
	}
}
