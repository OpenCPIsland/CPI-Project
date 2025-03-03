using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class FixedOffsetTowardsCameraGoalPlanner : GoalPlanner
	{
		public float DistanceOnIdle = 3f;

		public float HeightOffset = 0f;

		public bool AlwaysUseDistOnIdle = false;

		public AnimationCurve Curve = new AnimationCurve();

		public float Duration = 1f;

		public AnimationCurve DistanceCurve = new AnimationCurve();

		public float DistanceDuration = 1f;

		private GameObject localPlayer;

		private Animator anim;

		private bool initialized;

		private Vector3 originalGoal = Vector3.zero;

		private Vector3 desiredGoal = Vector3.zero;

		private Vector3 flatInvCamDir;

		private float originalDistance;

		private float originalHeight;

		private float elapsedTime;

		private float elapsedDistanceTime;

		private float curDistance;

		private float curHeight;

		private float desiredDistance;

		private float desiredHeight;

		private bool zoomedInOnce;

		public void Start()
		{
			setup();
		}

		private void setup()
		{
			if (localPlayer == null)
			{
				localPlayer = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
				anim = localPlayer.GetComponent<Animator>();
				if (Duration <= 0f)
				{
					Duration = 1f;
				}
			}
		}

		public void OnEnable()
		{
			initialized = false;
		}

		private void initialize(ref Setup setup)
		{
			flatInvCamDir = setup.Goal - setup.Focus.position;
			originalDistance = flatInvCamDir.magnitude;
			originalHeight = flatInvCamDir.y;
			flatInvCamDir.y = 0f;
			flatInvCamDir.Normalize();
			originalGoal = setup.Goal;
			elapsedTime = 0f;
			elapsedDistanceTime = 0f;
			zoomedInOnce = getIdealDistanceToTarget(ref desiredDistance, ref desiredHeight);
			curDistance = desiredDistance;
			curHeight = desiredHeight;
			initialized = true;
		}

		private bool getIdealDistanceToTarget(ref float distance, ref float height)
		{
			bool result = false;
			if (AlwaysUseDistOnIdle)
			{
				distance = DistanceOnIdle;
				height = HeightOffset;
				result = true;
			}
			else
			{
				setup();
				distance = originalDistance;
				height = originalHeight;
				AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(AnimationHashes.Layers.Base);
				if (LocomotionUtils.IsIdling(currentAnimatorStateInfo) || LocomotionHelper.IsCurrentControllerOfType<SitController>(localPlayer))
				{
					distance = DistanceOnIdle;
					height = HeightOffset;
					result = true;
				}
			}
			return result;
		}

		private void calcDesiredGoal(ref Setup setup)
		{
			float distance = 0f;
			float height = 0f;
			bool idealDistanceToTarget = getIdealDistanceToTarget(ref distance, ref height);
			if (distance != desiredDistance)
			{
				if (idealDistanceToTarget)
				{
					if (zoomedInOnce)
					{
						desiredDistance = curDistance;
						desiredHeight = curHeight;
					}
					else
					{
						zoomedInOnce = true;
						desiredDistance = distance;
						desiredHeight = height;
					}
				}
				else
				{
					desiredDistance = distance;
					desiredHeight = height;
				}
				elapsedDistanceTime = 0f;
			}
			float t = DistanceCurve.Evaluate(elapsedDistanceTime / DistanceDuration);
			curDistance = Mathf.Lerp(curDistance, desiredDistance, t);
			curHeight = Mathf.Lerp(curHeight, desiredHeight, t);
			desiredGoal = setup.Focus.position + flatInvCamDir * curDistance;
			desiredGoal.y += curHeight;
		}

		public override void Plan(ref Setup setup)
		{
			if (!initialized)
			{
				initialize(ref setup);
			}
			elapsedTime += Time.deltaTime;
			if (elapsedTime > Duration)
			{
				elapsedTime = Duration;
			}
			elapsedDistanceTime += Time.deltaTime;
			if (elapsedDistanceTime > DistanceDuration)
			{
				elapsedDistanceTime = DistanceDuration;
			}
			calcDesiredGoal(ref setup);
			setup.Goal = Vector3.Lerp(originalGoal, desiredGoal, Curve.Evaluate(elapsedTime / Duration));
			Dirty = true;
		}
	}
}
