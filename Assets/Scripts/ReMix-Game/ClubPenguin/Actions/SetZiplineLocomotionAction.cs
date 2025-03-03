using ClubPenguin.Locomotion;
using Disney.LaunchPadFramework;

namespace ClubPenguin.Actions
{
	public class SetZiplineLocomotionAction : Action
	{
		protected override void CopyTo(Action _destAction)
		{
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			if (!LocomotionHelper.SetCurrentController<ZiplineController>(GetTarget()))
			{
				Log.LogError(this, "Failed to set the ZiplineController");
			}
			Completed();
		}
	}
}
