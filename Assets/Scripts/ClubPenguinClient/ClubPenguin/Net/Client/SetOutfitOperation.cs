using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/player/v1/outfit")]
	public class SetOutfitOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public PlayerOutfit RequestBody;

		[HttpResponseJsonBody]
		public SignedResponse<ClubPenguin.Net.Domain.PlayerOutfitDetails> ResponseBody;

		public SetOutfitOperation(PlayerOutfit outfit)
		{
			RequestBody = outfit;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			CustomEquipmentCollection customEquipmentCollection = offlineDatabase.Read<CustomEquipmentCollection>();
			ClubPenguin.Net.Offline.PlayerOutfitDetails value = default(ClubPenguin.Net.Offline.PlayerOutfitDetails);
			value.Init();
			long[] parts = RequestBody.parts;
			foreach (long num in parts)
			{
				foreach (CustomEquipment item in customEquipmentCollection.Equipment)
				{
					if (item.equipmentId == num)
					{
						value.Parts.Add(item);
						break;
					}
				}
			}
			offlineDatabase.Write(value);
			ResponseBody = new SignedResponse<ClubPenguin.Net.Domain.PlayerOutfitDetails>
			{
				Data = new ClubPenguin.Net.Domain.PlayerOutfitDetails
				{
					parts = value.Parts.ToArray()
				}
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.PlayerOutfitDetails value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerOutfitDetails>();
			value.Parts = new List<CustomEquipment>(ResponseBody.Data.parts);
			offlineDatabase.Write(value);
		}
	}
}
