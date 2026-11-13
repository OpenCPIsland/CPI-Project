using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using System;

namespace ClubPenguin.Consumable
{
	[Serializable]
	public class ConsumableRewardDefinition : AbstractStaticGameDataRewardDefinition<PropDefinition>
	{
		public PropDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new ConsumableReward(Definition.Id);
			}
		}

		protected override PropDefinition getField()
		{
			return Definition;
		}
	}
}
