using ClubPenguin.Accessibility;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Animator))]
	public class TrayNotification : MonoBehaviour
	{
		protected const string HIDE_ANIMATION_PARAM = "HideNotification";

		public Action<NotificationCompleteEnum> ENotificationCompleted;

		public GameObject NotificationView;

		public GameObject NotificationButtonView;

		public Text messageWithoutButtons;

		public Text messageWithButtons;

		public Image TintableHeader;

		public SpeakAccessibilityOnStart AccessibilitySpeak;

		protected Animator anim;

		public virtual void Show(DNotification data)
		{
			if (anim == null)
			{
				anim = GetComponent<Animator>();
			}
			anim.SetBool("HideNotification", false);
			messageWithoutButtons.text = data.Message;
			if (TintableHeader != null)
			{
				TintableHeader.color = data.HeaderTint;
			}
			if (data.ContainsButtons)
			{
				NotificationView.SetActive(false);
				if (NotificationButtonView != null)
				{
					NotificationButtonView.SetActive(true);
				}
				messageWithButtons.text = data.Message;
			}
			else
			{
				if (NotificationButtonView != null)
				{
					NotificationButtonView.SetActive(false);
				}
				NotificationView.SetActive(true);
			}
			if (AccessibilitySpeak != null)
			{
				AccessibilitySpeak.enabled = true;
			}
			Service.Get<EventDispatcher>().AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransition);
			if (data.AutoClose)
			{
				CoroutineRunner.Start(notificationDisplayTime(data.PopUpDelayTime), this, "notificationDisplayTime");
			}
		}

		public virtual void Dismiss(bool showAnimation = true)
		{
			disableButtons();
			if (showAnimation)
			{
				CoroutineRunner.Start(playDismissAnimation(), this, "notificationDismiss");
			}
			else if (ENotificationCompleted != null)
			{
				ENotificationCompleted(NotificationCompleteEnum.dismissed);
			}
		}

		private IEnumerator playDismissAnimation()
		{
			anim.SetBool("HideNotification", true);
			yield return new WaitForEndOfFrame();
			float animTime = anim.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(animTime);
			if (ENotificationCompleted != null)
			{
				ENotificationCompleted(NotificationCompleteEnum.dismissed);
			}
		}

		public void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		protected virtual IEnumerator notificationDisplayTime(float delaySeconds)
		{
			yield return new WaitForSeconds(delaySeconds);
			yield return animateAway();
		}

		protected IEnumerator animateAway()
		{
			anim.SetBool("HideNotification", true);
			yield return new WaitForEndOfFrame();
			float animTime = anim.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(animTime);
			if (ENotificationCompleted != null)
			{
				ENotificationCompleted(NotificationCompleteEnum.animationComplete);
			}
		}

		public void AcceptButton()
		{
			disableButtons();
			if (ENotificationCompleted != null)
			{
				ENotificationCompleted(NotificationCompleteEnum.acceptButton);
			}
		}

		public void DeclineButton()
		{
			disableButtons();
			if (ENotificationCompleted != null)
			{
				ENotificationCompleted(NotificationCompleteEnum.declineButton);
			}
		}

		public void DismissButton()
		{
			disableButtons();
			if (ENotificationCompleted != null)
			{
				ENotificationCompleted(NotificationCompleteEnum.dismissed);
			}
		}

		private void disableButtons()
		{
			Button[] componentsInChildren = GetComponentsInChildren<Button>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
			}
		}

		private bool onSceneTransition(SceneTransitionEvents.TransitionStart evt)
		{
			if (ENotificationCompleted != null)
			{
				ENotificationCompleted(NotificationCompleteEnum.leftRoom);
			}
			CoroutineRunner.StopAllForOwner(this);
			Service.Get<EventDispatcher>().RemoveListener<SceneTransitionEvents.TransitionStart>(onSceneTransition);
			UnityEngine.Object.Destroy(base.gameObject);
			return false;
		}
	}
}
