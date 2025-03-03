using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Rewards
{
	[Serializable]
	public class MusicTrackReward : AbstractListReward<int>
	{
		public List<int> MusicTracks
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
				return "musicTracks";
			}
		}

		public MusicTrackReward()
		{
		}

		public MusicTrackReward(int value)
			: base(value)
		{
		}
	}
}
