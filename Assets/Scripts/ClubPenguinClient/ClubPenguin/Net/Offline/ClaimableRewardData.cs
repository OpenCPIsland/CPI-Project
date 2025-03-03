using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct ClaimableRewardData : IOfflineData
	{
		public List<int> ClimedRewards;

		public void Init()
		{
			ClimedRewards = new List<int>();
		}
	}
}
