using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Rewards
{
	[Serializable]
	public class LightingRewardDefinition : AbstractStaticGameDataRewardDefinition<LightingDefinition>
	{
		public LightingDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new LightingReward(Definition.Id);
			}
		}

		protected override LightingDefinition getField()
		{
			return Definition;
		}
	}
}
