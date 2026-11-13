using ClubPenguin.Rewards;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DisneyStoreTrayAnimator : MonoBehaviour
	{
		public Animator TrayAnimator;

		public Transform ToyboxDestination;

		public Transform MyPenguinDestination;

		[SerializeField]
		private float TWEEN_TIME = 1f;

		[SerializeField]
		private float TWEEN_CURVE_DAMPENER = 10f;

		[SerializeField]
		private float TWEEN_SCALE = 0.5f;

		private readonly int TRAY_CLOSE_TRIGGER = Animator.StringToHash("Close");

		private readonly int TRAY_OPEN_TRIGGER = Animator.StringToHash("Open");

		private RoundedSinTweener tween;

		public float TweenTime
		{
			get
			{
				return TWEEN_TIME;
			}
		}

		public void TweenToTray(Transform tweenDestination, Transform iconTransform)
		{
			openTray();
			tweenIconToDestination(iconTransform, tweenDestination);
		}

		private void tweenIconToDestination(Transform iconTransform, Transform destination)
		{
			iconTransform.SetParent(base.transform, true);
			tween = iconTransform.gameObject.AddComponent<RoundedSinTweener>();
			RoundedSinTweener roundedSinTweener = tween;
			roundedSinTweener.TweenCompleteAction = (Action)Delegate.Combine(roundedSinTweener.TweenCompleteAction, new Action(onTweenToMainNavComplete));
			tween.DestinationScale = Vector3.one * TWEEN_SCALE;
			tween.CurveDampener = TWEEN_CURVE_DAMPENER;
			tween.StartTween(destination, TWEEN_TIME);
		}

		private void onTweenToMainNavComplete()
		{
			tween = null;
			closeTray();
		}

		private void openTray()
		{
			TrayAnimator.SetTrigger(TRAY_OPEN_TRIGGER);
		}

		private void closeTray()
		{
			TrayAnimator.SetTrigger(TRAY_CLOSE_TRIGGER);
		}
	}
}
