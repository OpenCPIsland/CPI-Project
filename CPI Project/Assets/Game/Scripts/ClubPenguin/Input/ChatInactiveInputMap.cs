namespace ClubPenguin.Input
{
	public class ChatInactiveInputMap : InputMap<ChatInactiveInputMap.Result>
	{
		public class Result
		{
			public readonly ButtonInputResult Chat = new ButtonInputResult();

			public readonly ButtonInputResult QuickEmote = new ButtonInputResult();

			public readonly ButtonInputResult QuickChat = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			bool flag = controlScheme.Chat.ProcessInput(mapResult.Chat);
			flag = (controlScheme.QuickEmote.ProcessInput(mapResult.QuickEmote) || flag);
			return controlScheme.QuickChat.ProcessInput(mapResult.QuickChat) || flag;
		}
	}
}
