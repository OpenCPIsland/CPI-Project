using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.NPC
{
	[Serializable]
	public class MascotXPReward : AbstractDictionaryReward<string, int>
	{
		public Dictionary<string, int> XP
		{
			get
			{
				return data;
			}
		}

		public override string RewardType
		{
			get
			{
				return "mascotXP";
			}
		}

		public MascotXPReward()
		{
		}

		public MascotXPReward(string key, int value)
			: base(key, value)
		{
		}

		protected override int combineValues(int val1, int val2)
		{
			return val1 + val2;
		}
	}
}
