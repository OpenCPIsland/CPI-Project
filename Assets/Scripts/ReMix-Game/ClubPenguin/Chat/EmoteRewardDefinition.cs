using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Chat
{
	[Serializable]
	public class EmoteRewardDefinition : AbstractStaticGameDataRewardDefinition<EmoteDefinition>
	{
		public EmoteDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new EmoteReward(Definition.Id);
			}
		}

		protected override EmoteDefinition getField()
		{
			return Definition;
		}
	}
}
