using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Collectibles
{
	[Serializable]
	public class CollectibleRewardDefinition : AbstractStaticGameDataRewardDefinition<CollectibleDefinition>
	{
		public CollectibleDefinition Collectible;

		public int Amount;

		public override IRewardable Reward
		{
			get
			{
				return new CollectibleReward(Collectible.CollectibleType, Amount);
			}
		}

		protected override CollectibleDefinition getField()
		{
			return Collectible;
		}
	}
}
