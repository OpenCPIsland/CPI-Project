using ClubPenguin.Locomotion;

namespace ClubPenguin.Actions
{
	public class DisallowLocomotionControllerAction : Action
	{
		public float RunDelay;

		public float SwimDelay;

		public float SlideDelay;

		public float SitDelay;

		protected override void CopyTo(Action _destWarper)
		{
			DisallowLocomotionControllerAction disallowLocomotionControllerAction = _destWarper as DisallowLocomotionControllerAction;
			disallowLocomotionControllerAction.RunDelay = RunDelay;
			disallowLocomotionControllerAction.SwimDelay = SwimDelay;
			disallowLocomotionControllerAction.SlideDelay = SlideDelay;
			disallowLocomotionControllerAction.SitDelay = SitDelay;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			LocomotionTracker component = GetTarget().GetComponent<LocomotionTracker>();
			if (component != null)
			{
				if (RunDelay > 0f)
				{
					component.DisallowController<RunController>(RunDelay);
				}
				if (SwimDelay > 0f)
				{
					component.DisallowController<SwimController>(SwimDelay);
				}
				if (SlideDelay > 0f)
				{
					component.DisallowController<SlideController>(SlideDelay);
				}
				if (SitDelay > 0f)
				{
					component.DisallowController<SitController>(SitDelay);
				}
			}
			Completed();
		}
	}
}
