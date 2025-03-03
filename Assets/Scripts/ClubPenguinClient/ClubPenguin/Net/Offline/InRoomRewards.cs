using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct InRoomRewards : IOfflineData
	{
		public struct InRoomReward
		{
			public string Room;

			public Dictionary<string, long> Collected;
		}

		public List<InRoomReward> Collected;

		public void Init()
		{
			Collected = new List<InRoomReward>();
		}
	}
}
