namespace ClubPenguin.Input
{
	public class HorizontalArrowsInputMap : InputMap<HorizontalArrowsInputMap.Result>
	{
		public class Result
		{
			public readonly ButtonInputResult Left = new ButtonInputResult();

			public readonly ButtonInputResult Right = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			bool flag = controlScheme.Left.ProcessInput(mapResult.Left);
			return controlScheme.Right.ProcessInput(mapResult.Right) || flag;
		}
	}
}
