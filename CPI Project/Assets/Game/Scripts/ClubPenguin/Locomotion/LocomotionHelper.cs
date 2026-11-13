using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public static class LocomotionHelper
	{
		public static LocomotionController GetCurrentController(GameObject target)
		{
			LocomotionTracker component = target.GetComponent<LocomotionTracker>();
			if (component != null)
			{
				return component.GetCurrentController();
			}
			return null;
		}

		public static bool SetCurrentController<T>(GameObject target) where T : LocomotionController
		{
			LocomotionTracker component = target.GetComponent<LocomotionTracker>();
			if (component != null)
			{
				return component.SetCurrentController<T>();
			}
			return false;
		}

		public static bool IsCurrentControllerOfType<T>(GameObject target) where T : LocomotionController
		{
			LocomotionTracker component = target.GetComponent<LocomotionTracker>();
			if (component != null)
			{
				return component.IsCurrentControllerOfType<T>();
			}
			return false;
		}
	}
}
