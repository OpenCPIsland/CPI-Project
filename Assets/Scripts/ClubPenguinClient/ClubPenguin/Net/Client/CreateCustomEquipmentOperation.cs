using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using Disney.Manimal.Common.Util;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/inventory/v1/equipment")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	public class CreateCustomEquipmentOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public CustomEquipment CustomEquipmentRequest;

		[HttpResponseJsonBody]
		public CreateEquipmentResponse CustomEquipmentResponse;

		public CreateCustomEquipmentOperation(CustomEquipment equipment)
		{
			CustomEquipmentRequest = equipment;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			Random random = new Random();
			byte[] array = new byte[8];
			random.NextBytes(array);
			long equipmentId = BitConverter.ToInt64(array, 0);
			createEquipment(equipmentId, offlineDatabase, offlineDefinitions);
			CustomEquipmentResponse = new CreateEquipmentResponse
			{
				assets = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>().Assets,
				equipmentId = equipmentId
			};
		}

		private void createEquipment(long equipmentId, OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			CustomEquipmentCollection value = offlineDatabase.Read<CustomEquipmentCollection>();
			CustomEquipment customEquipment = default(CustomEquipment);
			customEquipment.dateTimeCreated = DateTime.UtcNow.GetTimeInMilliseconds();
			customEquipment.definitionId = CustomEquipmentRequest.definitionId;
			customEquipment.equipmentId = equipmentId;
			customEquipment.parts = CustomEquipmentRequest.parts;
			CustomEquipment item = customEquipment;
			value.Equipment.Add(item);
			offlineDefinitions.SubtractEquipmentCost(item.definitionId);
			offlineDatabase.Write(value);
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			createEquipment(CustomEquipmentResponse.equipmentId, offlineDatabase, offlineDefinitions);
			ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			value.Assets = CustomEquipmentResponse.assets;
			offlineDatabase.Write(value);
		}
	}
}
