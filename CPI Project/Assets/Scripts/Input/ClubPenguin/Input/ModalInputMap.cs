namespace ClubPenguin.Input
{
	public class ModalInputMap : InputMap<ModalInputMap.Result>
	{
		public class Result
		{
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			return false;
		}
	}
}
