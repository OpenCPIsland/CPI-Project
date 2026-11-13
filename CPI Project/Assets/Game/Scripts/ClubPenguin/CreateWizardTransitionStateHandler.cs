using DevonLocalization.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class CreateWizardTransitionStateHandler : AbstractStateHandler
	{
		[Serializable]
		public struct TransitionInfo
		{
			public CreateWizardStepAnimator Animator;

			public string TriggerName;

			public bool ChangeActive;
		}

		public TransitionInfo exitTransition = default(TransitionInfo);

		public TransitionInfo enterTransition = default(TransitionInfo);

		public string completeEvent = "complete";

		[Header("Progress Bar")]
		public string progressBarTrigger = string.Empty;

		public string avatarAnimation = string.Empty;

		public CreateWizardProgressTextInfo ProgressText;

		protected override void OnEnter()
		{
			CreatePopupWizardContentController component = GetComponent<CreatePopupWizardContentController>();
			if (component != null)
			{
				component.UpdateProgressBar(progressBarTrigger, avatarAnimation);
			}
			startExitTransition();
		}

		private void startExitTransition()
		{
			if (exitTransition.Animator != null)
			{
				exitTransition.Animator.PlayAnimation(exitTransition.TriggerName, onTransitionExitComplete);
			}
			else
			{
				startEnterTransition();
			}
		}

		private void onTransitionExitComplete()
		{
			if (exitTransition.ChangeActive)
			{
				exitTransition.Animator.gameObject.SetActive(false);
			}
			startEnterTransition();
		}

		private void startEnterTransition()
		{
			if (enterTransition.Animator != null)
			{
				bool flag = !string.IsNullOrEmpty(enterTransition.TriggerName);
				if (enterTransition.ChangeActive)
				{
					enterTransition.Animator.gameObject.SetActive(true);
				}
				if (flag)
				{
					enterTransition.Animator.PlayAnimation(enterTransition.TriggerName, onTransitionEnterComplete);
				}
				if (!flag)
				{
					onTransitionEnterComplete();
				}
			}
			else
			{
				onTransitionEnterComplete();
			}
		}

		private void onTransitionEnterComplete()
		{
			if (ProgressText.TextField != null)
			{
				ProgressText.TextField.text = Service.Get<Localizer>().GetTokenTranslation(ProgressText.Token);
			}
			rootStateMachine.SendEvent(completeEvent);
		}
	}
}
