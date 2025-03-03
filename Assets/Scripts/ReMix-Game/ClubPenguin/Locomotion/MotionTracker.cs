using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class MotionTracker : MonoBehaviour, IMotionTrackerAdapter
	{
		private static readonly float sampleTime = 0.15f;

		private Vector3 prevPos;

		private Vector3 prevFramePos;

		private float elapsedTime;

		public Vector3 Velocity
		{
			get;
			private set;
		}

		public Vector3 FrameVelocity
		{
			get;
			private set;
		}

		private void OnEnable()
		{
			prevPos = base.transform.position;
			prevFramePos = prevPos;
		}

		private void Update()
		{
			FrameVelocity = (base.transform.position - prevFramePos) / Time.deltaTime;
			prevFramePos = base.transform.position;
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= sampleTime)
			{
				Velocity = (base.transform.position - prevPos) / elapsedTime;
				prevPos = base.transform.position;
				elapsedTime = 0f;
			}
		}
	}
}
