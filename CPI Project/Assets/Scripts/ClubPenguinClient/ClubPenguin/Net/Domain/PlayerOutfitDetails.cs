using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct PlayerOutfitDetails
	{
		public long? sessionId;

		public CustomEquipment[] parts;
	}
}
