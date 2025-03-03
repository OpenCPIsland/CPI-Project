namespace ClubPenguin.Input
{
	public class WorldMapControllerInputMap : InputMap<WorldMapControllerInputMap.Result>
	{
		public class Result
		{
			public readonly ButtonInputResult Back = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			return controlScheme.MergedBackMap.ProcessInput(mapResult.Back);
		}
	}
}
