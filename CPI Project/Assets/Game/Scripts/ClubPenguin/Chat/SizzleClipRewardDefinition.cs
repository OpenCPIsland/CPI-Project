using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using System;

namespace ClubPenguin.Chat
{
	[Serializable]
	public class SizzleClipRewardDefinition : AbstractStaticGameDataRewardDefinition<SizzleClipDefinition>
	{
		public SizzleClipDefinition Definition;

		public override IRewardable Reward
		{
			get
			{
				return new SizzleClipReward(Definition.Id);
			}
		}

		protected override SizzleClipDefinition getField()
		{
			return Definition;
		}
	}
}
