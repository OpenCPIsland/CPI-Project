namespace ClubPenguin.Input
{
	public class CellphoneButtonInputMap : InputMap<CellphoneButtonInputMap.Result>
	{
		public class Result
		{
			public readonly ButtonInputResult Cellphone = new ButtonInputResult();
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			return controlScheme.Cellphone.ProcessInput(mapResult.Cellphone);
		}
	}
}
