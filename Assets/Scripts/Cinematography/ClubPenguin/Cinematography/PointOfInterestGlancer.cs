#define UNITY_ASSERTIONS
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	internal class PointOfInterestGlancer : Glancer
	{
		[Range(0f, 90f)]
		[Tooltip("Maximum angle for the glance.  If the glance needs an angle greater than this then no glance will occur.")]
		public float MaxGlanceAngle = 20f;

		[Range(0f, 1f)]
		[Tooltip("Multiplier for the glance. If this is set to 1.0 then the glance will be directly at the point of interest.")]
		public float GlancePercent = 1f;

		[Tooltip("When checked, this glancer blends to points of interest using animation curve.")]
		public bool UseBlendCurve = false;

		public AnimationCurve BlendCurve = new AnimationCurve();

		public float BlendDuration = 1f;

		private Quaternion blendInStartGlance = Quaternion.identity;

		private Quaternion blendOutStartGlance = Quaternion.identity;

		private Vector3 prevPOIPos;

		private Vector3 originalPOI;

		private float elapsedTime;

		private bool startNewBlend;

		private bool isBlendingIn;

		private PointOfInterestDirector pointOfInterestDirector = null;

		private Vector3 oldPointOfInterestPosition = Vector3.zero;

		private bool oldHasPointOfInterest = false;

		private bool isBlendingOut;

		private void OnEnable()
		{
			startNewBlend = true;
		}

		private void Update()
		{
			if (pointOfInterestDirector != null)
			{
				if (oldHasPointOfInterest != pointOfInterestDirector.HasActivePointOfInterest)
				{
					Dirty = true;
					oldHasPointOfInterest = pointOfInterestDirector.HasActivePointOfInterest;
				}
				else if (pointOfInterestDirector.HasActivePointOfInterest)
				{
					Vector3 weightedPointOfInterest = pointOfInterestDirector.GetWeightedPointOfInterest();
					Dirty = ((weightedPointOfInterest - oldPointOfInterestPosition).sqrMagnitude > 0.01f);
					oldPointOfInterestPosition = weightedPointOfInterest;
				}
			}
			Dirty |= UseBlendCurve;
		}

		private void OnDrawGizmosSelected()
		{
			if (pointOfInterestDirector != null && pointOfInterestDirector.HasActivePointOfInterest)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(pointOfInterestDirector.GetWeightedPointOfInterest(), 0.5f);
			}
		}

		public override bool Aim(ref Setup setup)
		{
			bool result = false;
			if (pointOfInterestDirector == null)
			{
				pointOfInterestDirector = setup.Focus.GetComponent<PointOfInterestDirector>();
				startNewBlend = true;
				Debug.Assert(pointOfInterestDirector != null, "Camera focus object does not have Point of Interest Director component on it.  The PointOfInterestGlancer will not work.");
			}
			if (pointOfInterestDirector != null && pointOfInterestDirector.HasActivePointOfInterest)
			{
				Vector3 weightedPointOfInterest = pointOfInterestDirector.GetWeightedPointOfInterest();
				if (isBlendingOut || weightedPointOfInterest != prevPOIPos)
				{
					startNewBlend = true;
					prevPOIPos = weightedPointOfInterest;
				}
				if (startNewBlend)
				{
					if (!isBlendingOut)
					{
						blendInStartGlance = setup.Glance;
					}
					restartBlend(ref setup);
				}
				Vector3 vector = setup.LookAt - setup.Goal;
				Vector3 vector2 = weightedPointOfInterest - setup.Goal;
				Debug.DrawLine(setup.LookAt, setup.Goal, Color.green, 1f);
				Debug.DrawLine(weightedPointOfInterest, setup.Goal, Color.yellow, 1f);
				float num = Vector3.Angle(vector, vector2);
				if (num < MaxGlanceAngle)
				{
					Vector3 axis = Vector3.Cross(vector, vector2);
					axis.z = 0f;
					axis.Normalize();
					Quaternion quaternion = Quaternion.AngleAxis(num * GlancePercent, axis);
					if (UseBlendCurve && BlendDuration > 0f)
					{
						elapsedTime += Time.deltaTime;
						if (elapsedTime > BlendDuration)
						{
							elapsedTime = BlendDuration;
						}
						setup.Glance = Quaternion.Slerp(blendInStartGlance, quaternion, BlendCurve.Evaluate(elapsedTime / BlendDuration));
						blendOutStartGlance = setup.Glance;
						isBlendingIn = true;
					}
					else
					{
						setup.Glance = quaternion;
					}
					result = true;
				}
				else if (isBlendingIn)
				{
					isBlendingIn = false;
					isBlendingOut = true;
					elapsedTime = 0f;
				}
			}
			else
			{
				if (isBlendingIn)
				{
					isBlendingIn = false;
					isBlendingOut = true;
					elapsedTime = 0f;
				}
				if (isBlendingOut)
				{
					elapsedTime += Time.deltaTime;
					if (elapsedTime > BlendDuration)
					{
						restartBlend(ref setup);
					}
					else
					{
						setup.Glance = Quaternion.Slerp(blendOutStartGlance, Quaternion.identity, BlendCurve.Evaluate(elapsedTime / BlendDuration));
						blendInStartGlance = setup.Glance;
						result = true;
					}
				}
				else
				{
					restartBlend(ref setup);
				}
			}
			return result;
		}

		private void restartBlend(ref Setup setup)
		{
			elapsedTime = 0f;
			startNewBlend = false;
			isBlendingIn = false;
			isBlendingOut = false;
		}
	}
}
