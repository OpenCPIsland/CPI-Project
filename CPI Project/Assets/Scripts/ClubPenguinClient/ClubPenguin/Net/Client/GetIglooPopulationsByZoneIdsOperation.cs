using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common.DataModel;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/igloo/v1/igloos/populations/zoneIds")]
	[RequestQueue("zoneIds")]
	[HttpPOST]
	[HttpAccept("application/json")]
	public class GetIglooPopulationsByZoneIdsOperation : CPAPIHttpOperation
	{
		public List<DataEntityHandle> Handles;

		[HttpQueryString("language")]
		public string Language;

		[HttpRequestJsonBody]
		public IList<ZoneId> ZoneIds;

		[HttpResponseJsonBody]
		public List<RoomPopulation> RoomPopulations;

		public GetIglooPopulationsByZoneIdsOperation(string language, IList<ZoneId> zoneIds, List<DataEntityHandle> handles)
		{
			Language = language;
			ZoneIds = zoneIds;
			Handles = handles;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			RoomPopulations = new List<RoomPopulation>();
		}
	}
}
