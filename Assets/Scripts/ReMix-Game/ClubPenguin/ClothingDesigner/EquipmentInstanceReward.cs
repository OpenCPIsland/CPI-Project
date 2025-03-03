using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.ClothingDesigner
{
	[Serializable]
	public class EquipmentInstanceReward : AbstractListReward<CustomEquipment>
	{
		public List<CustomEquipment> EquipmentInstances
		{
			get
			{
				return data;
			}
		}

		public override string RewardType
		{
			get
			{
				return "equipmentInstances";
			}
		}

		public EquipmentInstanceReward()
		{
		}

		public EquipmentInstanceReward(CustomEquipment value)
			: base(value)
		{
		}
	}
}
