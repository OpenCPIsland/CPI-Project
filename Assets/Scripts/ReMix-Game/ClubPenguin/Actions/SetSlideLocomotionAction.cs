using ClubPenguin.Locomotion;
using Disney.LaunchPadFramework;

namespace ClubPenguin.Actions
{
	public class SetSlideLocomotionAction : Action
	{
		protected override void CopyTo(Action _destAction)
		{
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			if (!LocomotionHelper.SetCurrentController<SlideController>(GetTarget()))
			{
				Log.LogError(this, "Failed to set the SlideController");
			}
			Completed();
		}
	}
}
