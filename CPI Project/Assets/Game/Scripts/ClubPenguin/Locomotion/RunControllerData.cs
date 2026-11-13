using System;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class RunControllerData : LocomotionControllerData
	{
		[Serializable]
		public class LocoParams
		{
			[Tooltip("Animation speed multiplier when this locomotion mode begins. Used for initial acceleration.")]
			[Range(0.01f, 1f)]
			public float IntoSpeedMult = 0.1f;

			[Range(0.01f, 1f)]
			[Tooltip("Animation speed multiplier to optionally slowdown avatar when making sharp rotations of MinFacingAngleToResetMomentum or more.")]
			public float TurnSpeedMult = 0.7f;

			[Tooltip("Dampens the avatar's rotational changes. Higher values yield less smoothing. 0 disables smoothing.")]
			public float TurnSmoothing = 15f;

			[Tooltip("Max degrees/sec by which avatar is allowed to rotate, applied after smoothing.")]
			public float MaxTurnDegreesPerSec = 1200f;

			[Tooltip("Controls the time it takes for speed multipliers to return to 1.")]
			public float Acceleration = 3f;

			[Tooltip("This locomotion state's velocity.")]
			[Range(0.01f, 10f)]
			public float Speed = 1f;

			[HideInInspector]
			public float speedMult;
		}

		[Serializable]
		public class SprintLocoParams : LocoParams
		{
			[Tooltip("Min jogging time before triggering sprint locomotion.")]
			public float MinTimeToStartSprinting = 3f;

			[Tooltip("Directly controls sprint's lean weight over LeanDuration seconds in blend tree.")]
			public AnimationCurve LeanCurve;

			[Tooltip("Scales duration of LeanCurve.")]
			public float LeanDuration = 2.5f;

			[HideInInspector]
			public float ElapsedLeanTime;
		}

		[Serializable]
		public class WalkLocoParams : LocoParams
		{
			[HideInInspector]
			public float ElapsedWalkTime;
		}

		[Serializable]
		public class JogLocoParams : LocoParams
		{
			[HideInInspector]
			public float ElapsedJogTime;

			[Tooltip("Min walking time before triggering jog locomotion.")]
			public float MinTimeToStartJogging = 1f;

			public float MinSteerMag = 0.5f;
		}

		[Serializable]
		public class InAirLocoParams : LocoParams
		{
			[Tooltip("Max distance to check for landing surface. If surface is beyond this distance, LandingTime returns infinity.")]
			public float LandingGroundCheckDistance = 10f;

			[Tooltip("Distance from surface at which to trigger a freefall (if other conditions are met).")]
			public float StartFreefallGroundCheckDistance = 0.5f;

			[Tooltip("Min collider velocity Y before collider is considered to have gained enough momentum to trigger freefall.")]
			public float MinVelocityYToStartFreefall = -1f;

			[Tooltip("Max distance to ground to trigger landing when falling.")]
			public float LandingTriggerDistance = 0.1f;
		}

		[Serializable]
		public class IdleLocoParams
		{
			[Tooltip("Dampens the avatar's rotational changes. Higher values yield less smoothing. 0 disables smoothing.")]
			public float TurnSmoothing = 15f;

			[Tooltip("Max degrees/sec by which avatar is allowed to rotate, applied after smoothing.")]
			public float MaxTurnDegreesPerSec = 1200f;

			[Tooltip("The offset threshold to our previous rotation before we start rotating again. 0 will always rotate")]
			[Range(0f, 179f)]
			public int RotationDegreesOffsetThreshold = 15;
		}

		public float JumpSpeed = 4f;

		public float Gravity = 9.8f;

		public float Deceleration = 2f;

		[Tooltip("Resets locomotion's animation multiplier to its TurnSpeedMult value when avatar rotates more than this many degrees/sec.")]
		public float MinFacingAngleToResetMomentum = 600f;

		[Tooltip("Triggers a pivot animation when desired facing direction is greater than this many degrees from current heading. Currently for Sprint only.")]
		public float MinFacingAngleToPivot = 60f;

		[Tooltip("Multiplies the pivot animation to produce illusion of slipery surface. Lower values mean more slippage. Currently for Sprint only.")]
		public float GroundFriction = 1.8f;

		[Tooltip("Amount of translation to preserve when playing stop animation")]
		public float StopAnimVelMultiplier = 0.5f;

		public IdleLocoParams IdleParams;

		public WalkLocoParams WalkParams;

		public JogLocoParams JogParams;

		public SprintLocoParams SprintParams;

		public InAirLocoParams InAirParams;
	}
}
