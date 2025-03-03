using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using System;

namespace ClubPenguin.Consumable
{
	[Serializable]
	public class ConsumableInstanceRewardDefinition : AbstractStaticGameDataRewardDefinition<PropDefinition>
	{
		public PropDefinition Consumable;

		public int Count;

		public override IRewardable Reward
		{
			get
			{
				return new ConsumableInstanceReward(Consumable.NameOnServer, Count);
			}
		}

		protected override PropDefinition getField()
		{
			return Consumable;
		}
	}
}
