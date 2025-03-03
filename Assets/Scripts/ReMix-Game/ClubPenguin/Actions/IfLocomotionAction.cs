using ClubPenguin.Locomotion;

namespace ClubPenguin.Actions
{
	public class IfLocomotionAction : Action
	{
		public bool Sitting;

		public bool Tubing;

		public bool Swimming;

		public bool Running;

		protected override void CopyTo(Action _destWarper)
		{
			IfLocomotionAction ifLocomotionAction = _destWarper as IfLocomotionAction;
			ifLocomotionAction.Sitting = Sitting;
			ifLocomotionAction.Tubing = Tubing;
			ifLocomotionAction.Swimming = Swimming;
			ifLocomotionAction.Running = Running;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			LocomotionTracker component = GetTarget().GetComponent<LocomotionTracker>();
			if (component == null)
			{
				Completed(null, false);
			}
			else if (Sitting && component.IsCurrentControllerOfType<SitController>())
			{
				Completed();
			}
			else if (Tubing && component.IsCurrentControllerOfType<SlideController>())
			{
				Completed();
			}
			else if (Swimming && component.IsCurrentControllerOfType<SwimController>())
			{
				Completed();
			}
			else if (Running && component.IsCurrentControllerOfType<RunController>())
			{
				Completed();
			}
			else
			{
				Completed(null, false);
			}
		}
	}
}
