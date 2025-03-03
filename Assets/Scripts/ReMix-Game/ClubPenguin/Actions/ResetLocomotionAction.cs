using ClubPenguin.Locomotion;

namespace ClubPenguin.Actions
{
	public class ResetLocomotionAction : Action
	{
		protected override void CopyTo(Action _destAction)
		{
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			LocomotionHelper.SetCurrentController<RunController>(GetTarget());
			Completed();
		}
	}
}
