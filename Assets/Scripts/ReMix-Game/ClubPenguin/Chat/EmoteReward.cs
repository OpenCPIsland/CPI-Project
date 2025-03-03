using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Chat
{
	[Serializable]
	public class EmoteReward : AbstractListReward<string>
	{
		public List<string> Emotes
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
				return "emotePacks";
			}
		}

		public EmoteReward()
		{
		}

		public EmoteReward(string value)
			: base(value)
		{
		}
	}
}
