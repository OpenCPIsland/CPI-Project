using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpGET]
	[HttpAccept("application/json")]
	[HttpPath("cp-api-base-uri", "/player/v1/durable/equip/{$type}")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class EquipDurableOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("type")]
		public int propId;

		[HttpResponseJsonBody]
		public SignedResponse<EquipDurableResponse> SignedEquipDurableResponse;

		public EquipDurableOperation(int propId)
		{
			this.propId = propId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			SignedEquipDurableResponse = new SignedResponse<EquipDurableResponse>
			{
				Data = new EquipDurableResponse
				{
					propId = propId
				}
			};
		}
	}
}
