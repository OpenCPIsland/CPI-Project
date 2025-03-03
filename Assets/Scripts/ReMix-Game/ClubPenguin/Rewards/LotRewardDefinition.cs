using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Rewards
{
	[Serializable]
	public class LotRewardDefinition : AbstractStaticGameDataRewardDefinition<LotDefinition>
	{
		public LotDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new LotReward(Definition.LotName);
			}
		}

		protected override LotDefinition getField()
		{
			return Definition;
		}
	}
}
