using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	public class StructureRewardDefinition : AbstractStaticGameDataRewardDefinition<StructureDefinition>
	{
		public StructureDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new StructureReward(Definition.Id);
			}
		}

		protected override StructureDefinition getField()
		{
			return Definition;
		}
	}
}
