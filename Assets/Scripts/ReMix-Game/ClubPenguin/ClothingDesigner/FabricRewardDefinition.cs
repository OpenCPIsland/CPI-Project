using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.ClothingDesigner
{
	[Serializable]
	public class FabricRewardDefinition : AbstractStaticGameDataRewardDefinition<FabricDefinition>
	{
		public FabricDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new FabricReward(Definition.Id);
			}
		}

		protected override FabricDefinition getField()
		{
			return Definition;
		}
	}
}
