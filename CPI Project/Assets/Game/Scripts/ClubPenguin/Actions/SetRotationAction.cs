using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin.Actions
{
	[RequireComponent(typeof(MotionTracker))]
	public class SetRotationAction : Action
	{
		public Transform TargetRotation;

		public bool Loop;

		public bool SetHeadingOnExit = true;

		public bool BroadcastOnExit = false;

		private MotionTracker motionTracker;

		private RunController runController;

		private LocomotionEventBroadcaster broadcaster;

		private bool isUsingControllerVelocity;

		private GameObject actionTarget;

		protected override void CopyTo(Action _destWarper)
		{
			SetRotationAction setRotationAction = _destWarper as SetRotationAction;
			setRotationAction.TargetRotation = TargetRotation;
			setRotationAction.Loop = Loop;
			setRotationAction.SetHeadingOnExit = SetHeadingOnExit;
			setRotationAction.BroadcastOnExit = BroadcastOnExit;
			base.CopyTo(_destWarper);
		}

		protected override void OnEnable()
		{
			actionTarget = GetTarget();
			motionTracker = actionTarget.GetComponent<MotionTracker>();
			runController = actionTarget.GetComponent<RunController>();
			broadcaster = actionTarget.GetComponent<LocomotionEventBroadcaster>();
			base.OnEnable();
		}

		public override void Completed(object userData = null, bool conditionBranchValue = true)
		{
			if (base.enabled)
			{
				if (SetHeadingOnExit && motionTracker != null)
				{
					Vector3 velocity = motionTracker.Velocity;
					if (isUsingControllerVelocity)
					{
						Vector3 forward = actionTarget.transform.forward * Mathf.Sign(actionTarget.transform.up.y);
						forward.y = 0f;
						forward.Normalize();
						actionTarget.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
					}
					else if (Mathf.Abs(Vector3.Dot(velocity.normalized, Vector3.up)) > 0.707f)
					{
						actionTarget.transform.rotation = Quaternion.LookRotation(actionTarget.transform.forward, Vector3.up);
					}
					else
					{
						actionTarget.transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
					}
				}
				if (BroadcastOnExit && broadcaster != null)
				{
					broadcaster.BroadcastOnSteerRotationDirectionEvent(actionTarget.transform.forward);
					broadcaster.BroadcastOnSteerRotationFlushEvent();
				}
			}
			base.Completed(userData);
		}

		protected override void Update()
		{
			if (TargetRotation != null)
			{
				actionTarget.transform.rotation = TargetRotation.rotation;
			}
			else if (IncomingUserData != null && IncomingUserData.GetType() == typeof(Vector3))
			{
				Vector3 vector = (Vector3)IncomingUserData;
				vector.y = 0f;
				if (vector == Vector3.zero)
				{
					vector = actionTarget.transform.forward;
				}
				actionTarget.transform.rotation = Quaternion.LookRotation(vector);
			}
			else if (motionTracker != null)
			{
				Vector3 normalized = motionTracker.FrameVelocity.normalized;
				if (normalized != Vector3.zero)
				{
					Vector3 zero = Vector3.zero;
					Vector3 zero2 = Vector3.zero;
					zero = ((!(normalized != Vector3.up) || !(normalized != Vector3.down)) ? Vector3.Cross(Vector3.back, normalized) : Vector3.Cross(Vector3.up, normalized));
					zero2 = Vector3.Cross(zero, normalized);
					if (runController != null)
					{
						runController.SnapToFacing(Quaternion.LookRotation(zero2, normalized) * Vector3.forward, normalized);
					}
					else
					{
						actionTarget.transform.rotation = Quaternion.LookRotation(zero2, normalized);
					}
				}
				isUsingControllerVelocity = true;
			}
			if (!Loop)
			{
				Completed();
			}
		}
	}
}
