using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct PartyGameStartEvent
	{
		public long owner;

		public long[] players;

		public int sessionId;

		public int templateId;
	}
}
