namespace ClubPenguin.Input
{
	public class NavBarButtonsInputMap : InputMap<NavBarButtonsInputMap.Result>
	{
		public class Result
		{
			public readonly ButtonInputResult Profile = new ButtonInputResult();

			public readonly ButtonInputResult Consumables = new ButtonInputResult();

			public readonly ButtonInputResult Quest = new ButtonInputResult();

			public readonly ButtonInputResult Map = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			bool flag = controlScheme.Profile.ProcessInput(mapResult.Profile);
			flag = (controlScheme.Consumables.ProcessInput(mapResult.Consumables) || flag);
			flag = (controlScheme.Quest.ProcessInput(mapResult.Quest) || flag);
			return controlScheme.Map.ProcessInput(mapResult.Map) || flag;
		}
	}
}
