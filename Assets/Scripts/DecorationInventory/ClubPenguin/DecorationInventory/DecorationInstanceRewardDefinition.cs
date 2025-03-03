using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	public class DecorationInstanceRewardDefinition : AbstractStaticGameDataRewardDefinition<DecorationDefinition>
	{
		public DecorationDefinition Decoration;

		public int Count;

		public override IRewardable Reward
		{
			get
			{
				return new DecorationInstanceReward(Decoration.Id, Count);
			}
		}

		protected override DecorationDefinition getField()
		{
			return Decoration;
		}
	}
}
