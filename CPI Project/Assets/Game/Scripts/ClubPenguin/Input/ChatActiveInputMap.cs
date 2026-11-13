namespace ClubPenguin.Input
{
	public class ChatActiveInputMap : InputMap<ChatActiveInputMap.Result>
	{
		public class Result
		{
			public readonly ButtonInputResult Send = new ButtonInputResult();

			public readonly ButtonInputResult Back = new ButtonInputResult();

			public readonly ButtonInputResult QuickChat = new ButtonInputResult();

			public readonly ButtonInputResult QuickEmote = new ButtonInputResult();

			public readonly LocomotionInputResult Locomotion = new LocomotionInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			bool flag = controlScheme.Locomotion.ProcessInput(mapResult.Locomotion);
			flag = (controlScheme.Back.ProcessInput(mapResult.Back) || flag);
			flag = (controlScheme.SendChat.ProcessInput(mapResult.Send) || flag);
			flag = (controlScheme.QuickChat.ProcessInput(mapResult.QuickChat) || flag);
			return controlScheme.QuickEmote.ProcessInput(mapResult.QuickEmote) || flag;
		}
	}
}
