namespace ClubPenguin.Input
{
	public class HorizontalArrowsInputMap : InputMap<HorizontalArrowsInputMap.Result>
	{
		private readonly ButtonInputResult tmpLeft = new ButtonInputResult();
		private readonly ButtonInputResult tmpRight = new ButtonInputResult();

		public class Result
		{
			public readonly ButtonInputResult Left = new ButtonInputResult();

			public readonly ButtonInputResult Right = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			bool left = controlScheme.Left.ProcessInput(mapResult.Left);
			bool right = controlScheme.Right.ProcessInput(mapResult.Right);

			tmpLeft.Reset();
			tmpRight.Reset();

			bool navLeft = controlScheme.UI_NavigationBackwards.ProcessInput(tmpLeft);
			bool navRight = controlScheme.UI_Navigation.ProcessInput(tmpRight);

			if (navLeft)
			{
				mapResult.Left.IsHeld |= tmpLeft.IsHeld;
				mapResult.Left.WasJustPressed |= tmpLeft.WasJustPressed;
				mapResult.Left.WasJustReleased |= tmpLeft.WasJustReleased;
			}

			if (navRight)
			{
				mapResult.Right.IsHeld |= tmpRight.IsHeld;
				mapResult.Right.WasJustPressed |= tmpRight.WasJustPressed;
				mapResult.Right.WasJustReleased |= tmpRight.WasJustReleased;
			}

			return left || right || navLeft || navRight;
		}
	}
}
