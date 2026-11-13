using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Depercated
{
	[Serializable]
	public class FurnitureTemplateReward : AbstractListReward<object>
	{
		public override string RewardType
		{
			get
			{
				return "furnitureInstances";
			}
		}

		public FurnitureTemplateReward()
		{
		}

		public FurnitureTemplateReward(object value)
			: base(value)
		{
		}
	}
}
