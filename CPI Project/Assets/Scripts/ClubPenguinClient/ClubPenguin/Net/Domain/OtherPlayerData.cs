using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class OtherPlayerData
	{
		public PlayerId id;

		public string name;

		public RoomIdentifier onlineLocation;

		public bool member;

		public List<CustomEquipment> outfit;

		public Profile profile;

		public Dictionary<string, long> mascotXP;

		public ZoneId zoneId;
	}
}
