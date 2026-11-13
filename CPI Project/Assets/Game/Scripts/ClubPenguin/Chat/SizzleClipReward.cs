using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Chat
{
	[Serializable]
	public class SizzleClipReward : AbstractListReward<int>
	{
		public List<int> SizzleClips
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
				return "sizzleClips";
			}
		}

		public SizzleClipReward()
		{
		}

		public SizzleClipReward(int value)
			: base(value)
		{
		}
	}
}
