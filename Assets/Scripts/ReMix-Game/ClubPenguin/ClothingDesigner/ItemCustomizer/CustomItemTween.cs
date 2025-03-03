using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class CustomItemTween : MonoBehaviour
	{
		private const iTween.EaseType DEFAULT_EASE_TYPE = iTween.EaseType.easeInOutSine;

		private const float DEFAULT_EASE_TIME = 2f;

		private static readonly Vector3 DEFAULT_BEZIER_OFFSET = new Vector3(-4.327f, 0.849f, 0.496f);

		public Action<GameObject> CurveCompleted;

		private Vector3 destinationPoint;

		private Vector3 startingPoint;

		private Vector3 offset;

		private bool isAnimating;

		public Vector3 BezierOffsetPoint
		{
			get;
			set;
		}

		public iTween.EaseType TweenEaseType
		{
			get;
			set;
		}

		public float TweenEaseTime
		{
			get;
			set;
		}

		private void Awake()
		{
			BezierOffsetPoint = DEFAULT_BEZIER_OFFSET;
			TweenEaseType = iTween.EaseType.easeInOutSine;
			TweenEaseTime = 2f;
			isAnimating = false;
		}

		public void MoveTo(Vector3 destinationPoint, float delay)
		{
			if (!isAnimating)
			{
				this.destinationPoint = destinationPoint;
				isAnimating = true;
				setupPoints();
				CoroutineRunner.Start(startTweenOverCurve(delay), this, "startTweenOverCurve");
			}
		}

		private void setupPoints()
		{
			offset = calculateRenderersOffsetPoint();
			startingPoint = base.transform.position;
			BezierOffsetPoint += offset;
			destinationPoint += offset;
		}

		private IEnumerator startTweenOverCurve(float delay)
		{
			yield return new WaitForSeconds(delay);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", TweenEaseTime, "onupdatetarget", base.gameObject, "oncomplete", "curveComplete", "onupdate", "animateOverBezierCurve", "easetype", TweenEaseType));
		}

		private void animateOverBezierCurve(float t)
		{
			float num = 1f - t;
			float num2 = num * num;
			float num3 = 2f * num * t;
			float num4 = t * t;
			float x = num2 * startingPoint.x + num3 * BezierOffsetPoint.x + num4 * destinationPoint.x;
			float y = num2 * startingPoint.y + num3 * BezierOffsetPoint.y + num4 * destinationPoint.y;
			float z = num2 * startingPoint.z + num3 * BezierOffsetPoint.z + num4 * destinationPoint.z;
			base.transform.position = new Vector3(x, y, z);
		}

		private void curveComplete()
		{
			isAnimating = false;
			if (CurveCompleted != null)
			{
				CurveCompleted(base.gameObject);
				CurveCompleted = null;
			}
		}

		private Vector3 calculateRenderersOffsetPoint()
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			Bounds bounds = componentsInChildren[0].bounds;
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				bounds.Encapsulate(componentsInChildren[i].bounds);
			}
			Vector3 center = bounds.center;
			return base.transform.position - center;
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
