using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpGET]
	[HttpPath("cp-api-base-uri", "/game/v1/world/roomPopulation/{$room}")]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class GetRoomPopulationForWorldsOperation : CPAPIHttpOperation
	{
		[HttpQueryString("language")]
		public string Language;

		[HttpUriSegment("room")]
		public string Room;

		[HttpResponseJsonBody]
		public GetRoomPopulationForWorldsResponse Response;

		public GetRoomPopulationForWorldsOperation(string room, string language)
		{
			Room = room;
			Language = language;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			Response = new GetRoomPopulationForWorldsResponse
			{
				worldRoomPopulations = new List<WorldRoomPopulation>()
			};
		}
	}
}
