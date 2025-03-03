namespace ClubPenguin.Input
{
	public class PenguinControlsChatInputMap : InputMap<PenguinControlsChatInputMap.Result>
	{
		public class Result
		{
			public readonly LocomotionInputResult Locomotion = new LocomotionInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			controlScheme.Locomotion.ProcessInput(mapResult.Locomotion);
			return true;
		}
	}
}
