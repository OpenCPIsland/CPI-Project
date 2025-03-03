using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct InRoomRewards
	{
		public string room;

		public Dictionary<string, long> collected;
	}
}
