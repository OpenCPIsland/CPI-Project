namespace ClubPenguin.Input
{
	public class BackControllerInputMap : InputMap<BackControllerInputMap.Result>
	{
		public class Result
		{
			public readonly ButtonInputResult Back = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			return controlScheme.Back.ProcessInput(mapResult.Back);
		}
	}
}
