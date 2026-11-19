using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct LanFriendsData : IOfflineData
	{
		public struct Entry
		{
			public string Swid;

			public string Name;

			public long Id;
		}

		public List<Entry> Friends;

		public List<Entry> Incoming;

		public List<Entry> Outgoing;

		public void Init()
		{
			Friends = new List<Entry>();
			Incoming = new List<Entry>();
			Outgoing = new List<Entry>();
		}
	}
}
