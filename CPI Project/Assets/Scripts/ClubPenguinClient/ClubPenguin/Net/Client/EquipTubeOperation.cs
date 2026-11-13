using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPUT]
	[HttpPath("cp-api-base-uri", "/tube/v1/equip/{$type}")]
	public class EquipTubeOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("type")]
		public int TubeId;

		[HttpResponseJsonBody]
		public SignedResponse<EquipTubeResponse> SignedEquipTubeResponse;

		public EquipTubeOperation(int tubeId)
		{
			TubeId = tubeId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			TubeData value = offlineDatabase.Read<TubeData>();
			value.EquippedTubeId = TubeId;
			offlineDatabase.Write(value);
			SignedEquipTubeResponse = new SignedResponse<EquipTubeResponse>
			{
				Data = new EquipTubeResponse
				{
					tubeId = TubeId
				}
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			TubeData value = offlineDatabase.Read<TubeData>();
			value.EquippedTubeId = TubeId;
			offlineDatabase.Write(value);
		}
	}
}
