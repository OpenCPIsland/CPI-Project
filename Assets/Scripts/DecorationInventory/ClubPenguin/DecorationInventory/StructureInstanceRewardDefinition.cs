using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	public class StructureInstanceRewardDefinition : AbstractStaticGameDataRewardDefinition<StructureDefinition>
	{
		public StructureDefinition Decoration;

		public int Count;

		public override IRewardable Reward
		{
			get
			{
				return new StructureInstanceReward(Decoration.Id, Count);
			}
		}

		protected override StructureDefinition getField()
		{
			return Decoration;
		}
	}
}
