using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	public class RewardPopupScreenCount : RewardPopupScreen
	{
		private const float ALPHA_TWEEN_SPEED = 5f;

		private const float TWEEN_TIME = 1f;

		public Text RewardCategoryText;

		public Text RewardCountText;

		public Tweenable CountPanel;

		private DRewardPopupScreenCount screenData;

		private bool isIntroComplete;

		private Vector3 originalCountTextPosition;

		public override void Init(DRewardPopupScreen screenData, RewardPopupController popupController)
		{
			this.screenData = (DRewardPopupScreenCount)screenData;
			RewardCategoryText.text = RewardUtils.GetUnlockText(this.screenData.CountCategory);
			RewardCountText.text = string.Format("+ {0}", this.screenData.Count);
			popupController.RewardChest.ChestAnimator.SetTrigger("LaunchItems");
			isIntroComplete = false;
			tweenCountText();
		}

		public override void OnClick()
		{
			if (isIntroComplete)
			{
				screenComplete();
			}
		}

		private void tweenCountText()
		{
			originalCountTextPosition = CountPanel.transform.position;
			CountPanel.transform.position = CountPanel.transform.position + new Vector3(0f, -2f, 0f);
			Tweenable countPanel = CountPanel;
			countPanel.TweenCompleteAction = (Action<GameObject>)Delegate.Combine(countPanel.TweenCompleteAction, new Action<GameObject>(onTweenComplete));
			CountPanel.TweenPosition(originalCountTextPosition, 1f);
			CountPanel.transform.localScale = Vector3.zero;
			CountPanel.TweenScale(Vector3.one, 1f);
			CoroutineRunner.Start(restoreCountTextAlpha(), this, "");
		}

		private void onTweenComplete(GameObject tweenGO)
		{
			isIntroComplete = true;
		}

		private IEnumerator restoreCountTextAlpha()
		{
			Color textColor = RewardCountText.color;
			textColor.a = 0f;
			while (textColor.a < 1f)
			{
				textColor.a += 5f * Time.deltaTime;
				RewardCountText.color = textColor;
				yield return null;
			}
			textColor.a = 1f;
			RewardCountText.color = textColor;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
