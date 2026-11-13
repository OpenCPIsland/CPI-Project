using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct PartyGameMessageEvent
	{
		public int sessionId;

		public int type;

		public string message;
	}
}
