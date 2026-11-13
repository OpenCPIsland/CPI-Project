using UnityEngine;

namespace ClubPenguin.MiniGames.Fishing
{
	public class FishingGameConfig : ScriptableObject
	{
		[Tooltip("The time delay before cast is complete.")]
		[Header("Time Delays")]
		public float CastingLineDelay;

		[Tooltip("Delay before camera zooms in after catching fish.")]
		public float CameraZoomDelay;

		[Tooltip("Time before the fish drops beside the penguin.")]
		public float PrizeDropDelay;

		[Tooltip("Delay before the penguin celebrates the catch.")]
		public float CelebrateDelay;

		[Tooltip("Time the Too Early or Too Late popup is displayed for.")]
		public float TryAgainPopupShowTime;

		public float HoldPrizeEscapedDelay;

		public float HoldPrizeCaughtDelay;

		public float ReelingAnimationTime;

		[Header("Positions")]
		public Vector3 PlayerRotationTowardsWater;

		public Vector3 PrizeDropOffset;

		public Vector3 BobberLocationInWater;

		[Header("Controller Settings")]
		public float patternRadius = 1.5f;

		public float baseFishSpeed = 0.2f;

		public float spherecastRadius = 0.33f;

		public float spherecastRadiusScare = 1f;

		public float spherecastDepth = 1f;

		public Vector2 reelExtremes = new Vector2(-3f, 2f);

		public float perReelStrength = 1f;

		public float baseFishReelStrength = 0.5f;

		public FishingGamePatternConfig[] patterns = null;

		[Header("Fish Settings")]
		public float fishHideDuration = 2f;

		public float fishScaleDuration = 0.33f;

		public float fishScaleCommon = 0.5f;

		public float fishScaleRare = 0.75f;

		public float fishScaleLegendary = 1f;

		public float fishReelStrengthCommon = 1f;

		public float fishReelStrengthRare = 1.5f;

		public float fishReelStrengthLegendary = 3f;
	}
}
