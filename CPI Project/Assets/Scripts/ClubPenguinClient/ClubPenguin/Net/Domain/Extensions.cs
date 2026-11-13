namespace ClubPenguin.Net.Domain
{
	public static class Extensions
	{
		public static bool IsMovement(this LocomotionAction locomotionAction)
		{
			switch (locomotionAction)
			{
			case LocomotionAction.Move:
			case LocomotionAction.Jump:
			case LocomotionAction.Torpedo:
			case LocomotionAction.SlideTrick:
				return true;
			default:
				return false;
			}
		}
	}
}
