namespace ClubPenguin.Input
{
	public class PromptControllerInputMap : InputMap<PromptControllerInputMap.Result>
	{
		public class Result
		{
			public readonly ButtonInputResult Accept = new ButtonInputResult();

			public readonly ButtonInputResult Cancel = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			bool flag = controlScheme.UI_Accept.ProcessInput(mapResult.Accept);
			return controlScheme.UI_Cancel.ProcessInput(mapResult.Cancel) || flag;
		}
	}
}
