#define UNITY_ASSERTIONS
using ClubPenguin.Core;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class RailDolly : MonoBehaviour
	{
		public SmoothBezierCurve Rail;

		public float DelayBeforeStartingMotion = 0f;

		public float DelayAfterFinishingMotion = 0f;

		public float Duration = 5f;

		public AnimationCurve MotionCurve;

		private float timer;

		private List<float> railSegmentLengths;

		private Dictionary<int, List<float>> railSegmentArcLengths;

		private int segmentArcSubdivisions = 100;

		private float totalRailLength;

		public bool Active
		{
			get;
			set;
		}

		public float Timer
		{
			get
			{
				return timer;
			}
			set
			{
				timer = Mathf.Clamp(value, 0f, TimerMax);
			}
		}

		public float TimerMax
		{
			get
			{
				return DelayBeforeStartingMotion + Duration + DelayAfterFinishingMotion;
			}
		}

		public bool IsComplete
		{
			get
			{
				return Timer >= TimerMax;
			}
		}

		private void Awake()
		{
			if (Rail == null)
			{
				Rail = GetComponent<SmoothBezierCurve>();
			}
			Debug.Assert(Rail != null, "Dolly camera requires a rail to move on.");
			calculateRailLengths();
			Active = false;
			Timer = 0f;
		}

		public Vector3 GetDollyPosition()
		{
			return GetDollyPositionAtTime(Timer);
		}

		public Vector3 GetDollyPositionAtTime(float time)
		{
			if (totalRailLength > 0f)
			{
				float distance = MotionCurve.Evaluate(Mathf.Clamp(time - DelayBeforeStartingMotion, 0f, Duration) / Duration) * totalRailLength;
				return interpolateByDistance(distance);
			}
			return base.transform.position;
		}

		private Vector3 interpolateByDistance(float distance)
		{
			distance = Mathf.Clamp(distance, 0f, totalRailLength);
			int num = 0;
			for (int i = 0; i < railSegmentLengths.Count; i++)
			{
				if (distance <= railSegmentLengths[i] || i == railSegmentLengths.Count - 1)
				{
					num = i;
					break;
				}
				distance -= railSegmentLengths[i];
			}
			Debug.Assert(num >= 0 && num < railSegmentLengths.Count, "Current segment is out of bounds.");
			int num2 = -1;
			List<float> list = railSegmentArcLengths[num];
			for (int i = 0; i < segmentArcSubdivisions; i++)
			{
				if (distance <= list[i] || i == segmentArcSubdivisions - 1)
				{
					num2 = i;
					break;
				}
				distance -= list[i];
			}
			Debug.Assert(num2 >= 0 && num2 < list.Count, "Current arc is out of bounds.");
			float u = 1f / (float)segmentArcSubdivisions * (float)num2 + distance / list[num2] / (float)segmentArcSubdivisions;
			return Rail.InterpolateSegment(num, u);
		}

		private void Update()
		{
			if (Active)
			{
				Timer = Mathf.Clamp(Timer + Time.deltaTime, 0f, TimerMax);
			}
		}

		private void calculateRailLengths()
		{
			railSegmentLengths = new List<float>(Rail.SegmentCount);
			railSegmentArcLengths = new Dictionary<int, List<float>>();
			totalRailLength = 0f;
			float num = 0f;
			for (int i = 0; i < Rail.SegmentCount; i++)
			{
				num = 0f;
				railSegmentArcLengths.Add(i, new List<float>(segmentArcSubdivisions));
				float num2 = 1f / (float)segmentArcSubdivisions;
				Vector3 zero = Vector3.zero;
				Vector3 vector = Rail.InterpolateSegment(i, 0f);
				float num3 = 0f;
				for (int j = 0; j < segmentArcSubdivisions; j++)
				{
					zero = vector;
					vector = Rail.InterpolateSegment(i, num2 * (float)(j + 1));
					num3 = Vector3.Distance(zero, vector);
					railSegmentArcLengths[i].Add(num3);
					num += num3;
				}
				railSegmentLengths.Add(num);
				totalRailLength += num;
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (Rail != null)
			{
				calculateRailLengths();
				Gizmos.color = Rail.Color * 0.5f;
				Gizmos.DrawSphere(GetDollyPosition(), 0.25f);
			}
		}
	}
}
