using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/inventory/v1/equipment")]
	[HttpGET]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpAccept("application/json")]
	public class GetInventoryOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public List<CustomEquipment> CustomEquipmentResponses;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			CustomEquipmentResponses = offlineDatabase.Read<CustomEquipmentCollection>().Equipment;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			CustomEquipmentCollection value = offlineDatabase.Read<CustomEquipmentCollection>();
			value.Equipment = CustomEquipmentResponses;
			offlineDatabase.Write(value);
		}
	}
}
