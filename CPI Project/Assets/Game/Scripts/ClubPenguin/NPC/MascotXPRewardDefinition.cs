using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.NPC
{
	[Serializable]
	public class MascotXPRewardDefinition : AbstractStaticGameDataRewardDefinition<MascotDefinition>
	{
		public MascotDefinition Mascot;

		public int XP;

		public override IRewardable Reward
		{
			get
			{
				return new MascotXPReward(Mascot.name, XP);
			}
		}

		protected override MascotDefinition getField()
		{
			return Mascot;
		}
	}
}
