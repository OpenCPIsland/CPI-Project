namespace ClubPenguin
{
	public struct DChatMessage
	{
		public readonly long PlayerId;

		public readonly string Message;

		public readonly int SizzleClipID;

		public DChatMessage(long playerId, string message, int sizzleClipID)
		{
			PlayerId = playerId;
			Message = message;
			SizzleClipID = sizzleClipID;
		}
	}
}
