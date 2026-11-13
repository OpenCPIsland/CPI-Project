using ClubPenguin.Locomotion.Primitives;
using System;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class SlideControllerData : LocomotionControllerData
	{
		[Serializable]
		public class FabricAudio
		{
			public SurfaceEffectsData SurfaceSamplingData;

			public string VelocityMagEventName;

			public string VelocityMagRTP;

			public string YVelocityEventName;

			public string YVelocityRTP;

			public string SupportedEventName;

			public string AirborneEventName;

			public string LandedEventName;
		}

		public GameObject PilotPrefab;

		public GameObject SledPrefab;

		public ParticleSystem ParticlesPrefab;

		public float BuildMomentumTime = 1f;

		public float MinHeightForTrick = 0.1f;

		public float TurnSmoothing = 2f;

		public float SpinScale = 2f;

		public float SpinScaleOnWater = 0.5f;

		public float MaxSpinRate = 3f;

		public float MaxSpinRateOnWater = 1.5f;

		public float SurfaceSmoothing = 5f;

		public float SpringAccel = 11f;

		public float StartingSpringAccel = 1f;

		public float ImpulseScale = 0.06f;

		public float MaxManualSpeedBeforeMomentumTakeover = 2.5f;

		public float HopSpeed = 4f;

		public float JumpSpeed = 5f;

		public float EjectHopSpeed = 3.5f;

		public float EjectDamper = 0.25f;

		public float GroundedDistance = 0.75f;

		public float RotationSmoothing = 2f;

		public Vector3 SledOffsetFromPilot = new Vector3(0f, -0.05f, 0f);

		public float MinMagnitudeForBump = 2f;

		public float IdleLoopTransitionSmoothing = 12f;

		public AnimationCurve WaveCurve;

		public float ImpulseScaleOnWater = 0.05f;

		public ParticleSystem WaterRipples;

		public GameObject WaterSplash;

		public float SplashCooldown = 0.75f;

		public float SplashOffset = 0.1f;

		public float WaterRippleOffset = -0.12f;

		public float WaterRippleSmoothing = 1f;

		public string ChestBoneName = "chest_jnt";

		public Vector3 ChestBoneRotation = new Vector3(5f, 0f, 0f);

		public WaterProperties.Properties WaterProperties;

		[Range(0f, 100f)]
		public float MinSpeedForSlowLoopAnim = 1.5f;

		[Range(0.01f, 100f)]
		public float MinSpeedForFastLoopAnim = 3f;

		public ForceAccumulatorPrimitiveData ImpulseProperties;

		public FabricAudio Audio = new FabricAudio();
	}
}
