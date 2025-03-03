using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class ImpulseAction : Action
	{
		public Transform StartTransform;

		public Transform DirectionY;

		public Transform DirectionZ;

		public Transform DirectionTo;

		public Vector3 Direction;

		public float Magnitude;

		public Vector3 AddDirection;

		public float AddMagnitude;

		public bool SetRunLocomotion;

		public bool PreserveTubing;

		protected override void CopyTo(Action _destWarper)
		{
			ImpulseAction impulseAction = _destWarper as ImpulseAction;
			impulseAction.StartTransform = StartTransform;
			impulseAction.DirectionY = DirectionY;
			impulseAction.DirectionZ = DirectionZ;
			impulseAction.DirectionTo = DirectionTo;
			impulseAction.Direction = Direction;
			impulseAction.Magnitude = Magnitude;
			impulseAction.AddDirection = AddDirection;
			impulseAction.AddMagnitude = AddMagnitude;
			impulseAction.SetRunLocomotion = SetRunLocomotion;
			impulseAction.PreserveTubing = PreserveTubing;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			GameObject target = GetTarget();
			if (target != null)
			{
				Vector3 a = base.transform.forward;
				if (IncomingUserData != null && IncomingUserData.GetType() == typeof(Vector3))
				{
					a = ((Vector3)IncomingUserData).normalized;
				}
				else if (DirectionY != null)
				{
					a = DirectionY.up;
				}
				else if (DirectionZ != null)
				{
					a = DirectionZ.forward;
				}
				else if (DirectionTo != null)
				{
					a = (DirectionTo.position - target.transform.position).normalized;
				}
				else if (Direction != Vector3.zero)
				{
					a = Direction;
				}
				Vector3 vector = a * Magnitude + AddDirection * AddMagnitude;
				bool flag = LocomotionHelper.IsCurrentControllerOfType<SlideController>(target);
				Rigidbody component = target.GetComponent<Rigidbody>();
				if (!flag && component != null)
				{
					if (StartTransform != null)
					{
						component.transform.position = StartTransform.position;
						component.transform.rotation = StartTransform.rotation;
						component.velocity = Vector3.zero;
						component.angularVelocity = Vector3.zero;
						component.WakeUp();
					}
					component.AddForce(vector, ForceMode.VelocityChange);
				}
				else
				{
					if (SetRunLocomotion && (!PreserveTubing || !flag))
					{
						LocomotionHelper.SetCurrentController<RunController>(target);
						flag = false;
					}
					if (!flag && StartTransform != null)
					{
						target.transform.position = StartTransform.position;
						target.transform.rotation = StartTransform.rotation;
					}
					LocomotionController currentController = LocomotionHelper.GetCurrentController(target);
					if (currentController != null)
					{
						if (currentController is RunController)
						{
							((RunController)currentController).ClearAllVelocityInputs();
						}
						currentController.SetForce(vector, base.gameObject);
					}
				}
			}
			Completed();
		}
	}
}
