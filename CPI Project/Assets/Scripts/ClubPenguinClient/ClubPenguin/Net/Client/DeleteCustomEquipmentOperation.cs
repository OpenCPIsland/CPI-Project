using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpDELETE]
	[HttpPath("cp-api-base-uri", "/inventory/v1/equipment/{$equipmentId}")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class DeleteCustomEquipmentOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("equipmentId")]
		public long EquipmentId;

		[HttpResponseJsonBody]
		public CustomEquipmentId CustomEquipmentId;

		public DeleteCustomEquipmentOperation(long equipmentId)
		{
			EquipmentId = equipmentId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			CustomEquipmentCollection value = offlineDatabase.Read<CustomEquipmentCollection>();
			foreach (CustomEquipment item in value.Equipment)
			{
				if (item.equipmentId == EquipmentId)
				{
					value.Equipment.Remove(item);
					break;
				}
			}
			offlineDatabase.Write(value);
			CustomEquipmentId = new CustomEquipmentId
			{
				equipmentId = EquipmentId
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			CustomEquipmentCollection value = offlineDatabase.Read<CustomEquipmentCollection>();
			foreach (CustomEquipment item in value.Equipment)
			{
				if (item.equipmentId == EquipmentId)
				{
					value.Equipment.Remove(item);
					break;
				}
			}
			offlineDatabase.Write(value);
		}
	}
}
