using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct PartyGameEndEvent
	{
		public Dictionary<long, int> playerSessionIdToPlacement;

		public int sessionId;
	}
}
