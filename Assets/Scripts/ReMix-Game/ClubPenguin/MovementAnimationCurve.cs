using ClubPenguin.Net;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class MovementAnimationCurve : ProximityBroadcaster
	{
		[Tooltip("Animation curve to apply to x-coordinate")]
		public AnimationCurve animCurveX;

		[Tooltip("Value to multiply X curve data by")]
		public float magnitudeX = 1f;

		[Tooltip("Reverses X animation if checked")]
		public bool animReverseX = false;

		[Tooltip("Animation curve to apply to y-coordinate")]
		public AnimationCurve animCurveY;

		[Tooltip("Value to multiply Y curve data by")]
		public float magnitudeY = 1f;

		[Tooltip("Reverses Y animation if checked")]
		public bool animReverseY = false;

		[Tooltip("Amount of seconds to play 1 cycle")]
		public float animSecondsPerCycle = 1f;

		[Range(0f, 1f)]
		public float startPosition = 0f;

		public GameObject RelativeToObject;

		private Vector3 originalPosition;

		private INetworkServicesManager network;

		public bool IsActive = false;

		public override void Start()
		{
			base.Start();
			SetOriginalPosition();
		}

		private void OnEnable()
		{
			network = Service.Get<INetworkServicesManager>();
		}

		private void Update()
		{
			if (IsActive)
			{
				float curveTime = (network != null) ? (startPosition + (float)(network.GameTimeMilliseconds % (int)(animSecondsPerCycle * 1000f)) / 1000f / animSecondsPerCycle) : (startPosition + Time.time % (float)(int)(animSecondsPerCycle * 1f) / 1f / animSecondsPerCycle);
				if (RelativeToObject == null)
				{
					base.gameObject.transform.position = GetPointAtTime(originalPosition, curveTime);
				}
				else
				{
					base.gameObject.transform.position = RelativeToObject.transform.position + GetPointAtTime(originalPosition, curveTime);
				}
			}
		}

		public override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			if (animCurveX == null || animCurveY == null || (animCurveX.length <= 0 && animCurveY.length <= 0))
			{
				return;
			}
			for (float num5 = 0f; num5 <= 1f; num5 += 0.05f)
			{
				float curveTime = startPosition + num5;
				if (!Application.isPlaying)
				{
					originalPosition = base.transform.position;
				}
				Vector3 pointAtTime = GetPointAtTime(originalPosition, curveTime);
				if (pointAtTime.x < num)
				{
					num = pointAtTime.x;
				}
				else if (pointAtTime.x > num3)
				{
					num3 = pointAtTime.x;
				}
				if (pointAtTime.y < num2)
				{
					num2 = pointAtTime.y;
				}
				else if (pointAtTime.y > num4)
				{
					num4 = pointAtTime.y;
				}
			}
			Gizmos.color = Color.white;
			Gizmos.DrawLine(new Vector3(num, num2, base.gameObject.transform.position.z), new Vector3(num3, num4, base.gameObject.transform.position.z));
		}

		private Vector3 GetPointAtTime(Vector3 origPos, float curveTime)
		{
			float num = 0f;
			float num2 = 0f;
			if (animCurveX != null && animCurveX.length > 0)
			{
				num = animCurveX.Evaluate(curveTime) * magnitudeX;
				if (animReverseX)
				{
					num *= -1f;
				}
			}
			if (animCurveY != null && animCurveY.length > 0)
			{
				num2 = animCurveY.Evaluate(curveTime) * magnitudeY;
				if (animReverseY)
				{
					num2 *= -1f;
				}
			}
			Vector3 result = origPos;
			result.x += num;
			result.y += num2;
			return result;
		}

		public void SetOriginalPosition()
		{
			originalPosition = base.gameObject.transform.position;
		}

		public override void OnProximityEnter(ProximityListener other)
		{
			IsActive = true;
		}

		public override void OnProximityExit(ProximityListener other)
		{
			IsActive = false;
		}
	}
}
