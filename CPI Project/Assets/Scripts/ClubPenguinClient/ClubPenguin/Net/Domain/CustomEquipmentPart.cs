using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct CustomEquipmentPart
	{
		public int slotIndex;

		public CustomEquipmentCustomization[] customizations;
	}
}
