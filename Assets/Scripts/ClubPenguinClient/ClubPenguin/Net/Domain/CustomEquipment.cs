using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct CustomEquipment
	{
		public long equipmentId;

		public long dateTimeCreated;

		public int definitionId;

		public CustomEquipmentPart[] parts;
	}
}
