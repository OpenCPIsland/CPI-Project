using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using System;

namespace ClubPenguin.Durable
{
	[Serializable]
	public class DurableRewardDefinition : AbstractStaticGameDataRewardDefinition<PropDefinition>
	{
		public PropDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new DurableReward(Definition.Id);
			}
		}

		protected override PropDefinition getField()
		{
			return Definition;
		}
	}
}
