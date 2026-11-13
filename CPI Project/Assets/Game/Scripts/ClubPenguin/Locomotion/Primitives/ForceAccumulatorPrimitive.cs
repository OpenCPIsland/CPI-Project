using UnityEngine;

namespace ClubPenguin.Locomotion.Primitives
{
	public class ForceAccumulatorPrimitive : LocomotionPrimitive
	{
		private ForceAccumulatorPrimitiveData mutableData;

		private Vector3 wsForce;

		private float gravityStep;

		public override void ResetState()
		{
			wsForce = Vector3.zero;
			base.ResetState();
		}

		public void SetData(ForceAccumulatorPrimitiveData data)
		{
			mutableData = Object.Instantiate(data);
			gravityStep = mutableData.Gravity * Time.fixedDeltaTime;
		}

		public void AddForce(Vector3 _wsForce)
		{
			wsForce += _wsForce;
			Output.wsVelocity = wsForce;
		}

		public void SetForce(Vector3 _wsForce)
		{
			wsForce = _wsForce;
			Output.wsVelocity = wsForce;
		}

		private void FixedUpdate()
		{
			if (mutableData.DragSmoothing > 0f)
			{
				wsForce = Vector3.Lerp(wsForce, Vector3.zero, mutableData.DragSmoothing * Time.fixedDeltaTime);
			}
			wsForce.y -= gravityStep;
			Output.wsVelocity = wsForce;
		}
	}
}
