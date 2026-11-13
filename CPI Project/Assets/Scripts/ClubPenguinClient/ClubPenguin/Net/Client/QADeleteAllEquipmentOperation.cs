using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpDELETE]
	[HttpPath("cp-api-base-uri", "/inventory/v1/qa/equipment")]
	[HttpAccept("application/json")]
	public class QADeleteAllEquipmentOperation : CPAPIHttpOperation
	{
		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			CustomEquipmentCollection value = offlineDatabase.Read<CustomEquipmentCollection>();
			value.Equipment.Clear();
			offlineDatabase.Write(value);
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			PerformOfflineAction(offlineDatabase, offlineDefinitions);
		}
	}
}
