using ClubPenguin.Locomotion;

namespace ClubPenguin.Actions
{
	public class SetSwimLocomotionAction : Action
	{
		protected override void CopyTo(Action _destAction)
		{
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			LocomotionHelper.SetCurrentController<SwimController>(GetTarget());
			Completed();
		}
	}
}
