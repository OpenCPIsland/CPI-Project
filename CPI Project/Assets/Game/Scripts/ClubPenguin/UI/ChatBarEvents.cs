namespace ClubPenguin.UI
{
	public static class ChatBarEvents
	{
		public struct ChatBarStateChanged
		{
			public readonly ChatBarState ChatBarState;

			public ChatBarStateChanged(ChatBarState chatBarState)
			{
				ChatBarState = chatBarState;
			}
		}
	}
}
