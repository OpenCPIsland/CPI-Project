using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class FollowAction : Action
	{
		public Transform PilotTransform;

		public bool SetPosition = true;

		public bool SetRotation = true;

		public bool UseVelAsFacing;

		public bool IgnoreVelY;

		private RunController runController;

		private Transform thisTransform;

		private Transform pilot;

		private Vector3 prevPos;

		protected override void CopyTo(Action _destWarper)
		{
			FollowAction followAction = _destWarper as FollowAction;
			followAction.PilotTransform = PilotTransform;
			followAction.SetPosition = SetPosition;
			followAction.SetRotation = SetRotation;
			followAction.UseVelAsFacing = UseVelAsFacing;
			followAction.IgnoreVelY = IgnoreVelY;
			base.CopyTo(_destWarper);
		}

		protected override void OnEnable()
		{
			runController = GetComponent<RunController>();
			thisTransform = base.transform;
			prevPos = thisTransform.position;
			if (runController != null)
			{
				runController.ResetState();
				runController.Behaviour = new RunController.ControllerBehaviour
				{
					IgnoreCollisions = true,
					IgnoreRotation = true,
					IgnoreGravity = true,
					IgnoreTranslation = true
				};
			}
			pilot = PilotTransform;
			if (pilot == null && IncomingUserData != null && IncomingUserData.GetType() == typeof(GameObject))
			{
				pilot = ((GameObject)IncomingUserData).transform;
			}
			base.OnEnable();
		}

		public override void Completed(object userData = null, bool conditionBranchValue = true)
		{
			if (runController != null)
			{
				runController.Behaviour.Reset();
			}
			base.Completed(userData);
		}

		protected override void Update()
		{
			if (!(pilot != null))
			{
				return;
			}
			Vector3 vector = thisTransform.forward;
			if (SetPosition)
			{
				Vector3 position = pilot.position;
				if (prevPos != position)
				{
					vector = position - prevPos;
					prevPos = position;
				}
				thisTransform.position = position;
			}
			if (SetRotation)
			{
				thisTransform.rotation = pilot.rotation;
			}
			else if (UseVelAsFacing)
			{
				if (IgnoreVelY)
				{
					vector.y = 0f;
				}
				if (vector != Vector3.zero)
				{
					thisTransform.rotation = Quaternion.LookRotation(vector);
				}
			}
		}
	}
}
