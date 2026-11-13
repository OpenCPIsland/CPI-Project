using ClubPenguin.Locomotion;

namespace ClubPenguin.Actions
{
	public class ToggleCoreGameplayAction : Action
	{
		public bool UserControls = true;

		public bool Steering = true;

		public bool Collisions = true;

		public bool Gravity = true;

		public bool Controller = true;

		public bool AbortQueuedLoco = true;

		protected override void CopyTo(Action _destWarper)
		{
			ToggleCoreGameplayAction toggleCoreGameplayAction = _destWarper as ToggleCoreGameplayAction;
			toggleCoreGameplayAction.UserControls = UserControls;
			toggleCoreGameplayAction.Steering = Steering;
			toggleCoreGameplayAction.Collisions = Collisions;
			toggleCoreGameplayAction.Gravity = Gravity;
			toggleCoreGameplayAction.AbortQueuedLoco = AbortQueuedLoco;
			toggleCoreGameplayAction.Controller = Controller;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			if (LocomotionHelper.IsCurrentControllerOfType<RunController>(GetTarget()))
			{
				RunController component = GetTarget().GetComponent<RunController>();
				component.Behaviour = new RunController.ControllerBehaviour
				{
					IgnoreTranslation = !Controller,
					IgnoreCollisions = !Collisions,
					IgnoreRotation = !Steering,
					IgnoreGravity = !Gravity
				};
				if (!Gravity)
				{
					component.ResetMomentum();
				}
			}
			PenguinUserControl component2 = GetTarget().GetComponent<PenguinUserControl>();
			if (component2 != null)
			{
				component2.enabled = UserControls;
			}
			Completed();
		}
	}
}
