using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	public class ClaimServerAddedRewardsResponse : CPResponse
	{
		public List<ClaimedServerAddedReward> claimedRewards;
	}
}
