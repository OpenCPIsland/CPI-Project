using System;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public class EquipmentModelDefinition
	{
		[Serializable]
		public struct Part
		{
			public int SlotIndex;

			public EquipmentPartType PartType;

			public bool Required;
		}

		public string Name;

		public Part[] Parts;
	}
}
