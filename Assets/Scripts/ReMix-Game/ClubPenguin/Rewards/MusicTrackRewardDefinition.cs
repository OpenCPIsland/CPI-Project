using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Rewards
{
	[Serializable]
	public class MusicTrackRewardDefinition : AbstractStaticGameDataRewardDefinition<MusicTrackDefinition>
	{
		public MusicTrackDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new MusicTrackReward(Definition.Id);
			}
		}

		protected override MusicTrackDefinition getField()
		{
			return Definition;
		}
	}
}
