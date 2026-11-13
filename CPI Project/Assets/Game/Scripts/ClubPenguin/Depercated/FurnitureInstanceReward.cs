using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Depercated
{
	[Serializable]
	public class FurnitureInstanceReward : AbstractListReward<string>
	{
		public override string RewardType
		{
			get
			{
				return "furnitureTemplates";
			}
		}

		public FurnitureInstanceReward()
		{
		}

		public FurnitureInstanceReward(string value)
			: base(value)
		{
		}
	}
}
