namespace ClubPenguin.Input
{
	public class UINavigationInputMap : InputMap<UINavigationInputMap.Result>
	{
		public class Result
		{
			public readonly ButtonInputResult Navigate = new ButtonInputResult();

			public readonly ButtonInputResult NavigateBackwards = new ButtonInputResult();

			public readonly ButtonInputResult Submit = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			bool flag = controlScheme.UI_Navigation.ProcessInput(mapResult.Navigate);
			controlScheme.UI_NavigationBackwards.ProcessInput(mapResult.NavigateBackwards);
			return controlScheme.UI_Submit.ProcessInput(mapResult.Submit) || flag;
		}
	}
}
