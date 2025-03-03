using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class RaceControllerData : SlideControllerData
	{
		public GameObject SpeedLinesTubeRacePrefab;

		[Tooltip("Speed below which a constant forward thrust is applied to the player")]
		public float ConstantForwardThrustThreshold = 6f;

		public float ConstantForwardThrust = 0.2f;

		public float LateralThrustScale = 0.5f;

		public float LaunchImpulse = 3f;

		public int CountdownMS = 3000;

		[Tooltip("Override the tube's PhysicMaterial properties")]
		public float Bounciness = 0f;

		public PhysicMaterialCombine BounceCombine = PhysicMaterialCombine.Minimum;

		public RaceTrackProperties.Properties RaceTrackProperties;

		[Tooltip("This flag enables a visual aid for adjusting the playability of the track.")]
		public bool VisualizeTrackSegment = false;
	}
}
