namespace ClubPenguin.Input
{
	public class PenguinControlsInputMap : InputMap<PenguinControlsInputMap.Result>
	{
		public class Result
		{
			public readonly LocomotionInputResult Locomotion = new LocomotionInputResult();

			public readonly ButtonInputResult Jump = new ButtonInputResult();

			public readonly ButtonInputResult Action1 = new ButtonInputResult();

			public readonly ButtonInputResult Action2 = new ButtonInputResult();

			public readonly ButtonInputResult Action3 = new ButtonInputResult();

			public readonly ButtonInputResult Cancel = new ButtonInputResult();

			public readonly ButtonInputResult WalkModifier = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			controlScheme.Locomotion.ProcessInput(mapResult.Locomotion);
			controlScheme.Jump.ProcessInput(mapResult.Jump);
			controlScheme.Action1.ProcessInput(mapResult.Action1);
			controlScheme.Action2.ProcessInput(mapResult.Action2);
			controlScheme.Action3.ProcessInput(mapResult.Action3);
			controlScheme.Cancel.ProcessInput(mapResult.Cancel);
			controlScheme.WalkModifier.ProcessInput(mapResult.WalkModifier);
			return true;
		}
	}
}
