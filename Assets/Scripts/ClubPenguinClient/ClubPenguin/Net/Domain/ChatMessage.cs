namespace ClubPenguin.Net.Domain
{
	public struct ChatMessage
	{
		public long senderSessionId;

		public string message;

		public int emotion;

		public bool moderated;

		public string questId;

		public string objective;
	}
}
