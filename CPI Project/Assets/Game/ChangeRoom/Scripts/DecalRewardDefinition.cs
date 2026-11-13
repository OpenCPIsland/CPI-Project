using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.ClothingDesigner
{
	[Serializable]
	public class DecalRewardDefinition : AbstractStaticGameDataRewardDefinition<DecalDefinition>
	{
		public DecalDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new DecalReward(Definition.Id);
			}
		}

		protected override DecalDefinition getField()
		{
			return Definition;
		}
	}
}
