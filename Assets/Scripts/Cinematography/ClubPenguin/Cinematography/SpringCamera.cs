using ClubPenguin.Core;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class SpringCamera : BaseCamera
	{
		[Tooltip("When enabled this camera will always snap")]
		public bool AlwaysSnapOverride = false;

		public float SpringFrequency = 10f;

		public float SpringDamping = 1.2f;

		public float TurnSmoothing = 10f;

		private Vector3 velocity = Vector3.zero;

		protected override void Move(Vector3 goal, bool snap = false)
		{
			Vector3 position = base.transform.position;
			if (snap || AlwaysSnapOverride)
			{
				velocity = Vector3.zero;
				base.transform.position = goal;
			}
			else
			{
				Vector3 position2 = base.transform.position;
				for (int i = 0; i < 3; i++)
				{
					float pPos = position2[i];
					float equilibriumPos = goal[i];
					float pVel = velocity[i];
					FeepMath.calcDampedSimpleHarmonicMotion(ref pPos, ref pVel, equilibriumPos, Time.deltaTime, SpringFrequency, SpringDamping);
					position2[i] = pPos;
					velocity[i] = pVel;
				}
				base.transform.position = position2;
			}
			if (base.transform.position != position)
			{
				Moved.InvokeSafe();
			}
		}

		protected override void Aim(Quaternion aim, bool snap = false)
		{
			if (snap)
			{
				base.transform.rotation = aim;
			}
			else
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, aim, TurnSmoothing * Time.deltaTime);
			}
		}

		protected override void Aim(ref Quaternion from, Quaternion aim)
		{
			from = Quaternion.Slerp(from, aim, TurnSmoothing * Time.deltaTime);
		}
	}
}
