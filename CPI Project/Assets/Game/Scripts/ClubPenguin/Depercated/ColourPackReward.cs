using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Depercated
{
	[Serializable]
	public class ColourPackReward : AbstractListReward<string>
	{
		public override string RewardType
		{
			get
			{
				return "colourPacks";
			}
		}

		public ColourPackReward()
		{
		}

		public ColourPackReward(string value)
			: base(value)
		{
		}
	}
}
