using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.ClothingDesigner
{
	[Serializable]
	public class EquipmentTemplateReward : AbstractListReward<int>
	{
		public List<int> EquipmentTemplates
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
				return "equipmentTemplates";
			}
		}

		public EquipmentTemplateReward()
		{
		}

		public EquipmentTemplateReward(int value)
			: base(value)
		{
		}
	}
}
