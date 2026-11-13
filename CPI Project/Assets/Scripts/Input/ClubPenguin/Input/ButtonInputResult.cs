namespace ClubPenguin.Input
{
	public class ButtonInputResult : InputResult<ButtonInputResult>
	{
		public bool IsHeld;

		public bool WasJustPressed;

		public bool WasJustReleased;

		public override void CopyTo(ButtonInputResult copyToInputResult)
		{
			copyToInputResult.IsHeld = IsHeld;
			copyToInputResult.WasJustPressed = WasJustPressed;
			copyToInputResult.WasJustReleased = WasJustReleased;
		}

		public override void Reset()
		{
			IsHeld = false;
			WasJustPressed = false;
			WasJustReleased = false;
		}
	}
}
