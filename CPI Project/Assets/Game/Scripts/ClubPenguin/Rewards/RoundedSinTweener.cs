using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RoundedSinTweener : MonoBehaviour
	{
		public Action TweenCompleteAction;

		public float CurveDampener = 10f;

		private Transform tweenDestination;

		private float tweenTime;

		private float startTime;

		private Transform itemTransform;

		private Vector3 originalPosition;

		private Vector3 positionModifier;

		private float curveMagnitude;

		private bool isScaling = false;

		private Vector3 originalScale;

		private Vector3 destinationScale;

		public Vector3 DestinationScale
		{
			set
			{
				destinationScale = value;
				isScaling = true;
				originalScale = base.transform.localScale;
			}
		}

		public void StartTween(Transform tweenDestination, float tweenTime)
		{
			this.tweenDestination = tweenDestination;
			this.tweenTime = tweenTime;
			startTime = Time.time;
			itemTransform = base.transform;
			originalPosition = base.transform.position;
			positionModifier = Vector3.zero;
			curveMagnitude = (originalPosition - tweenDestination.position).magnitude / CurveDampener;
			CoroutineRunner.Start(tweenRoutine(), this, "");
		}

		private IEnumerator tweenRoutine()
		{
			float tweenPercentage = 0f;
			while (tweenPercentage < 1f)
			{
				tweenPercentage = getTweenPercentage();
				itemTransform.position = Vector3.Lerp(originalPosition, tweenDestination.position, tweenPercentage);
				float modifierMagnitude = Mathf.Sin((float)Math.PI * tweenPercentage);
				positionModifier.y = modifierMagnitude * curveMagnitude;
				positionModifier.x = modifierMagnitude * curveMagnitude * -1f;
				itemTransform.position += positionModifier;
				if (isScaling)
				{
					itemTransform.localScale = Vector3.Lerp(originalScale, destinationScale, tweenPercentage);
				}
				yield return null;
			}
			if (TweenCompleteAction != null)
			{
				TweenCompleteAction();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private float getTweenPercentage()
		{
			float num = Time.time - startTime;
			return num / tweenTime;
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
