using UnityEngine;

namespace ClubPenguin.Locomotion.Primitives
{
	public class SwimPrimitiveData : ScriptableObject
	{
		public ParticleSystem BreathBubbles;

		public ParticleSystem SwimBubbles;

		public ParticleSystem TorpedoBubbles;

		public float MinDistFromSurfaceForBubbles = 2f;

		public float Accel = 0.5f;

		public float MinSpeedMult = 0.1f;

		public float RotationSmoothing = 5f;

		public float DragSmoothing = 1.5f;

		public float SpinSmoothing = 5f;

		[Range(0f, 179f)]
		[Tooltip("The offset threshold to our previous rotation before we start rotating again. 0 will always rotate")]
		public int RotationDegreesOffsetThreshold = 15;
	}
}
