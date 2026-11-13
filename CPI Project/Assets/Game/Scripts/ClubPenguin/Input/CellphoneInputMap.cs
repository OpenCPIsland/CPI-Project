namespace ClubPenguin.Input
{
	public class CellphoneInputMap : InputMap<CellphoneInputMap.Result>
	{
		public class Result
		{
			public readonly ButtonInputResult Back = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			return controlScheme.MergedBackCellphone.ProcessInput(mapResult.Back);
		}
	}
}
