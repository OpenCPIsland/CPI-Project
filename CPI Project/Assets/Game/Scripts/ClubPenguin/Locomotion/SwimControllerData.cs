using ClubPenguin.Locomotion.Primitives;
using System;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class SwimControllerData : LocomotionControllerData
	{
		public enum FreezeAxisType
		{
			None = -1,
			X,
			Y,
			Z
		}

		[Serializable]
		public struct LowAirThreshold
		{
			public float AirSupplyThreshold;

			public float AnimDuration;
		}

		public ParticleSystem Ripples;

		public float RippleHeightOffset = 0.5f;

		public float SplashHeightOffset = 0.5f;

		public GameObject Splash;

		public float MaxDistToConsiderNearSurface = 1f;

		public float MaxDistToConsiderOnSurface = 0.4f;

		public float HitReactRotationSmoothing = 5f;

		public FreezeAxisType FreezeAxis = FreezeAxisType.Z;

		public float FreezeDist = 0f;

		public float SurfaceConstraintSmoothing = 10f;

		public float SurfaceSontraintSnapThreshold = 0.5f;

		public float MaxShallowWaterDepth = 0.4f;

		public float ShallowWaterDepthHysteresis = 0.05f;

		public float ShallowWaterSwimSpeedMultiplier = 0.8f;

		public Transform ResurfaceTransform;

		public Transform QuickResurfacingTransform;

		public LowAirThreshold[] LowAirThresholds;

		public float LowAirAnimSmoothing = 5f;

		public SwimPrimitiveData SwimProperties;

		public ForceAccumulatorPrimitiveData ImpulseProperties;

		public float SwimWithPropIKSmoothing = 5f;

		public string CollisionAudioEvent = "SFX/Player/Swim/Impact";

		public string AirRechargeAudioEvent = "SFX/UI/Swim/AirSupplyFilled";

		public string AirWarningAudioEvent = "SFX/UI/Swim/AirSupplyWarning";

		public string AirCriticalAudioEvent = "SFX/UI/Swim/AirSupplyEmpty";
	}
}
