using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class CreateEquipmentResponse : CPResponse
	{
		public long equipmentId;

		public PlayerAssets assets;
	}
}
