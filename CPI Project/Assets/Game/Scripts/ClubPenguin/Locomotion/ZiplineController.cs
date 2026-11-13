using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(MotionTracker))]
	public class ZiplineController : LocomotionController
	{
		private void OnEnable()
		{
			base.Broadcaster.BroadcastOnControlsLocked();
		}

		private void OnDisable()
		{
			base.Broadcaster.BroadcastOnControlsUnLocked();
		}

		public override void Steer(Vector2 steerInput)
		{
		}

		public override void Steer(Vector3 wsSteerInput)
		{
		}

		public override void DoAction(LocomotionAction action, object userData = null)
		{
		}

		public override bool AllowTriggerInteractions()
		{
			return false;
		}

		public override bool AllowTriggerOnStay()
		{
			return false;
		}
	}
}
