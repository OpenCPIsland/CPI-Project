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

		public struct CachedProfile
		{
			public string Swid;

			public string Name;

			public string OutfitJson;

			public int? Colour;

			public int? DaysOld;

			public Dictionary<string, long> MascotXP;

			public string IglooName;

			public string IglooInstance;

			public string Visibility;
		}

		public List<Entry> Friends;

		public List<Entry> Incoming;

		public List<Entry> Outgoing;

		public List<CachedProfile> Profiles;

		public void Init()
		{
			Friends = new List<Entry>();
			Incoming = new List<Entry>();
			Outgoing = new List<Entry>();
			Profiles = new List<CachedProfile>();
		}
	}
}
