using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct ReceivedChatMessage
	{
		public long senderSessionId;

		public string message;

		public int emotion;
	}
}
