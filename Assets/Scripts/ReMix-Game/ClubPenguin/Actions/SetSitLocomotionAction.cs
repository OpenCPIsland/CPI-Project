using ClubPenguin.Locomotion;

namespace ClubPenguin.Actions
{
	public class SetSitLocomotionAction : Action
	{
		protected override void CopyTo(Action _destAction)
		{
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			ChairProperties component = SceneRefs.ActionSequencer.GetTrigger(GetTarget()).GetComponent<ChairProperties>();
			if (component != null)
			{
				LocomotionHelper.SetCurrentController<SitController>(GetTarget());
				LocomotionController currentController = LocomotionHelper.GetCurrentController(GetTarget());
				if (currentController is SitController)
				{
					SitController sitController = (SitController)currentController;
					sitController.SetChair(component);
				}
			}
			Completed();
		}
	}
}
