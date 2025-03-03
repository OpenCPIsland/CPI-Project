using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Tubes
{
	[Serializable]
	public class TubeRewardDefinition : AbstractStaticGameDataRewardDefinition<TubeDefinition>
	{
		public TubeDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new TubeReward(Definition.Id);
			}
		}

		protected override TubeDefinition getField()
		{
			return Definition;
		}
	}
}
