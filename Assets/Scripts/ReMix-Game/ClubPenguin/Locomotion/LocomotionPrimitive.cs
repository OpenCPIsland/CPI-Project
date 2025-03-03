using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class LocomotionPrimitive : MonoBehaviour
	{
		public struct PrimitiveOutput
		{
			public Vector3 wsDeltaPos;

			public Quaternion wsRotation;

			public Vector3 wsVelocity;

			public void Reset()
			{
				wsDeltaPos = Vector3.zero;
				wsRotation = Quaternion.identity;
				wsVelocity = Vector3.zero;
			}
		}

		protected PrimitiveOutput Output;

		public bool IsFinished
		{
			get;
			protected set;
		}

		public virtual void ResetState()
		{
			Output.Reset();
		}

		public virtual void Steer(Vector2 steerInput)
		{
		}

		public virtual void Steer(Vector3 wsForce)
		{
		}

		public virtual void SteerRotation(Vector2 steerInput)
		{
		}

		public virtual void SteerRotation(Vector3 wsSteerInput)
		{
		}

		public virtual PrimitiveOutput GetOutput()
		{
			return Output;
		}
	}
}
