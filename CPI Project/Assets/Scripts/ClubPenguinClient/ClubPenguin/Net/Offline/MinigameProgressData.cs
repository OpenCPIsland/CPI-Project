using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct MinigameProgressData : IOfflineData
	{
		public long LastResetDateInMilliseconds;

		public Dictionary<string, int> PlayCounts;

		public void Init()
		{
			LastResetDateInMilliseconds = 0L;
			PlayCounts = new Dictionary<string, int>();
		}
	}
}
