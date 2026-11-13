using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	public class DecorationRewardDefinition : AbstractStaticGameDataRewardDefinition<DecorationDefinition>
	{
		public DecorationDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new DecorationReward(Definition.Id);
			}
		}

		protected override DecorationDefinition getField()
		{
			return Definition;
		}
	}
}
