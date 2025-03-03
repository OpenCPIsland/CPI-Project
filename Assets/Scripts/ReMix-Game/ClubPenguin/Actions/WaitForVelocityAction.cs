using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class WaitForVelocityAction : Action
	{
		public Vector3 Min = default(Vector3);

		public Vector3 Max = default(Vector3);

		public float MinMag;

		public float MaxMag;

		public bool IgnoreX;

		public bool IgnoreY;

		public bool IgnoreZ;

		public bool IgnoreMinMag = true;

		public bool IgnoreMaxMag = true;

		private MotionTracker motionTracker;

		private Rigidbody rbody;

		private float minMagSq;

		private float maxMagSq;

		protected override void CopyTo(Action _destWarper)
		{
			WaitForVelocityAction waitForVelocityAction = _destWarper as WaitForVelocityAction;
			waitForVelocityAction.Min = Min;
			waitForVelocityAction.Max = Max;
			waitForVelocityAction.MinMag = MinMag;
			waitForVelocityAction.MaxMag = MaxMag;
			waitForVelocityAction.IgnoreX = IgnoreX;
			waitForVelocityAction.IgnoreY = IgnoreY;
			waitForVelocityAction.IgnoreZ = IgnoreZ;
			waitForVelocityAction.IgnoreMinMag = IgnoreMinMag;
			waitForVelocityAction.IgnoreMaxMag = IgnoreMaxMag;
			base.CopyTo(_destWarper);
		}

		protected override void OnEnable()
		{
			motionTracker = GetTarget().GetComponent<MotionTracker>();
			rbody = GetTarget().GetComponent<Rigidbody>();
			minMagSq = MinMag * MinMag;
			maxMagSq = MaxMag * MaxMag;
			base.OnEnable();
		}

		protected override void Update()
		{
			Vector3 vector = Vector3.zero;
			bool flag = true;
			bool flag2 = true;
			if (motionTracker != null)
			{
				vector = motionTracker.FrameVelocity;
			}
			else if (rbody != null)
			{
				vector = rbody.velocity;
			}
			else
			{
				flag = false;
				flag2 = false;
			}
			if (flag)
			{
				if (!IgnoreX && (vector.x < Min.x || vector.x > Max.x))
				{
					flag2 = false;
				}
				if (!IgnoreY && (vector.y < Min.y || vector.y > Max.y))
				{
					flag2 = false;
				}
				if (!IgnoreZ && (vector.z < Min.z || vector.z > Max.z))
				{
					flag2 = false;
				}
				if (!IgnoreMinMag && vector.sqrMagnitude < minMagSq)
				{
					flag2 = false;
				}
				if (!IgnoreMaxMag && vector.sqrMagnitude < maxMagSq)
				{
					flag2 = false;
				}
			}
			if (!flag2)
			{
				Completed();
			}
		}
	}
}
