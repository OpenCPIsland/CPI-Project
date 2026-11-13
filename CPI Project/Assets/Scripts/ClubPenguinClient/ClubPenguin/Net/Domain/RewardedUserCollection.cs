using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	public class RewardedUserCollection
	{
		public RewardSource source;

		public string sourceId;

		public Dictionary<long, Reward> rewards;
	}
}
