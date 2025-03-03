namespace ClubPenguin.Input
{
	public class NavBarSecondaryInputMap : InputMap<NavBarSecondaryInputMap.Result>
	{
		public class Result
		{
			public readonly LocomotionInputResult Locomotion = new LocomotionInputResult();

			public readonly ButtonInputResult Close = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			bool flag = controlScheme.Locomotion.ProcessInput(mapResult.Locomotion);
			return controlScheme.Cancel.ProcessInput(mapResult.Close) || flag;
		}
	}
}
